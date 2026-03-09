using System.Text.RegularExpressions;

namespace OcrPro;

/// <summary>
/// A single matched region of interest extracted from the OCR word list.
/// </summary>
/// <param name="RoiLabel">Display label, e.g. "ROI A – UPC"</param>
/// <param name="FieldName">Short field name shown as the card tag, e.g. "UPC"</param>
/// <param name="Value">Extracted value string</param>
/// <param name="Bounds">Bounding rectangle in image-pixel coordinates (union of matched words)</param>
record RoiMatch(
    string RoiLabel,
    string FieldName,
    string Value,
    Rectangle Bounds);

/// <summary>
/// Word entry built from Tesseract's page iterator.
/// </summary>
record WordEntry(string Text, Rectangle Bounds);

static class RoiExtractor
{
    // ── Patterns ────────────────────────────────────────────────────────────────
    // ROI B: "Serial" "Number" ":" "<digits>"
    // Fuzzy: allow OCR misreads of "Serial":
    //   - "Senal", "Senial", "Seria1", "Sonal"  — standard \w misreads
    //   - "Ser,al" — comma inside the word (comma not matched by \w, handled separately)
    //   - Leading apostrophe / tick before digits tolerated (e.g. '421... → 1421...)
    private static readonly Regex _serialRx =
        new(@"S[\w,]{1,5}l\s+Number\s*[:\-\.]?\s*(['\u2018\u2019]?\d[\d\-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // ROI C: "Part Number" (and fuzzy variants) followed by the value.
    //
    // Observed failure modes from WinRT OCR:
    //   • "Part Number"          — normal (matched by primary)
    //   • "Part Number:"         — colon separator (matched)
    //   • "Part Number."         — dot separator (matched)
    //   • "PartNumber."          — no space (\s* makes space optional)
    //   • "Pan Number"           — 't'→'n' OCR misread  (NEW: broaden final char)
    //   • "Pari Number"          — 't'→'i' OCR misread
    //   • "art Number"           — leading 'P' dropped
    //   • "Part 945-..."         — "Number" word dropped entirely (fallback pattern)
    //
    // Value capture is intentionally broad — OCR noise chars like '<', 'O' (letter),
    // 'l' (ell), spaces inside the value are all captured and cleaned by NormalisePartValue().

    // Primary: "Part Number" and common OCR variants.
    //   Capture stops at first lowercase character — part numbers are uppercase/digits/dashes only.
    //   Max 22 chars covers DDD-DDDDD-DDDD-DDD (18) with a little slack for OCR noise.
    private static readonly Regex _partRx =
        new(@"(?:Pa[a-z]{1,2}|art)\s*Number\s*[:\-\.]?\s*([A-Z0-9Ol\-_<>][A-Z0-9Ol\-_<>\s]{1,21})",
            RegexOptions.Compiled);

    // Fallback: "Part " immediately followed by a digit-like sequence (label "Number" dropped by OCR)
    private static readonly Regex _partFallbackRx =
        new(@"\bPar[a-z]\s+([9][0-9OolI\-<\s]{5,30})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Label-less fallback: part value appears with no label at all (e.g. after serial number).
    // Matches a standalone digit cluster that looks like "945-..." or 15 loosely-clustered digits.
    // Only used if both primary and secondary patterns fail.
    private static readonly Regex _partLabellessRx =
        new(@"(?<=\d{13}\s+)(9[0-9OolI\-<]{5,30})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Digits-only extractor used by the normaliser.
    private static readonly Regex _digitsRx = new(@"\d", RegexOptions.Compiled);

    // Canonical part-number format: DDD-DDDDD-DDDD-DDD  (3-5-4-3, total 15 digits).
    //
    // OCR substitution table applied BEFORE digit counting:
    //   O (letter)  → 0 (zero)
    //   l/I (ell/I) → 1
    //   < or >      → - (dash confused with angle bracket by WinRT)
    //
    // After substitution, if we have exactly 15 digits → reformat.
    // If 14 digits → try inserting '0' at positions 8–11 (the 4-digit group).
    //
    // Returns null if the result does not match \d{3}-\d{5}-\d{4}-\d{3} — caller
    // should treat a null as "no valid part number found" and suppress the match.
    private static readonly Regex _partCanonRx =
        new(@"^\d{3}-\d{5}-\d{4}-\d{3}$", RegexOptions.Compiled);

    private static string? NormalisePartValue(string raw)
    {
        // Step 1: OCR character substitutions
        var sb = new System.Text.StringBuilder(raw.Length);
        foreach (char c in raw)
        {
            sb.Append(c switch
            {
                'O' or 'o'           => '0',
                'l' or 'I'           => '1',
                '<' or '>'           => '-',
                _                    => c,
            });
        }
        string cleaned = sb.ToString();

        // Step 2: extract only digit characters
        var digits = string.Concat(_digitsRx.Matches(cleaned).Select(m => m.Value));

        string? result = null;

        if (digits.Length == 15)
        {
            result = $"{digits[..3]}-{digits[3..8]}-{digits[8..12]}-{digits[12..15]}";
        }
        else if (digits.Length == 14)
        {
            // Step 3: 14-digit fallback — the 4-digit group (positions 8-11) is most
            // often where a '0' gets swallowed.  Try inserting a '0' at index 8–11.
            for (int insertAt = 8; insertAt <= 11; insertAt++)
            {
                string candidate  = digits[..insertAt] + "0" + digits[insertAt..];
                string formatted  = $"{candidate[..3]}-{candidate[3..8]}-{candidate[8..12]}-{candidate[12..15]}";
                if (_partCanonRx.IsMatch(formatted))
                {
                    result = formatted;
                    break;
                }
            }
        }

        // Validate structural format — reject anything that isn't DDD-DDDDD-DDDD-DDD.
        // This prevents OCR misreads (e.g. '1'→'4') producing plausible-looking but
        // wrong values like "945-34500-0000-100".
        if (result == null || !_partCanonRx.IsMatch(result))
            return null;

        return result;
    }

    // ── Public entry point ───────────────────────────────────────────────────────
    /// <summary>
    /// Given the flat list of OCR words with their bounding rects, returns up to
    /// 3 named ROI matches (UPC / Serial Number / Part Number).
    /// </summary>
    public static List<RoiMatch> Extract(IReadOnlyList<WordEntry> words)
    {
        // Build a single normalised text string with word indices embedded so we
        // can map regex match positions back to individual WordEntry objects.
        // Format: "<idx>:<text> " repeated for every word.
        // We also maintain a parallel list of span → word-index for lookup.

        var fullText = string.Join(" ", words.Select(w => w.Text));
        var results  = new List<RoiMatch>(3);

        TryExtractSerial(fullText, words, results);
        TryExtractPart(fullText, words, results);
        TryExtractUpc(words, results);

        // Sort by appearance order (top-to-bottom, then left-to-right in image)
        results.Sort((a, b) =>
        {
            int dy = a.Bounds.Top - b.Bounds.Top;
            return dy != 0 ? dy : a.Bounds.Left - b.Bounds.Left;
        });

        // Assign ROI letters A/B/C in sorted order
        var roiLetters = new[] { "A", "B", "C" };
        for (int i = 0; i < results.Count && i < roiLetters.Length; i++)
        {
            results[i] = results[i] with
            {
                RoiLabel = $"ROI {roiLetters[i]} – {results[i].FieldName}"
            };
        }

        return results;
    }

    // ── Serial Number ─────────────────────────────────────────────────────────
    private static void TryExtractSerial(string fullText,
        IReadOnlyList<WordEntry> words, List<RoiMatch> results)
    {
        var m = _serialRx.Match(fullText);
        if (!m.Success) return;

        string value = m.Groups[1].Value.Trim();
        // Leading apostrophe/tick is a common OCR misread of '1' — replace it
        if (value.Length > 0 && (value[0] == '\'' || value[0] == '\u2018' || value[0] == '\u2019'))
            value = "1" + value[1..];
        var bounds = GetBoundsForSubstring(fullText, m.Index, m.Length, words);

        results.Add(new RoiMatch("", "Serial Number", value, bounds));
    }

    // ── Part Number ───────────────────────────────────────────────────────────
    private static void TryExtractPart(string fullText,
        IReadOnlyList<WordEntry> words, List<RoiMatch> results)
    {
        // Try primary pattern first (requires "Number" in label)
        var m = _partRx.Match(fullText);

        // If primary failed, try fallback (OCR dropped "Number" word entirely)
        if (!m.Success)
            m = _partFallbackRx.Match(fullText);

        // If both label patterns failed, try label-less: value follows serial digits directly
        if (!m.Success)
            m = _partLabellessRx.Match(fullText);

        if (!m.Success) return;

        // For label-less match, group 1 is the value capture; same for all patterns
        string rawValue = m.Groups[1].Value.Trim();
        string? value   = NormalisePartValue(rawValue);

        // Reject if normaliser couldn't produce a valid DDD-DDDDD-DDDD-DDD value —
        // better to report no match than a structurally wrong value.
        if (value == null) return;

        var bounds = GetBoundsForSubstring(fullText, m.Index, m.Length, words);

        results.Add(new RoiMatch("", "Part Number", value, bounds));
    }

    // ── UPC ───────────────────────────────────────────────────────────────────
    // Simply detect the word "UPC" in the OCR output and report its location.
    private static void TryExtractUpc(IReadOnlyList<WordEntry> words, List<RoiMatch> results)
    {
        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].Text.Equals("UPC", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(new RoiMatch("", "UPC", "UPC", words[i].Bounds));
                return;
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Finds the word entries whose text overlaps a regex match span in the
    /// joined string, and returns the union of their bounding boxes.
    /// </summary>
    private static Rectangle GetBoundsForSubstring(string fullText, int start, int length,
        IReadOnlyList<WordEntry> words)
    {
        var matched = GetWordsInSpan(fullText, start, length, words);
        if (matched.Count == 0) return Rectangle.Empty;
        return matched.Select(w => w.Bounds).Aggregate(Union);
    }

    /// <summary>
    /// Walks the joined fullText to identify which word indices fall within
    /// [start, start+length) and returns those WordEntry objects.
    /// </summary>
    private static List<WordEntry> GetWordsInSpan(string fullText, int start, int length,
        IReadOnlyList<WordEntry> words)
    {
        var result   = new List<WordEntry>();
        int pos      = 0;
        int end      = start + length;

        for (int i = 0; i < words.Count; i++)
        {
            int wordStart = pos;
            int wordEnd   = pos + words[i].Text.Length;

            // Overlap check
            if (wordEnd > start && wordStart < end)
                result.Add(words[i]);

            pos = wordEnd + 1; // +1 for the space separator
            if (pos > end) break;
        }

        return result;
    }

    private static Rectangle Union(Rectangle a, Rectangle b)
    {
        if (a == Rectangle.Empty) return b;
        if (b == Rectangle.Empty) return a;
        return Rectangle.Union(a, b);
    }

    // ── Corrected text builder ────────────────────────────────────────────────
    /// <summary>
    /// Returns <paramref name="rawText"/> with OCR-noise in ROI-matched spans
    /// replaced by their corrected/normalised values.
    ///
    /// Specifically:
    ///   • Serial Number value: leading tick/apostrophe → '1'
    ///   • Part Number value:   O→0, l/I→1, &lt;&gt;→-, reformatted to DDD-DDDDD-DDDD-DDD
    ///
    /// Non-ROI text is returned unchanged.
    /// </summary>
    public static string BuildCorrectedText(string rawText, IReadOnlyList<WordEntry> words)
    {
        string result = rawText;

        // Apply serial correction — fix leading tick on the captured value
        result = _serialRx.Replace(result, m =>
        {
            string captured  = m.Groups[1].Value;
            string corrected = captured;
            if (corrected.Length > 0 &&
                (corrected[0] == '\'' || corrected[0] == '\u2018' || corrected[0] == '\u2019'))
                corrected = "1" + corrected[1..];

            if (corrected == captured) return m.Value; // nothing changed
            int relStart = m.Groups[1].Index - m.Index;
            return string.Concat(m.Value.AsSpan(0, relStart), corrected);
        });

        // Apply part number correction via primary pattern
        bool primaryMatched = _partRx.IsMatch(rawText);
        if (primaryMatched)
        {
            result = _partRx.Replace(result, m =>
            {
                string  captured  = m.Groups[1].Value;
                string? corrected = NormalisePartValue(captured);
                if (corrected == null || corrected == captured) return m.Value;
                int relStart = m.Groups[1].Index - m.Index;
                return string.Concat(m.Value.AsSpan(0, relStart), corrected);
            });
        }
        else
        {
            result = _partFallbackRx.Replace(result, m =>
            {
                string  captured  = m.Groups[1].Value;
                string? corrected = NormalisePartValue(captured);
                if (corrected == null || corrected == captured) return m.Value;
                int relStart = m.Groups[1].Index - m.Index;
                return string.Concat(m.Value.AsSpan(0, relStart), corrected);
            });
        }

        return result;
    }
}

using System.Text.RegularExpressions;

namespace OcrTesseract;

/// <summary>
/// A single matched region of interest extracted from the OCR word list.
/// </summary>
/// <param name="RoiLabel">Display label, e.g. "ROI A – UPC"</param>
/// <param name="FieldName">Short field name shown as the card tag, e.g. "UPC"</param>
/// <param name="Value">Extracted value string</param>
/// <param name="Confidence">Mean word confidence 0–1 for the matched words</param>
/// <param name="Bounds">Bounding rectangle in image-pixel coordinates (union of matched words)</param>
record RoiMatch(
    string RoiLabel,
    string FieldName,
    string Value,
    float  Confidence,
    Rectangle Bounds);

/// <summary>
/// Word entry built from Tesseract's page iterator.
/// </summary>
record WordEntry(string Text, float Confidence, Rectangle Bounds);

static class RoiExtractor
{
    // ── Patterns ────────────────────────────────────────────────────────────────
    // ROI B: "Serial" "Number" ":" "<digits>"
    // Fuzzy: allow OCR misreads of "Serial" (e.g. "Sonal", "Senial", "Seria1")
    private static readonly Regex _serialRx =
        new(@"S\w{2,5}l\s+Number\s*[:\-\.]?\s*(\d[\d\-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // ROI C: "Part" "Number" ":" "<alphanumeric-dash-underscore>"
    private static readonly Regex _partRx =
        new(@"P\w{0,3}t\s+Number\s*[:\-\.]?\s*([A-Z0-9][A-Z0-9\-_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
        var    bounds = GetBoundsForSubstring(fullText, m.Index, m.Length, words);
        float  conf   = GetMeanConfForSubstring(fullText, m.Index, m.Length, words);

        results.Add(new RoiMatch("", "Serial Number", value, conf, bounds));
    }

    // ── Part Number ───────────────────────────────────────────────────────────
    private static void TryExtractPart(string fullText,
        IReadOnlyList<WordEntry> words, List<RoiMatch> results)
    {
        var m = _partRx.Match(fullText);
        if (!m.Success) return;

        string value = m.Groups[1].Value.Trim();
        var    bounds = GetBoundsForSubstring(fullText, m.Index, m.Length, words);
        float  conf   = GetMeanConfForSubstring(fullText, m.Index, m.Length, words);

        results.Add(new RoiMatch("", "Part Number", value, conf, bounds));
    }

    // ── UPC ───────────────────────────────────────────────────────────────────
    // Simply detect the word "UPC" in the OCR output and report its location.
    private static void TryExtractUpc(IReadOnlyList<WordEntry> words, List<RoiMatch> results)
    {
        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].Text.Equals("UPC", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(new RoiMatch("", "UPC", "UPC", words[i].Confidence, words[i].Bounds));
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

    private static float GetMeanConfForSubstring(string fullText, int start, int length,
        IReadOnlyList<WordEntry> words)
    {
        var matched = GetWordsInSpan(fullText, start, length, words);
        if (matched.Count == 0) return 0f;
        return matched.Average(w => w.Confidence);
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
}

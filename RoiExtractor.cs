using System.Text.RegularExpressions;

namespace OcrPro;

record RoiMatch(
    string RoiLabel,
    string FieldName,
    string Value,
    Rectangle Bounds);

record WordEntry(string Text, Rectangle Bounds);

static class RoiExtractor
{
    private static readonly Regex _serialRx =
        new(@"S[\w,]{1,5}l\s+Number\s*[:\-\.]?\s*(['\u2018\u2019]?\d[\d\-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex _partRx =
        new(@"(?:Pa[a-z]{1,2}|art)\s*Number\s*[:\-\.]?\s*([A-Z0-9Ol\-_<>][A-Z0-9Ol\-_<>\s]{1,21})",
            RegexOptions.Compiled);

    private static readonly Regex _partFallbackRx =
        new(@"\bPar[a-z]\s+([9][0-9OolI\-<\s]{5,30})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex _partLabellessRx =
        new(@"(?<=\d{13}\s+)(9[0-9OolI\-<]{5,30})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex _digitsRx = new(@"\d", RegexOptions.Compiled);

    private static readonly Regex _partCanonRx =
        new(@"^\d{3}-\d{5}-\d{4}-\d{3}$", RegexOptions.Compiled);

    private static string? NormalisePartValue(string raw)
    {
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

        var digits = string.Concat(_digitsRx.Matches(cleaned).Select(m => m.Value));

        string? result = null;

        if (digits.Length == 15)
        {
            result = $"{digits[..3]}-{digits[3..8]}-{digits[8..12]}-{digits[12..15]}";
        }
        else if (digits.Length == 14)
        {
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

        if (result == null || !_partCanonRx.IsMatch(result))
            return null;

        return result;
    }

    public static List<RoiMatch> Extract(IReadOnlyList<WordEntry> words)
    {
        var fullText = string.Join(" ", words.Select(w => w.Text));
        var results  = new List<RoiMatch>(3);

        TryExtractSerial(fullText, words, results);
        TryExtractPart(fullText, words, results);
        TryExtractUpc(words, results);

        results.Sort((a, b) =>
        {
            int dy = a.Bounds.Top - b.Bounds.Top;
            return dy != 0 ? dy : a.Bounds.Left - b.Bounds.Left;
        });

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

    private static void TryExtractSerial(string fullText,
        IReadOnlyList<WordEntry> words, List<RoiMatch> results)
    {
        var m = _serialRx.Match(fullText);
        if (!m.Success) return;

        string value = m.Groups[1].Value.Trim();
        if (value.Length > 0 && (value[0] == '\'' || value[0] == '\u2018' || value[0] == '\u2019'))
            value = "1" + value[1..];
        var bounds = GetBoundsForSubstring(fullText, m.Index, m.Length, words);

        results.Add(new RoiMatch("", "Serial Number", value, bounds));
    }

    private static void TryExtractPart(string fullText,
        IReadOnlyList<WordEntry> words, List<RoiMatch> results)
    {
        var m = _partRx.Match(fullText);

        if (!m.Success)
            m = _partFallbackRx.Match(fullText);

        if (!m.Success)
            m = _partLabellessRx.Match(fullText);

        if (!m.Success) return;

        string rawValue = m.Groups[1].Value.Trim();
        string? value   = NormalisePartValue(rawValue);

        if (value == null) return;

        var bounds = GetBoundsForSubstring(fullText, m.Index, m.Length, words);

        results.Add(new RoiMatch("", "Part Number", value, bounds));
    }

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

    private static Rectangle GetBoundsForSubstring(string fullText, int start, int length,
        IReadOnlyList<WordEntry> words)
    {
        var matched = GetWordsInSpan(fullText, start, length, words);
        if (matched.Count == 0) return Rectangle.Empty;
        return matched.Select(w => w.Bounds).Aggregate(Union);
    }

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

            if (wordEnd > start && wordStart < end)
                result.Add(words[i]);

            pos = wordEnd + 1;
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

    private static readonly Regex _senalRx =
        new(@"\bSenal\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string BuildCorrectedText(string rawText, IReadOnlyList<WordEntry> words)
    {
        string result = rawText;

        result = _senalRx.Replace(result, m =>
            char.IsUpper(m.Value[0]) ? "Serial" : "serial");

        result = _serialRx.Replace(result, m =>
        {
            string captured  = m.Groups[1].Value;
            string corrected = captured;
            if (corrected.Length > 0 &&
                (corrected[0] == '\'' || corrected[0] == '\u2018' || corrected[0] == '\u2019'))
                corrected = "1" + corrected[1..];

            if (corrected == captured) return m.Value;
            int relStart = m.Groups[1].Index - m.Index;
            return string.Concat(m.Value.AsSpan(0, relStart), corrected);
        });

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

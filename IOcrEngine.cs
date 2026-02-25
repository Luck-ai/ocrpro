namespace OcrTesseract;

/// <summary>
/// Which OCR backend to use. Listed fastest-first.
/// </summary>
enum OcrEngineType
{
    /// <summary>
    /// Windows.Media.Ocr — built-in Windows WinRT engine (~5–20 ms).
    /// Requires Windows 10 1803+ with the English language pack installed.
    /// </summary>
    WinRt,

    /// <summary>
    /// PaddleOCR v4 (English, CPU MKLDNN) — ~200–500 ms after warm-up.
    /// Models bundled via Sdcb.PaddleOCR.Models.Local; no internet needed.
    /// </summary>
    PaddleOcr,

    /// <summary>
    /// Tesseract 5 — vanilla LSTM engine (~700 ms). Authoritative fallback
    /// and supports custom tessdata for additional languages.
    /// </summary>
    Tesseract,
}

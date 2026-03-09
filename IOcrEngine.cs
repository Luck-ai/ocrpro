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

    /// <summary>
    /// RapidOCR — PP-OCRv4 mobile ONNX models via OnnxRuntime + DirectML.
    /// Target: 5–20 ms on DirectML-capable GPU; falls back to CPU.
    /// Requires models in models/ppocr/ (det, cls, rec ONNX + keys file).
    /// </summary>
    RapidOcr,

    /// <summary>
    /// Tesseract 5 Fast — integer-quantised LSTM models from tessdata_fast.
    /// ~2–4x faster than standard tessdata at a small accuracy cost.
    /// Requires tessdata_fast/eng.traineddata next to the executable.
    /// </summary>
    TesseractFast,

    /// <summary>
    /// Tesseract 5 Best — highest-accuracy LSTM models from tessdata_best.
    /// Requires tessdata_best/eng.traineddata next to the executable.
    /// </summary>
    TesseractBest,

    /// <summary>
    /// PP-OCRv5 Mobile (Chinese unified model handles English).
    /// Downloaded on first use via Sdcb.PaddleOCR.Models.Online.
    /// </summary>
    PaddleOcrV5Mobile,

    /// <summary>
    /// PP-OCRv5 Server — highest-accuracy PaddleOCR models.
    /// Downloaded on first use via Sdcb.PaddleOCR.Models.Online.
    /// </summary>
    PaddleOcrV5Server,
}

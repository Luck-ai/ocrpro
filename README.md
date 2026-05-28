<div align="center">

# 🔎 OcrPro

**A Windows desktop OCR tool that pairs the built-in Windows.Media.Ocr engine with OpenCV preprocessing and ROI/field extraction.**

[![Language](https://img.shields.io/badge/C%23-.NET%2010-512BD4?style=for-the-badge&labelColor=0E1626)](https://dotnet.microsoft.com/)
[![UI](https://img.shields.io/badge/Windows%20Forms-Desktop-22D3EE?style=for-the-badge&labelColor=0E1626)](https://learn.microsoft.com/dotnet/desktop/winforms/)
[![OCR](https://img.shields.io/badge/Windows.Media.Ocr-WinRT-5A8DFF?style=for-the-badge&labelColor=0E1626)](https://learn.microsoft.com/uwp/api/windows.media.ocr)
[![CV](https://img.shields.io/badge/Emgu.CV-4.12-34D399?style=for-the-badge&labelColor=0E1626)](https://www.emgu.com/)
[![Platform](https://img.shields.io/badge/Windows-10.0.19041%2B-8A2BE2?style=for-the-badge&labelColor=0E1626)](#-build-from-source)

</div>

---

## 🎯 What is it

**OcrPro** is a small Windows Forms app for reading text out of images. It runs each frame through an OpenCV preprocessing pipeline (grayscale → CLAHE → unsharp mask), then hands it to the WinRT `Windows.Media.Ocr` engine, and post-processes the words into structured **ROI matches** (e.g. *Serial Number*, *Part Number*) via regex.

Targets `net10.0-windows10.0.19041.0` and uses Windows Forms for the UI.

---

## ✨ Features

### 🧪 Preprocessing pipeline (OpenCV / Emgu.CV)
- **Grayscale** conversion
- **CLAHE** contrast equalisation (default clip 2.0, 8×8 tiles)
- **Unsharp-mask** sharpening (configurable sigma / strength)

### 🅰 OCR
- Wraps `Windows.Media.Ocr.OcrEngine` with a per-language cache
- **Warm-up on startup** so the first real run isn't cold
- Two-pass rotation handling

### 🔖 ROI / field extraction
Regex-driven extractor pulls structured fields out of free OCR text. Built-in matchers include:

- **Serial Number** (with OCR-error tolerant patterns)
- **Part Number** (multiple fallbacks + canonical `###-#####-####-###` validator)

### 🪟 UI
- Dark Windows Forms shell with custom flow panels
- Per-ROI accent colours overlaid on the image
- Live raw-OCR text panel + structured results card
- Crash logs written to `%LocalAppData%/OcrPro/crash.txt`

---

## 🛠 Build from source

### Prerequisites

| Requirement                            | Notes                                                |
|----------------------------------------|------------------------------------------------------|
| 🪟 **Windows 10 build 19041+**         | Required for the WinRT `Windows.Media.Ocr` API       |
| 🧰 **.NET 10 SDK (windows workload)**  | `TargetFramework=net10.0-windows10.0.19041.0`        |
| 🎨 **Windows Forms**                   | Enabled via `UseWindowsForms=true`                   |

### Build & run

```powershell
git clone https://github.com/Luck-ai/WinRT-OCR
cd WinRT-OCR
dotnet run --project OcrPro.csproj
```

Or open `OcrPro.sln` in Visual Studio and hit **F5**.

### Package references

```xml
<PackageReference Include="Emgu.CV"                 Version="4.12.0.5764" />
<PackageReference Include="Emgu.CV.Bitmap"          Version="4.12.0.5764" />
<PackageReference Include="Emgu.CV.runtime.windows" Version="4.12.0.5764" />
```

---

## 🗂 Project structure

```text
WinRT-OCR/
├── OcrPro.sln
├── OcrPro.csproj
├── Program.cs              # WinForms bootstrap + global crash handler
├── Form1.cs                # main UI, image loading, ROI rendering
├── Form1.Designer.cs       # designer-generated layout
├── Form1.resx
├── WinRtOcrEngine.cs       # Windows.Media.Ocr wrapper + warm-up + two-pass
├── ImagePreprocessor.cs    # OpenCV pipeline: Grayscale → CLAHE → Sharpen
└── RoiExtractor.cs         # regex-based field extractor (Serial / Part #)
```

---

## 🆘 Troubleshooting

| Symptom                                                  | Fix                                                                                                         |
|----------------------------------------------------------|-------------------------------------------------------------------------------------------------------------|
| `Windows.Media.Ocr` types not found                      | Make sure your TFM still targets `windows10.0.19041.0` or newer — the WinRT projection comes from the SDK.  |
| OCR returns nothing on the first run                     | The engine warms up in the background on `Shown`; very fast first runs may race it. Run again.              |
| App crashes silently                                     | Check `%LocalAppData%\OcrPro\crash.txt` — both `ThreadException` and `UnhandledException` are logged there. |
| Need a language other than the OS default                | `WinRtOcrEngine.RunAsync(bmp, languageTag: "en-US")` — pass any BCP-47 tag installed on the machine.        |

---

<div align="center">

**WinRT-OCR / OcrPro** · [GitHub](https://github.com/Luck-ai/WinRT-OCR) · [Issues](https://github.com/Luck-ai/WinRT-OCR/issues)

</div>

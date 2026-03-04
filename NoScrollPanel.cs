namespace OcrTesseract;

/// <summary>
/// A Panel that hides its native scrollbar chrome while keeping AutoScroll
/// scroll-position logic and mouse-wheel scrolling fully functional.
/// The scrollbar is removed at the Win32 window-style level, so Windows
/// never allocates space for it — no flickering, no extra padding.
/// </summary>
internal class NoScrollPanel : Panel
{
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.Style &= ~0x00200000; // WS_VSCROLL
            cp.Style &= ~0x00100000; // WS_HSCROLL
            return cp;
        }
    }
}

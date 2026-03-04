using System.Runtime.InteropServices;

namespace OcrTesseract;

static class Program
{
    [DllImport("kernel32.dll")] static extern bool AttachConsole(int pid);

    [STAThread]
    static void Main()
    {
        AttachConsole(-1);   // -1 = attach to parent process console (cmd / PowerShell)

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
using System.Runtime.InteropServices;

namespace OcrTesseract;

static class Program
{
    [DllImport("kernel32.dll")] static extern bool AttachConsole(int pid);

    [STAThread]
    static async Task Main(string[] args)
    {
        AttachConsole(-1);   // -1 = attach to parent process console (cmd / PowerShell)

        if (args.Length > 0 && args[0] == "--diag")
        {
            ApplicationConfiguration.Initialize();
            try { await DiagDump.RunAsync(); }
            catch (Exception ex) { File.WriteAllText(@"D:\ocr_tesserack\ocr_diag_error.txt", ex.ToString()); }
            return;
        }

        if (args.Length > 0 && args[0] == "--sweep")
        {
            ApplicationConfiguration.Initialize();
            try { await PrepSweep.RunAsync(); }
            catch (Exception ex) { File.WriteAllText(@"D:\ocr_tesserack\prep_sweep_error.txt", ex.ToString()); }
            return;
        }

        if (args.Length > 0 && args[0] == "--benchmark")
        {
            ApplicationConfiguration.Initialize();
            var progress = new Progress<string>(msg => Console.WriteLine(msg));
            try
            {
                string path = await OcrBenchmark.RunBenchmarkAsync(progress);
                Console.WriteLine($"\nBenchmark complete! Report: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Benchmark failed: {ex}");
            }
            return;
        }

        string crashLog = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OcrTesseract", "crash.txt");
        Application.ThreadException += (_, args) =>
        {
            string msg = args.Exception?.ToString() ?? "(null)";
            try { Directory.CreateDirectory(Path.GetDirectoryName(crashLog)!); File.WriteAllText(crashLog, msg); } catch { }
            MessageBox.Show($"Unhandled exception:\n\n{args.Exception?.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            string msg = args.ExceptionObject?.ToString() ?? "(null)";
            try { Directory.CreateDirectory(Path.GetDirectoryName(crashLog)!); File.AppendAllText(crashLog, "\n\n[Domain]\n" + msg); } catch { }
        };

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
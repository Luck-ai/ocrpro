namespace OcrPro;

static class Program
{
    [STAThread]
    static void Main(string[] _)
    {
        Application.ThreadException += (_, args) =>
        {
            string crashLog = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "OcrPro", "crash.txt");
            string msg = args.Exception?.ToString() ?? "(null)";
            try { Directory.CreateDirectory(Path.GetDirectoryName(crashLog)!); File.WriteAllText(crashLog, msg); } catch { }
            MessageBox.Show($"Unhandled exception:\n\n{args.Exception?.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            string crashLog = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "OcrPro", "crash.txt");
            string msg = args.ExceptionObject?.ToString() ?? "(null)";
            try { Directory.CreateDirectory(Path.GetDirectoryName(crashLog)!); File.AppendAllText(crashLog, "\n\n[Domain]\n" + msg); } catch { }
        };

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}

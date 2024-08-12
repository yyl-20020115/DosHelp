using System;
using System.Text;
using System.Windows.Forms;

namespace HelpBrowser;

public static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}

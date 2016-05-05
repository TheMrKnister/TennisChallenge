using System;
using System.Diagnostics;
using System.Windows.Forms;
using Inspectron.CodeCollection.ApplicationBasics;

namespace TennisChallenge.InfoBoard
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Run(() =>
            {
              using (var model = new Model())
              {
                  try
                  {
                      model.Connect();
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show("Error connecting to RFID device", "Connection Error", MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
                  }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain(model));
              }
            });
    }

    private static void Run(Action action)
    {
#if DEBUG
      AppEnvironmentBase.Initialize(typeof(Program).Assembly, false);
#else
      AppEnvironmentBase.Initialize(typeof(Program).Assembly, true);
#endif
      LoggingBase.Initialize(true);

      try
      {
        action();
      }
#if DEBUG
#else
      catch (Exception e)
      {
        if (Logging.LogLevel.TraceError)
          Trace.WriteLine(e.ToString());

        MessageBox.Show("Unerwarteter Fehler: " + e.Message);
      }
#endif
      finally
      {
        Trace.WriteLine("Anwendung beendet.");

        LoggingBase.Dispose();
      }
    }
  }
}

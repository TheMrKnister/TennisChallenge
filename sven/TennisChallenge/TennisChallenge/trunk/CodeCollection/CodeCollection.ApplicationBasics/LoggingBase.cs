using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Inspectron.CodeCollection.ApplicationBasics
{
    /// <summary>
    /// Stellt Hilfsfunktionen zum Logging bereit. Damit die Logging-Funktionalität verwendet werden kann, muss 
    /// zuerst einmal Initialize() aufgerufen werden.
    /// </summary>
    /// <remarks>
    /// <para>In der Anwendung kann diese Klasse abgeleitet werden. In der abgeleiteten Klasse können TraceSwitches 
    /// für die Anwendung instantiiert werden.</para>
    /// <para>Code-Beispiel:</para>
    /// <code>
    /// public class Logging : LoggingBase
    /// {
    ///
    ///    public static TraceSwitch LogLevel = new TraceSwitch("TraceLevelSwitch", "Global Trace-Level");
    ///
    /// }
    /// 
    /// static class Program
    /// {
    ///    static void Main()
    ///    {
    ///       AppEnvironment.Initialize(typeof(Program).Assembly, true);
    ///       Logging.Initialize(true);
    /// 
    ///       try
    ///       {
    ///          if(Logging.LogLevel.TraceVerbose)
    ///             System.Diagnostics.Trace.WriteLine("Beispiel für einen Logeintrag.");
    /// 
    ///          Application.EnableVisualStyles();
    ///          Application.SetCompatibleTextRenderingDefault(false);
    ///          Application.Run(new FrmMain());
    ///       }
    ///       finally
    ///       {
    ///          Logging.Dispose()
    ///       }
    ///    }
    /// }
    /// </code>
    /// <para>Im Code-Beispiel, kann mit folgendem Eintrag in der Datei app.config der Zustand des 
    /// Trace-Switches bestimmt werden:</para>
    /// <code>
    /// <system.diagnostics>
    ///   <switches>
    ///     <add name="TraceLevelSwitch" value="3" />
    ///   </switches>
    /// </system.diagnostics>
    /// </code>
    /// </remarks>
    public class LoggingBase
    {

        /// <summary>
        /// Richtet einen Trace-Listener ein, welcher alle Log-Einträge, welche mit 
        /// System.Diagnostics.Trace.Write...() vorgenommen werden, in eine Logdatei schreibt.
        /// </summary>
        /// <remarks>
        /// <para>Bei diesem Konstruktor wird als Pfad für die Logdateien 
        /// AppEnvironmentBase.AppDataPath"\log\yy-MM-dd.txt" gewählt. Daher muss, bevor das Logging 
        /// initialisiert wird, AppEnvironmentBase initialisiert werden. 
        /// </para>
        /// <para>Damit die Logdatei ordungsgemäss geschlossen wird, sollte vor dem Schliessen der Anwendung 
        /// Dispose() ausgeführt werden.
        /// </para>
        /// </remarks>
        /// <param name="logApplicationStart">Falls <b>true</b>, wird nach dem Einrichten der Logdatei gleich 
        /// der Eintrag "Starte Anwendung (Version)" eingetragen.</param>
        public static void Initialize(bool logApplicationStart)
        {
            if (!AppEnvironmentBase.Initialized)
                throw new Exception("AppEnvironmentBase muss initialisiert werden, " + 
                    "bevor LoggingBase initialisiert wird.");

            Initialize(Path.Combine(AppEnvironmentBase.AppDataPath, "log"), logApplicationStart);
        }

        /// <summary>
        /// Richtet einen Trace-Listener ein, welcher alle Log-Einträge, welche mit 
        /// System.Diagnostics.Trace.Write...() vorgenommen werden, in eine Logdatei schreibt.
        /// </summary>
        /// <remarks>
        /// <para>Damit die Logdatei ordungsgemäss geschlossen wird, sollte vor dem Schliessen der Anwendung 
        /// Dispose() ausgeführt werden.
        /// </para>
        /// </remarks>
        /// <param name="logDirectory">Verzeichnis, in welchem die Log-Datei erstellt wird. Falls das Verzeichnis 
        /// nicht existiert, wird es automatisch neu angelegt.</param>
        /// <param name="logApplicationStart">Falls <b>true</b>, wird nach dem Einrichten der Logdatei gleich 
        /// der Eintrag "Starte Anwendung (Version)" eingetragen.</param>
        public static void Initialize(string logDirectory, bool logApplicationStart)
        {
            if (!_initialized)
            {
                _initialized = true;

                string logfile = CheckLogDirectory(logDirectory) + DateTime.Now.ToString("yy-MM-dd") + ".txt";

                FileStream myTraceLog = new FileStream(logfile, FileMode.Append, FileAccess.Write);
                TextWriterTraceListenerWithTime myListener = new TextWriterTraceListenerWithTime(myTraceLog);
                System.Diagnostics.Trace.AutoFlush = true;
                System.Diagnostics.Trace.Listeners.Add(myListener);
            }

            if (logApplicationStart)
            {
                string appStart = " Starte Anwendung (" + AppEnvironmentBase.AssemblyVersion + ")";

                Trace.WriteLine(new String('*', appStart.Length + 1) + Environment.NewLine + appStart +
                    Environment.NewLine + new String('*', appStart.Length + 1));
            }
        }

        ~LoggingBase()
        {
            Dispose();
        }

        public static void LogStopWatchFrequency()
        {
            double res = 1.0e9 / Stopwatch.Frequency;

            Trace.WriteLine(String.Format("Stopwatch.Frequency = {0} => Zeitauflösung = {1:0.0##} ns",
                Stopwatch.Frequency, res));
        }

        /// <summary>
        /// Schliesst die Log-Datei und sollte vor dem Beenden des Programms aufgerufen werden.
        /// </summary>
        public static void Dispose()
        {
            if (_traceListener != null)
            {
                _traceListener.Close();
                _traceListener.Dispose();
                _traceListener = null;
            }

            if (_traceLogFile != null)
            {
                _traceLogFile.Close();
                _traceLogFile.Dispose();
                _traceLogFile = null;
            }
        }

        #region Private

        /// <summary>
        /// Prüft, ob das Log-Verzeichnis existiert. Falls nicht, wird versucht, das Verzeichnis anzulegen.
        /// </summary>
        /// <remarks>
        /// Es wird standardmässig das Verzeichnis AppEnvironmentBase.AppDataPath\log  verwendet.
        /// </remarks>
        /// <returns>Gibt den Pfad des Log-Verzeichnis zurück.</returns>
        static string CheckLogDirectory(string logDirectory)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            return logDirectory + "\\";
        }

        private static bool _initialized = false;

        private static FileStream _traceLogFile = null;

        private static TextWriterTraceListenerWithTime _traceListener = null;

        #endregion // Private

    }
}

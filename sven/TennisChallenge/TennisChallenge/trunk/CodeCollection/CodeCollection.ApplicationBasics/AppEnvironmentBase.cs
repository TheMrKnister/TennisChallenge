using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;



/*! \namespace Inspectron::CodeCollection::ApplicationBasics
 * \brief Enhält einige Hilfsklassen, welche bereits in verschiedenen Anwendungen verwendet wurden.
 * 
 * Folgende Punkte wurden bereits in den Projekten Gurit und Otto Fuchs ähnlich gelöst. Daher wurde der dazu
 * wesentliche Code in diese Bibliothek ausgelagert.
 * 
 * - Für ein einfaches Logging wurden die Trace Möglickeiten des .Net Frameworks verwendet. D.h. Logging Ausgaben
 *   werden in der Software einfach mit System.Diagnostics.Trace.WriteLine() ausgegeben. Diese Einträge werden
 *   dann in einer Log-Datei festgehalten. Der Code zum registrieren des dazu notwendigen TraceListeners steht
 *   in LoggingBase.
 * - Die Log-Dateien und sonstige anwendungsspezifische Information wurden immer in einen AppData - Ordner 
 *   gespeichert. Definition dieses Ordners, siehe AppEnvironmentBase.AppDataPath.
*/
namespace Inspectron.CodeCollection.ApplicationBasics
{
    /// <summary>
    /// Die Klasse enthält Funktionen, welche für die ganze Anwendung von Interesse sein könnten.
    /// </summary>
    /// <remarks>
    /// <para>Aktuell sind folgende Funktionen verfügbar:
    /// - Der AppDataPath: Definiert eine Pfad für anwendungsspezifische Informationen. 
    /// - AssemblyVersion: Hilfe, um direkten Zugriff auf einen String mit der Assembly-Version der Anwendung
    ///   zu haben.
    /// - Alle unbehandelten Exceptions der Anwendung können mit einem Standard-Handler behandelt werden. (Wird 
    ///   in Initialize() über den 2. Parameter ausgewählt.
    /// </para>
    /// <para>Die Information basiert auf den Infos zum Assembly der Hauptanwendung. Deshalb muss, bevor die 
    /// Klasse verwendet werden kann, Initialize() mit einer Referenz auf das Hauptassembly der Anwendung 
    /// aufgerufen werden.</para>
    /// <para>Vorschlag zur Verwendung:</para>
    /// <code>
    /// static class Program
    /// {
    ///    static void Main()
    ///    {
    ///       AppEnvironment.Initialize(typeof(Program).Assembly, true);
    /// 
    ///       Application.EnableVisualStyles();
    ///       Application.SetCompatibleTextRenderingDefault(false);
    ///       Application.Run(new FrmMain());
    ///    }
    /// }
    /// </code>
    /// </remarks>
    public class AppEnvironmentBase
    {
        /// <summary>
        /// In einem Debug-Build Initialize(appAssembly, false). Im Release-Build Initialize(appAssembly, true)
        /// </summary>
        /// <param name="appAssembly">Assembly-Objekt des Anwendungs-Assemblys.</param>
        public static void Initialize(Assembly appAssembly)
        {
#if DEBUG
          Initialize(appAssembly, false);
#else
          Initialize(appAssembly, true);
#endif
        }
        /// <summary>
        /// Übernimmt das Assembly der Anwendung.
        /// </summary>
        /// <param name="appAssembly">Assembly-Objekt des Anwendungs-Assemblys.</param>
        /// <param name="unhandledException"><b>True</b>, falls alle in der Anwendung nicht behandelten
        /// Exceptions in einem Standard Exception-Handler behandelt werden sollen.</param>
        public static void Initialize(Assembly appAssembly, bool unhandledException)
        {
            if (appAssembly == null)
                throw new ArgumentNullException();

            _appAssembly = appAssembly;

            if(unhandledException)
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// <b>True</b>, falls Initialize() korrekt ausgeführt wurde.
        /// </summary>
        public static bool Initialized
        {
            get
            {
                return (_appAssembly != null);
            }
        }

        /// <summary>
        /// Liefert den Speicherpfad für die Anwendungseinstellungen. Der Pfad wird zusammengesetzt aus 
        /// SpecialFolder.ApplicationData und der Firmen- und Produktinformation des Assembly: 
        /// "ApplicationData\Firma\Produkt\"
        /// </summary>
        /// <remarks>
        /// Auf einem deutschen Windows XP ergibt dies beispielsweise den folgenden Pfad:
        /// <code>C:\Dokumente und Einstellungen\Benutzername\Anwendungsdaten\Firma\Produkt</code>
        /// </remarks>
        public static string AppDataPath
        {
            get
            {
                if (_appDataPath == null)
                    DefineAppDataPath();

                return _appDataPath;
            }
        }

        /// <summary>
        /// Liefert einen String der Assembly-Version.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                if (Initialized)
                {
                    return _appAssembly.GetName().Version.ToString();
                }
                else
                {
                    throw new AppEnvironmentException("AppEnvironment wurde nicht korrekt initialisiert.");
                }
            }
        }

        /// <summary>
        /// Wird aufgerufen, falls ein Fehler aufgetreten ist und die Anwendung beendet werden muss.
        /// </summary>
        /// <remarks>
        /// Die Methode hat 3 Aufgaben:
        /// - Macht für den Fehler einen Eintrag in die TraceListener.
        /// - Informiert den Anwender mit einer MessageBox über den Fehler.
        /// - Beendet die Anwendung.
        /// <para>Der Logeintrag hat die Form <i>source</i> + ": Unhandled Exception, " + <i>exToString</i>.</para>
        /// </remarks>
        /// <param name="source">Text, welcher den Ort des Fehlers beschreibt. Wird für den Logeintrag verwendet.</param>
        /// <param name="message">Fehlertext, welcher in der MessageBox dem Anwender angezeigt wird.</param>
        /// <param name="exToString">Ausführliche Fehlerinformation, wird in Logdatei eingetragen. Es kann z.B.
        /// Exception.ToString() verwendet werden.</param>
        public static void LogExceptionTerminate(string source, string message, string exToString)
        {
            System.Diagnostics.Trace.WriteLine(source + ": Unhandled Exception, " + Environment.NewLine + exToString);

            if (string.IsNullOrEmpty(message))
            {
                int newLine = exToString.IndexOf(Environment.NewLine);
                if (newLine > 0)
                    message = exToString.Substring(0, newLine);
            }

            System.Windows.Forms.MessageBox.Show("Unbehandelte Ausnahme, die Anwendung muss beendet werden:" +
                Environment.NewLine + Environment.NewLine + message, "Fehler",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

            if(System.Windows.Forms.Application.MessageLoop)
                System.Windows.Forms.Application.Exit();
            Environment.Exit(1);
        }

        #region Private

        /// <summary>
        /// Erstellt den Speicherpfad für die Anwendungseinstellungen. Siehe auch #AppDataPath. Prüft, ob das 
        /// Verzeichnis vorhanden ist. Falls nicht, wird das Verzeichnis erstellt.
        /// </summary>
        /// <exception cref="AppEnvironmentException">Bei einem Fehler während der Erstellung des Pfades.</exception>
        private static void DefineAppDataPath()
        {
            if (!Initialized)
                throw new AppEnvironmentException("AppEnvironment wurde nicht korrekt initialisiert.");

            string companyName;
            AssemblyCompanyAttribute[] ascats =
                (AssemblyCompanyAttribute[])_appAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if (ascats.Length > 0)
            {
                AssemblyCompanyAttribute aca = ascats[0];
                companyName = aca.Company;
            }
            else
            {
                companyName = "Inspectron AG";
            }

            string productName = null;
            AssemblyProductAttribute[] aspproats =
                (AssemblyProductAttribute[])_appAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if (aspproats.Length > 0)
            {
                AssemblyProductAttribute apa = aspproats[0];
                productName = apa.Product;
            }

            if ((productName == null) || (productName.Length < 1))
            {
                throw new AppEnvironmentException("Der Entwickler muss für das Assembly einen Produktnamen definieren.");
            }

            _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" +
                companyName + "\\" + productName + "\\";

            if (!Directory.Exists(_appDataPath))
                Directory.CreateDirectory(_appDataPath);

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogExceptionTerminate(sender.ToString(), "", e.ExceptionObject.ToString());
        }


        private static string _appDataPath = null;

        private static Assembly _appAssembly = null;

        #endregion // Private
    }
}

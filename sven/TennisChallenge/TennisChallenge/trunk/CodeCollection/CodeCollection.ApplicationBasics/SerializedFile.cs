using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Inspectron.CodeCollection.ApplicationBasics
{
    /// <summary>
    /// Hilfsklasse, zu XmlSerializer des .Net-Frameworks. Soll die Speicherung von Xml-Dateien sicherer machen.
    /// </summary>
    /// <remarks>
    /// <para>Soll eine bestehende Xml-Datei mit einer aktuelleren Überschrieben werden, so ist es möglich dass, wenn 
    /// die Datei direkt überschrieben wird, bei einem Softwareabsturz in einem schlechten Moment keine konsistente 
    /// Datei mehr existiert. Diese Klasse bietet Methoden, welche dem Problem auf folgende Weise zu versuchen 
    /// zu vermeiden:</para>
    /// <para>Beim Schreibvorgang, wird die bestehende Datei in eine temporäre Datei verschoben, die neue geschrieben 
    /// und anschliessend die temporäre Datei gelöscht. Beim Lesevorgang, kann dann, falls die Datei nicht gelesen 
    /// werden kann, versucht werden die alte Datei aus der temporären Datei wieder herzustellen. Dazu müssen 
    /// SaveReplaceFile() und SaveReadFromFile<T>() verwendet werden.</para>
    /// </remarks>
    public static class SerializedFile
    {
        /// <summary>
        /// Speichert das Objekt mit Hilfe des XmlSerializers in die angegebene Datei. Existiert die Datei bereits, 
        /// so wird die bestehende Datei zuerst in eine temporäre Datei verschoben und erst gelöscht, wenn die neue 
        /// Datei geschrieben wurde.
        /// </summary>
        /// <param name="obj">Objekt, welches serialisiert werden soll. Die Serialisierung geschieht mit Hilfe des 
        /// .Net-XmlSerializers und muss daher dessen Bedingungen erfüllen, damit die Serialisierung erfolgreich 
        /// durchgeführt werden kann.</param>
        /// <param name="filepath">Dateiname der Datei, in welche das Objekt serialisiert wird.</param>
        /// <returns>
        /// <b>True</b>, falls eine bestehende Datei ersetzt wurde.
        /// </returns>
        public static bool SaveReplaceFile(object obj, string filepath)
        {
            bool fileExists = File.Exists(filepath);
            string backupFile = null;

            // Falls bereits eine Datei mit diesem Namen existiert, wird diese temporär verschoben und erst
            // nachdem die das Objekt erfolgreich gespeichert wurde gelöscht.
            if (fileExists)
            {
                backupFile = GetBackupFilePath(filepath);

                if (File.Exists(backupFile))
                    throw new SerializedFileBackupException("Saving of " + filepath +
                        "failed. The backup file " + backupFile + " already exists.");

                // Verschiebe die Datei
                File.Move(filepath, backupFile);
            }

            // Speichere das Objekt
            Write2File(obj, filepath);

            // Lösche falls notwendig die alte Datei.
            if (fileExists)
            {
                File.Delete(backupFile);
            }

            return fileExists;
        }

        /// <summary>
        /// Lädt ein Objekt aus der angegebenen Datei. Schlägt der Leseversuch fehl, so wird getestet, ob eine 
        /// Kopie der Datei, wie sie von SaveReplaceFile() angelegt wird existiert und versucht das Objekt 
        /// daraus wiederherzustellen.
        /// </summary>
        /// <remarks>
        /// Falls die Datei nur aus der Kopie geladen werden konnte, so wird automatisch die Kopie an die 
        /// Stelle der Originaldatei verschoben.
        /// </remarks>
        /// <typeparam name="T">Der Typ des Objekts, welches geladen werden soll.</typeparam>
        /// <param name="filename">Datei, aus welcher das Objekt geladen werden soll.</param>
        /// <returns>Das geladene Objekt.</returns>
        public static T SaveReadFromFile<T>(string filename)
        {
            try
            {
                return ReadFromFile<T>(filename);
            }
            catch(Exception e)
            {
                // Lesen aus der Originaldatei nicht möglich
                string bkp = GetBackupFilePath(filename);

                // Prüfe, ob die Backupdatei existiert
                if (!File.Exists(bkp))
                    throw new SerializedFileBackupException("Original file could not be read and there is " +
                        " no backup file.", e);

                // Lese das Objekt aus der Backupdatei
                T obj = ReadFromFile<T>(bkp);

                // Konnte das Objekt nur aus der Backupdatei lesen. -> Originaldatei ist evtl. defekt
                // -> versuche diese zu löschen.
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                // Verschiebe die Backupdatei zur Originaldatei.
                File.Move(bkp, filename);

                return obj;
            }
        }

        /// <summary>
        /// Speichert das Objekt mit Hilfe des .Net-XmlSerializers in die angegebene Datei. (Ohne spezielle 
        /// Vorsichtsmassnahme, eine evtl. existierende Datei wird überschrieben.)
        /// </summary>
        /// <param name="obj">Objekt, welches serialisiert werden soll. Die Serialisierung geschieht mit Hilfe des 
        /// .Net-XmlSerializers und muss daher dessen Bedingungen erfüllen, damit die Serialisierung erfolgreich 
        /// durchgeführt werden kann.</param>
        /// <param name="filepath">Dateiname der Datei, in welche das Objekt serialisiert wird.</param>
        public static void Write2File(object obj, string filepath)
        {
            XmlSerializer ser = new XmlSerializer(obj.GetType());
            XmlTextWriter writer = new XmlTextWriter(filepath, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            try
            {
                ser.Serialize(writer, obj);
            }
            finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Lädt ein Objekt aus der angegebenen Datei.
        /// </summary>
        /// <typeparam name="T">Der Typ des Objekts, welches geladen werden soll.</typeparam>
        /// <param name="filename">Datei, aus welcher das Objekt geladen werden soll.</param>
        /// <returns>Das geladene Objekt.</returns>
        public static T ReadFromFile<T>(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            XmlTextReader reader = new XmlTextReader(filename);
            T obj;

            try
            {
                obj = (T)ser.Deserialize(reader);
            }
            finally
            {
                reader.Close();
            } 

            return obj;
        }

        /// <summary>
        /// Bestimmt den Namen der Backupdatei, welcher von SaveReplaceFile() verwendet wird.
        /// </summary>
        /// <param name="filepath">Datei für welche die Backupdatei bestimmt wird.</param>
        /// <returns>Den Pfad zur Backupdatei.</returns>
        public static string GetBackupFilePath(string filepath)
        {
            // Bestimme den Namen der temporären Datei.
            string name = Path.GetFileNameWithoutExtension(filepath);
            string path = Path.GetDirectoryName(filepath);
            string ext = Path.GetExtension(filepath);

            return Path.Combine(path, name + "~" + ext);
        }

    }
}

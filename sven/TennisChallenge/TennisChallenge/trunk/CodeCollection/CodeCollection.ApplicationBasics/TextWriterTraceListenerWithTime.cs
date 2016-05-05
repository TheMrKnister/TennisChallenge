using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Inspectron.CodeCollection.ApplicationBasics
{
    /// <summary>
    /// Erweiterung von TextWriterTraceListener, welche TextWriterTraceListener.WriteLine() überschreibt und vor jede
    /// Zeile das aktuelle Datum und die Uhrzeit schreibt.
    /// </summary>
    public class TextWriterTraceListenerWithTime : TextWriterTraceListener
    {
        public TextWriterTraceListenerWithTime()
            : base()
        {
        }

        public TextWriterTraceListenerWithTime(Stream stream)
            : base(stream)
        {
        }

        public TextWriterTraceListenerWithTime(string path)
            : base(path)
        {
        }

        public TextWriterTraceListenerWithTime(TextWriter writer)
            : base(writer)
        {
        }

        public TextWriterTraceListenerWithTime(Stream stream, string name)
            : base(stream, name)
        {
        }

        public TextWriterTraceListenerWithTime(string path, string name)
            : base(path, name)
        {
        }

        public TextWriterTraceListenerWithTime(TextWriter writer, string name)
            : base(writer, name)
        {
        }

        public override void WriteLine(string message)
        {
            message = message.Replace(Environment.NewLine, Environment.NewLine + "                     ");
            base.Write(DateTime.Now.ToString("yy-MM-dd HH:mm:ss.f") + ": ");
            base.WriteLine(message);
        }
    }
}

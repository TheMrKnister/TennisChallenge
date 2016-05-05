using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


namespace Inspectron.CodeCollection.ApplicationBasics
{
    /// <summary>
    /// Wird bei spezifischen Ausnahmen in der AppEnvironment Klasse verwendet.
    /// </summary>
    public class AppEnvironmentException : Exception, ISerializable
    {
        public AppEnvironmentException()
            : base()
        {
        }

        public AppEnvironmentException(string message)
            : base(message)
        {
        }

        public AppEnvironmentException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AppEnvironmentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

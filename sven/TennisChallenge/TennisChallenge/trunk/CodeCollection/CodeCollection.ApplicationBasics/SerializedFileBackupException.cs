using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Inspectron.CodeCollection.ApplicationBasics
{
    public class SerializedFileBackupException : Exception, ISerializable
    {
        public SerializedFileBackupException()
            : base()
        {
        }

        public SerializedFileBackupException(string message)
            : base(message)
        {
        }

        public SerializedFileBackupException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SerializedFileBackupException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

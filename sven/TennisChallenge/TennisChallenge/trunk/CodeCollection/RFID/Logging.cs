using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Inspectron.CodeCollection.RFID
{
  class Logging
  {
    public static TraceSwitch LogLevel = new TraceSwitch("RfidTraceLevel", "Rfid Trace-Level");
  }
}

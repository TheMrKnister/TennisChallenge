using System.Diagnostics;
using Inspectron.CodeCollection.ApplicationBasics;

namespace TennisChallenge.InfoBoard
{
  class Logging : LoggingBase
  {
    public static TraceSwitch LogLevel = new TraceSwitch("TraceLevelSwitch", "Global Trace-Level");
  }
}

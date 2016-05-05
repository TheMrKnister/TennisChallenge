using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Inspectron.CodeCollection.RFID
{
    public delegate void Del_ToThreadedMethod();

    internal class ThreadingBase
    {
        #region Construction / Destruction

        public ThreadingBase(Del_ToThreadedMethod MethodToThread, ThreadPriority Priority)
        {
            mMethodToThread = MethodToThread;
            mThread = new Thread(new ThreadStart(ThreadMain));
            mThread.Priority = Priority;
            mThread.IsBackground = true;
            mRun = true;
            mThread.Start(); 
        }

        #endregion Construction / Destruction

        #region Public Methods

        public void ThreadMain()
        {
            try
            {
                mIsRunning = true;
                while (mRun)
                {
                  try
                  {
                    mMethodToThread();
                  }
                  catch (Exception e)
                  {
                    if(Logging.LogLevel.TraceWarning)
                      Trace.WriteLine(e.ToString());
                  }
                }
            }
            finally
            {
                mIsRunning = false;
                mRun = false;
            }
        }

        public bool IsRunning()
        {
            return mIsRunning;
        }

        public void Stop()
        {
            mRun = false;
        }

        #endregion Public Methods

        #region Attributes

        Thread mThread;
        Del_ToThreadedMethod mMethodToThread;
        bool mIsRunning;
        bool mRun;

        #endregion Attributes

    } // class ThreadingBase

} // namespace RFID

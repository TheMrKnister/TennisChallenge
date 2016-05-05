using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace Inspectron.CodeCollection.RFID
{
    public delegate void Del_OnSerialRead(string s);

    public class ThreadedSerialPort
    {

        #region Construction / Destruction

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Port e.g. COM3</param>
        /// <param name="OnSerialRead">Callback delegate</param>
        public ThreadedSerialPort(String port, Del_OnSerialRead OnSerialRead)
        {
            mPort = port;
            mConnection = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            mConnection.Handshake = Handshake.None;
            mConnection.NewLine = "\r\n";
            mOnSerialRead = OnSerialRead;
        }

        #endregion Construction / Destruction

        #region Public Methods

        public void Connect(bool suppressDuplicates)
        {
            mSuppressDuplicates = suppressDuplicates;
            mConnection.Open();
            mThreading = new ThreadingBase(new Del_ToThreadedMethod(Read), System.Threading.ThreadPriority.BelowNormal); 
        }

        public void Disconnect()
        {
            if (mThreading != null)
            {
              mThreading.Stop();
            }
            mConnection.Close();
        }

        public static string[] getPorts()
        {
            return SerialPort.GetPortNames();
        }

        #endregion Public Methods

        #region Private Methods

        private void Read()
        {
            mLastLine = mConnection.ReadLine();

            Match match = sRegex.Match(mLastLine);
            if (match.Success)
            {
                if (Logging.LogLevel.TraceInfo)
                {
                  Trace.WriteLine("RFID read: " + match.Value);
                }

                if (!mSuppressDuplicates || (match.Value != mLastRfid))
                {
                    mLastRfid = match.Value.Trim();
                    mOnSerialRead(mLastRfid);
                }
            }
            else
            {
                if (Logging.LogLevel.TraceError)
                {
                    Trace.WriteLine("Regex.Match fehlgeschlagen: " + mLastLine);
                }
            }

        } // Read()

        #endregion Private Methods

        #region Attributes

        static Regex sRegex = new Regex(@"[0-9a-fA-F]+\s*$");

        string mPort;
        Del_OnSerialRead mOnSerialRead;
        SerialPort mConnection;
        bool mSuppressDuplicates;
        string mLastLine = string.Empty;
        string mLastRfid = string.Empty;


        private ThreadingBase mThreading;

        #endregion Attributes

    } // class ThreadedSerialPort

} // namespace RFID

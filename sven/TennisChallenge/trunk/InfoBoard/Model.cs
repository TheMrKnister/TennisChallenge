using System;
using System.Diagnostics;
using Inspectron.CodeCollection.RFID;

namespace TennisChallenge.InfoBoard
{
    public class Model : IDisposable
    {

        private readonly ThreadedSerialPort _rfidReader;

        private Tuple<string, DateTime> _lastRfidInfo;

        public Model()
        {
            var comPort = Properties.Settings.Default.RfidComPort;
            _rfidReader = new ThreadedSerialPort(comPort, OnRfidRead);
        }

        public void Connect()
        {
            _rfidReader.Connect(false);
        }

        public event Action<string> RfidRead;

        private void OnRfidRead(string rfid)
        {
            // Unterdrücke gleichen Rfid-Wert innerhalb von 3 Sekunden
            if ((_lastRfidInfo != null) && (_lastRfidInfo.Item1 == rfid) &&
              (DateTime.Now.Subtract(_lastRfidInfo.Item2) < new TimeSpan(0, 0, 1)))
            {
                return;
            }

            _lastRfidInfo = new Tuple<string, DateTime>(rfid, DateTime.Now);

            var temp = RfidRead;

            if (temp != null)
            {
                temp(rfid);
            }
        }

        public void Dispose()
        {
            _rfidReader.Disconnect();
        }
    }
}

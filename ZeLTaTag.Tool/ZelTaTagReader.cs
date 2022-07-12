using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeLTaTag.Tool.Enums;
using ZeLTaTag.Tool.Interfaces;

namespace ZeLTaTag.Tool
{
    public class ZelTaTagReader : IZeLTaTag
    {
        private SerialPort mySerialPort;

        private string previosRead = string.Empty;

        public event EventHandler<string>? OnTagReaded;
        public ReaderType readerType;

        public ZelTaTagReader(int portNumber, ReaderType type = ReaderType.MultipleTimes) 
        {
            mySerialPort = new SerialPort($"COM{portNumber.ToString()}");
            readerType = type;

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;

            mySerialPort.DataReceived += SerialPort_DataReceived;
            try {
                mySerialPort.Open();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string info = string.Empty;
            for (int i = 0; i < 20; i++)
            {
                int indata = sp.ReadByte();
                if (i > 6 && i < 19)
                    info += indata.ToString("X2");
            }

            info = info.ToLower();
            switch (readerType)
            {
                case ReaderType.MultipleTimes:
                    OnTagReaded?.Invoke(this, info);
                    previosRead = info;
                    break;

                case ReaderType.SingleTime:
                    ValidateSingleRead(info);
                    break;
            }
        }

        public bool isCurrentlyReading()
        {
            return mySerialPort.IsOpen;
        }

        public void ClearPreviousRead()
        {
            previosRead = string.Empty;
        }

        private void ValidateSingleRead(string tag)
        {
            if (previosRead == string.Empty)
            {
                OnTagReaded?.Invoke(this, tag);
                previosRead = tag;
                return;
            }

            if (tag != previosRead)
            {
                OnTagReaded?.Invoke(this, tag);
                previosRead = tag;
            }
        }
    }
}

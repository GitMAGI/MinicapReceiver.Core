using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CA_Main
{
    public class RetrievingActivity : BaseActivity
    {
        private Stack<byte[]> _inputStack;
        private Socket _socket;

        public RetrievingActivity(Socket socket, Stack<byte[]> inputStack, string activityName = "Retrieving", int sleepingTime = 1) : base(activityName, sleepingTime)
        {
            if (socket == null)
                throw new ArgumentNullException(string.Format("Input {0} cannot be null! Initialize it before to instantiate a {1} object", socket.GetType().FullName, this.GetType().FullName));

            if (inputStack == null)
                throw new ArgumentNullException(string.Format("Input {0} cannot be null! Initialize it before to instantiate a {1} object", inputStack.GetType().FullName, this.GetType().FullName));

            _socket = socket;
            _inputStack = inputStack;
        }

        override protected void _runner()
        {
            try
            {
                _logger.Debug("Retrieving Packet Size. Reading 4 Bytes from Socket ...");
                byte[] packetSizeData = new byte[4];
                //int packetSizeDataLength = _socket.Receive(packetSizeData, 4, SocketFlags.None);
                int packetSizeDataLength = _continousReading(_socket, ref packetSizeData, 4);
                _logger.Debug("Retrieving Packet Size. Read {0} of {1} Bytes from Socket", packetSizeDataLength, 4);
                int packetSize = BitConverter.ToInt32(packetSizeData, 0);
                _logger.Debug("Decode Packet Size is {0}", packetSize);

                _logger.Debug("Retrieving Packet. Reading {0} Bytes from Socket ...", packetSize);
                byte[] data = new byte[packetSize];
                //int packetSizeLength = _socket.Receive(data, packetSize, SocketFlags.None);
                int packetSizeLength = _continousReading(_socket, ref data, packetSize);
                _logger.Debug("Retrieving Packet. Read {0} of {1} Bytes from Socket", packetSizeLength, packetSize);

                if (data != null)
                {
                    _logger.Debug("Pushing data into the Input Stack ...");
                    _inputStack.Push(data);
                    _logger.Debug("Data pusched successfully");
                }
            }
            catch(Exception e)
            {
                _logger.Warning(e, string.Format("Something went wrong during retrieving data from socket! Packet skipped! Error: {0}", e.Message));
            }  
        }

        private int _continousReading(Socket s, ref byte[] data, int expectedLength)
        {
            int readRemaining = expectedLength;
            int currentPosition = 0;

            while (readRemaining > 0)
            {
                byte[] tmp = new byte[readRemaining];
                int readLength = s.Receive(tmp, readRemaining, SocketFlags.None);   
                tmp = new List<byte>(tmp).GetRange(0, readLength).ToArray();
                readRemaining -= readLength;
                _logger.Debug(string.Format("Read {0} B of expected {1} B. Remaining {2} B", readLength, expectedLength, readRemaining));
                _logger.Debug(string.Format("Data Array Length {0} B. Current Position {1} B. Tmp Array Length {2} B", data.Length, currentPosition, tmp.Length));
                // Concat byte array
                tmp.CopyTo(data, currentPosition);
                currentPosition += readLength;
            }

            return expectedLength - readRemaining;
        }
    }
}

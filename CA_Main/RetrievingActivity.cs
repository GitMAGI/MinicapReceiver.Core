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
            _logger.Debug("Retrieving Packet Size. Reading 4 Bytes from Socket ...");
            byte[] packetSizeData = new byte[4];
            int packetSizeDataLength = _socket.Receive(packetSizeData, 4, SocketFlags.None);
            _logger.Debug("Retrieving Packet Size. Read {0} of {1} Bytes from Socket", packetSizeDataLength, 4);
            int packetSize = BitConverter.ToInt32(packetSizeData, 0);
            _logger.Debug("Decode Packet Size is {0}", packetSize);

            _logger.Debug("Retrieving Packet. Reading {0} Bytes from Socket ...", packetSize);
            byte[] data = new byte[packetSize];
            int packetSizeLength = _socket.Receive(data, packetSize, SocketFlags.None);
            _logger.Debug("Retrieving Packet. Read {0} of {1} Bytes from Socket", packetSizeLength, packetSize);

            if (data != null)
            {
                _logger.Debug("Pushing data into the Input Stack ...");
                _inputStack.Push(data);
                _logger.Debug("Data pusched successfully");
            }                
        }
    }
}

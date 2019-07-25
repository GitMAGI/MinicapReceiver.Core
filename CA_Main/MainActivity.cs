using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CA_Main
{
    public class MainActivity
    {
        private Serilog.Core.Logger _logger = Logger.GetInstance();

        public string RemoteIP { get; private set; }
        public int RemotePort { get; private set; }

        private Socket _socket;
        
        public uint RealScreenWidth { get; private set; }
        public uint RealScreenHeight { get; private set; }

        public double ScalingFactor { get; private set; }

        public uint VirtualScreenWidth { get; private set; }
        public uint VirtualScreenHeight { get; private set; }

        public byte DisplayOrientation { get; private set; }

        public MainActivity()
        {
            RemoteIP = "127.0.0.1";
            RemotePort = 1717;

            ScalingFactor = 0.4;
        }

        public void Start()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                _logger.Information(string.Format("Main Action Starting  ..."));

                IPAddress remoteAddr = IPAddress.Parse(RemoteIP);
                IPEndPoint remoteEndPoint = new IPEndPoint(remoteAddr, RemotePort);
                _socket = new Socket(remoteAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _logger.Information("Starting TCP Connection to {0}:{1} ...", RemoteIP, RemotePort);
                _socket.Connect(remoteEndPoint);
                _logger.Information("TCP Connection to {0}:{1} successfully established!", RemoteIP, RemotePort);

                byte[] globalHeader = new byte[24];
                // Retrieve Global Header Informartions
                _socket.Receive(globalHeader, 24, SocketFlags.None);

                byte version = globalHeader[0]; 
                byte headerSize = globalHeader[1];
                uint pid = BitConverter.ToUInt32(globalHeader, 2);
                RealScreenWidth = BitConverter.ToUInt32(globalHeader, 6);
                RealScreenHeight = BitConverter.ToUInt32(globalHeader, 10);
                VirtualScreenWidth = BitConverter.ToUInt32(globalHeader, 14);
                VirtualScreenHeight = BitConverter.ToUInt32(globalHeader, 18);
                DisplayOrientation = globalHeader[22];
                byte quirkFlag = globalHeader[23];

                _logger.Information("Information from the Server: Version {0}", version);
                _logger.Information("Information from the Server: Header Size {0}", headerSize);
                _logger.Information("Information from the Server: PID {0}", pid);
                _logger.Information("Information from the Server: Real Screen Width {0}", RealScreenWidth);
                _logger.Information("Information from the Server: Real Screen Height {0}", RealScreenHeight);
                _logger.Information("Information from the Server: Virtual Screen Width {0}", VirtualScreenWidth);
                _logger.Information("Information from the Server: Virtual Screen Height {0}", VirtualScreenHeight);
                _logger.Information("Information from the Server: DisplayOrientation {0}", DisplayOrientation);
                _logger.Information("Information from the Server: Quirk Flag {0}", quirkFlag);

                _logger.Information("Shutting down TCP socket {0}:{1} ..", RemoteIP, RemotePort);
                _socket.Shutdown(SocketShutdown.Both);
                _logger.Information("Socket shutted down");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if(_socket != null)
                {
                    _logger.Information("Closing TCP socket {0}:{1} ..", RemoteIP, RemotePort);
                    _socket.Close();
                    _logger.Information("Socket closed");
                }                

                stopwatch.Stop();
                _logger.Information(string.Format("Main Action Completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }
}

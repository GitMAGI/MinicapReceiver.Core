using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CA_FakeServer
{
    public class MainActivityAsync
    {
        private Serilog.Core.Logger _logger = Logger.GetInstance();

        public int LocalPort { get; private set; }
        public string LocalIP { get; private set; }
        private volatile bool _keepRunning;
        private volatile bool _keepTransmitting;

        public ManualResetEvent allDone = new ManualResetEvent(false);

        public int MaximumConnections { get; private set; }
        public int MainLoopTimeSleeping { get; private set; }

        private byte[] _header = new byte[24];
        private List<byte[]> _packets = new List<byte[]>();

        public MainActivityAsync()
        {
            LocalPort = 1717;
            LocalIP = "127.0.0.1";
            MainLoopTimeSleeping = 1;
            MaximumConnections = 1;

            // Building up the main header to transmit
            _header = FakeProcessing.HeaderMaker();
            // Acquiring data to transmit
            _packets = FakeProcessing.ImageExtraction();
        }

        public void Stop()
        {
            allDone.Set();
            _keepTransmitting = false;
            _keepRunning = false;
        }

        private void Send(Socket handler, byte[] header, List<byte[]> images)
        {
            IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            string remoteIP = remoteIpEndPoint.Address.ToString();
            int remotePort = remoteIpEndPoint.Port;

            _logger.Information("Trasmission on socket {0}:{1} starting ...", remoteIP, remotePort);

            try { handler.Send(header); } catch (Exception) { throw; }
            _logger.Information("Header sent on socket {0}:{1}", remoteIP, remotePort);

            _keepTransmitting = true;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            while (_keepTransmitting)
            {
                if (images == null || images.Count == 0)
                {
                    _logger.Information("No data to Transmit on socket {0}:{1} ...", remoteIP, remotePort);
                    break;
                }

                //_logger.Debug("Data in continous trasmission on socket {0}:{1}", remoteIP, remotePort);
                foreach (byte[] image in images)
                {
                    try { handler.Send(header); } catch (Exception) {
                        _keepTransmitting = false;
                        break;
                    }
                    Thread.Sleep(MainLoopTimeSleeping);
                }
                Thread.Sleep(MainLoopTimeSleeping);
            }

            _logger.Information("Trasmission on socket {0}:{1} terminated", remoteIP, remotePort);
            _logger.Information("Shutting down TCP socket {0}:{1} ..", remoteIP, remotePort);
            handler.Shutdown(SocketShutdown.Both);
            _logger.Information("Socket shut down");
            _logger.Information("Closing TCP socket {0}:{1} ..", remoteIP, remotePort);
            handler.Close();
            _logger.Information("Socket closed");

            _logger.Information("Socket {0}:{1} is listening for connections (MAX: {2})", LocalIP, LocalPort, MaximumConnections);
        }

        private void BeginAcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            string remoteIP = remoteIpEndPoint.Address.ToString();
            int remotePort = remoteIpEndPoint.Port;

            _logger.Information("A connection on socket {0}:{1} has been established", remoteIP, remotePort);

            Send(handler, _header, _packets);
        }
                
        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Stop();
            _logger.Information(string.Format("Cancelling Main Action Execution ..."));
        }
        
        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _logger.Information(string.Format("Main Action Starting  ..."));

            IPAddress localAddr = IPAddress.Parse(LocalIP);
            IPEndPoint localEndPoint = new IPEndPoint(localAddr, LocalPort);
            Socket listener = new Socket(localAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);            

            try
            {
                _logger.Information("Socket {0}:{1} initialized", LocalIP, LocalPort);
                listener.Bind(localEndPoint);
                _logger.Information("Socket {0}:{1} bounded", LocalIP, LocalPort);
                listener.Listen(MaximumConnections);
                _logger.Information("Socket {0}:{1} is listening for connections (MAX: {2})", LocalIP, LocalPort, MaximumConnections);

                _keepRunning = true;
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                while (_keepRunning)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                                        
                    listener.BeginAccept(new AsyncCallback(BeginAcceptCallback), listener);                    
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();

                    //_logger.Information("Socket {0}:{1} is listening for connections (MAX: {2})", LocalIP, LocalPort, MaximumConnections);
                }
            }
            catch (Exception) { throw; }
            finally
            {
                stopwatch.Stop();
                _logger.Information(string.Format("Main Action Completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }
}

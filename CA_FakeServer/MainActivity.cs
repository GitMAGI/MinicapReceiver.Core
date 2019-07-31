using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.IO;

namespace CA_FakeServer
{
    public class MainActivity
    {
        private Serilog.Core.Logger _logger = Logger.GetInstance();

        public int LocalPort { get; private set; }
        public string LocalIP { get; private set; }

        private volatile bool _keepRunning;
        private volatile bool _keepTransmitting;

        public int MainLoopTimeSleeping { get; private set; }

        private byte[] _header = new byte[24];
        private List<byte[]> _packets = new List<byte[]>();

        public MainActivity()
        {
            LocalPort = 1717;
            LocalIP = "127.0.0.1";
            MainLoopTimeSleeping = 1;

            // Building up the main header to transmit
            _header = FakeProcessing.HeaderMaker();
            // Acquiring data to transmit
            string startupPath = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.FullName;
            string input_fullfilename = System.IO.Path.Combine(startupPath, "Input", "video.mp4");
            string program_fullfileame = System.IO.Path.Combine(System.IO.Path.GetPathRoot(Environment.SystemDirectory), "ffmpeg", "bin", "ffmpeg.exe");
            uint w = 270;
            uint h = 480;
            _packets = FakeProcessing.ImageExtraction(program_fullfileame, input_fullfilename, w, h);
        }

        public void Stop()
        {
            _keepTransmitting = false;
            _keepRunning = false;
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
                listener.Listen(1);

                //Loop Main Activity until Stop is set
                _keepRunning = true;
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                while (_keepRunning)
                {
                    _logger.Information("Socket {0}:{1} is listening for connections", LocalIP, LocalPort);
                    Socket handler = listener.Accept();

                    IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
                    string remoteIP = remoteIpEndPoint.Address.ToString();
                    int remotePort = remoteIpEndPoint.Port;

                    try
                    {
                        _logger.Information("A connection on socket {0}:{1} has been established", remoteIP, remotePort);
                        _keepTransmitting = true;

                        _logger.Information("Starting trasmission on socket {0}:{1} ...", remoteIP, remotePort);

                        // Transmit just the header
                        handler.Send(_header);
                        _logger.Information("Header sent on socket {0}:{1}", remoteIP, remotePort);

                        if(_packets == null || _packets.Count == 0)
                        {
                            _logger.Information("No data to Transmit on socket {0}:{1} ...", remoteIP, remotePort);
                            break;
                        }

                        while (_keepTransmitting)
                        {
                            foreach (byte[] packet in _packets)
                            {
                                handler.Send(packet);
                                Thread.Sleep(MainLoopTimeSleeping);
                            }
                            Thread.Sleep(MainLoopTimeSleeping);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, string.Format("Handler Socket {0}:{1} failure! Message: {2}", remoteIP, remotePort, ex.Message));
                    }
                    finally
                    {
                        _logger.Information("Trasmission on socket {0}:{1} terminated", remoteIP, remotePort);

                        if (handler != null)
                        {
                            _logger.Information("Shutting down TCP socket {0}:{1} ..", remoteIP, remotePort);
                            handler.Shutdown(SocketShutdown.Send);
                            _logger.Information("Socket shut down");

                            _logger.Information("Closing TCP socket {0}:{1} ..", remoteIP, remotePort);
                            handler.Close();
                            _logger.Information("Socket closed");
                        }
                    }

                    Thread.Sleep(MainLoopTimeSleeping);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (listener != null)
                {
                    try
                    {
                        _logger.Information("Shutting down TCP socket {0}:{1} ..", LocalIP, LocalPort);
                        listener.Shutdown(SocketShutdown.Both);
                        _logger.Information("Socket shut down");
                    }
                    catch (Exception) { }

                    _logger.Information("Closing TCP socket {0}:{1} ..", LocalIP, LocalPort);
                    listener.Close();
                    _logger.Information("Socket closed");
                }

                stopwatch.Stop();
                _logger.Information(string.Format("Main Action Completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Stop();
            _logger.Information(string.Format("Cancelling Main Action Execution ..."));
        }

    }
}
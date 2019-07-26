using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        private bool _keepRunning;
        
        public int MainLoopTimeSleeping { get; private set; }

        private RetrievingActivity _retriever = null;
        private ProcessingActivity _processor = null;
        private DisplayingActivity _displayer = null;
        private MonitoringActivity _monitoring = null;

        public MainActivity()
        {
            RemoteIP = "127.0.0.1";
            RemotePort = 1717;

            ScalingFactor = 0.4;

            MainLoopTimeSleeping = 1;
        }

        public void Stop()
        {
            _keepRunning = false;
        }

        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Task taskRetriever = new Task(() => {
                _retriever = new RetrievingActivity(null, 1);
                _retriever.Run();
            });
            Task taskProcessor = new Task(() => {
                _processor = new ProcessingActivity(null, null, 1);
                _processor.Run();
            });
            Task taskDisplayer = new Task(() => {
                _displayer = new DisplayingActivity(null, 1);
                _displayer.Run();
            });
            Task taskMonitoring = new Task(() => {
                _monitoring = new MonitoringActivity(null, null, 1);
                _monitoring.Run();
            });

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

                //Thread for retrieving data from TCP 
                taskRetriever.Start();                

                //Thread for Processing 
                taskProcessor.Start();

                //Thread for Displaying frames 
                taskDisplayer.Start();

                //Thread for Monitoring queues status 
                taskMonitoring.Start();

                //Loop Main Activity until Stop is set
                _keepRunning = true;
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                while (_keepRunning)
                    Thread.Sleep(MainLoopTimeSleeping);                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Complete All Threads
                if (taskRetriever.Status == TaskStatus.Running)
                {
                    _retriever.Stop();
                    taskRetriever.Wait();
                }
                if (taskProcessor.Status == TaskStatus.Running)
                {
                    _processor.Stop();
                    taskProcessor.Wait();
                }
                if (taskDisplayer.Status == TaskStatus.Running)
                {
                    _displayer.Stop();
                    taskDisplayer.Wait();
                }
                if (taskMonitoring.Status == TaskStatus.Running)
                {
                    _monitoring.Stop();
                    taskMonitoring.Wait();
                }

                if (_socket != null)
                {
                    _logger.Information("Shutting down TCP socket {0}:{1} ..", RemoteIP, RemotePort);
                    _socket.Shutdown(SocketShutdown.Both);
                    _logger.Information("Socket shut down");

                    _logger.Information("Closing TCP socket {0}:{1} ..", RemoteIP, RemotePort);
                    _socket.Close();
                    _logger.Information("Socket closed");
                }                

                stopwatch.Stop();
                _logger.Information(string.Format("Main Action Completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _retriever?.Stop();
            _processor?.Stop();
            _displayer?.Stop();
            _monitoring?.Stop();
            this.Stop();
            _logger.Information(string.Format("Cancelling Main Action Execution ..."));
        }
    }
}

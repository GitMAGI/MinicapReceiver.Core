using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CA_Main
{
    class Program
    {
        public static readonly string AppName = "MinicapReceiver.Core";
        private static int _mainLoopSleepingTime = 1; // Milliseconds
        private static bool _keepRunning = true;
        private static Serilog.Core.Logger _logger = Logger.GetInstance();

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                _logger.Information(string.Format("Starting Application {0} ...", AppName));

                MainActivity main = new MainActivity();
                main.Start();

                // Loop Until Exit command is detected   
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                while (_keepRunning)
                    Thread.Sleep(_mainLoopSleepingTime);

                // Cleaning up all other threads
                // main.Stop(); // I think
            }
            catch(Exception ex)
            {
                _logger.Fatal(ex, string.Format("An Error Occurred! Message: {0}", ex.Message));
            }
            finally
            {
                stopwatch.Stop();
                _logger.Information(string.Format("Application {0} completed in {1}", AppName, Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _keepRunning = false;
            _logger.Information(string.Format("Cancelling Application {0} Execution ...", AppName));
        }
    }
}

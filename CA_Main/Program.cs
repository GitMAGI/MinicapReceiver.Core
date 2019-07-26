using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CA_Main
{
    class Program
    {
        public static readonly string AppName = "MinicapReceiver.Core";
        private static Serilog.Core.Logger _logger = Logger.GetInstance();

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                _logger.Information(string.Format("Starting Application {0} ...", AppName));

                MainActivity main = new MainActivity();
                main.Run();
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
    }
}

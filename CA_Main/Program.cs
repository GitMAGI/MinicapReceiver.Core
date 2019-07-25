using System;
using System.Diagnostics;

namespace CA_Main
{
    class Program
    {
        public static string appName = "MinicapReceiver.Core";

        static void Main(string[] args)
        {
            Serilog.Core.Logger logger = Logger.GetInstance();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {                
                logger.Information(string.Format("Starting Application {0} ...", appName));
                                
                TestTestTest.InnerTest00();
                (new ClassTest02()).Method01();

                ActionTest.Test00();
            }
            catch(Exception ex)
            {
                logger.Fatal(ex, string.Format("An Error Occurred! Message: {0}", ex.Message));
            }
            finally
            {
                stopwatch.Stop();
                logger.Information(string.Format("Application {0} completed in {1}", appName, Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }
}

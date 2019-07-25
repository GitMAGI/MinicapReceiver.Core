using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CA_Main
{
    public static class ActionTest
    {
        public static void Test00()
        {
            Serilog.Core.Logger logger = Logger.GetInstance();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                logger.Information(string.Format("Starting Test ..."));

                throw new ArgumentException("Errori di test Dajeeeee!");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stopwatch.Stop();
                logger.Information(string.Format("Test completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }

    public static class TestTestTest
    {
        public static void InnerTest00()
        {
            Serilog.Core.Logger logger = Logger.GetInstance();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                logger.Information(string.Format("Starting Test ..."));
                (new ClassTest()).Method01();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stopwatch.Stop();
                logger.Information(string.Format("Test completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }

    public class ClassTest
    {
        private Serilog.Core.Logger logger = Logger.GetInstance();

        public ClassTest(){ }

        public void Method01()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                logger.Information(string.Format("Starting Test ..."));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stopwatch.Stop();
                logger.Information(string.Format("Test completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }

    public class ClassTest02
    {
        private Serilog.Core.Logger logger = null;

        public ClassTest02()
        {
            logger = Logger.GetInstance();
        }

        public void Method01()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                logger.Information(string.Format("Starting Test ..."));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stopwatch.Stop();
                logger.Information(string.Format("Test completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }
}

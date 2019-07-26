using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CA_Main
{
    public class RetrievingActivity
    {
        private Serilog.Core.Logger _logger = Logger.GetInstance();
        private bool _keepRunning = false;
        public object InputQueue;
        public int WorkerLoopTimeSleeping { get; private set; }

        public RetrievingActivity(object inputQueue, int sleepingTime = 1)
        {
            InputQueue = inputQueue;
            WorkerLoopTimeSleeping = sleepingTime;
        }

        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                _logger.Information(string.Format("Retrieving Action Starting  ..."));
                _keepRunning = true;
                while (_keepRunning)
                {

                    Thread.Sleep(WorkerLoopTimeSleeping);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.Information(string.Format("Retrieving Action Completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }

        public void Stop()
        {
            this._keepRunning = false;
        }
    }
}

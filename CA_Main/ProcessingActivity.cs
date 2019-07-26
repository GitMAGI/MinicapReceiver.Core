using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CA_Main
{
    public class ProcessingActivity
    {
        private Serilog.Core.Logger _logger = Logger.GetInstance();
        private bool _keepRunning = false;
        public object InputQueue;
        public object OutputQueue;
        public int WorkerLoopTimeSleeping { get; private set; }

        public ProcessingActivity(object inputQueue, object outputQueue, int sleepingTime)
        {
            InputQueue = inputQueue;
            OutputQueue = outputQueue;
            WorkerLoopTimeSleeping = sleepingTime;
        }

        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                _logger.Information(string.Format("Processing Action Starting  ..."));
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
                _logger.Information(string.Format("Processing Action Completed in {0}", Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }

        public void Stop()
        {
            this._keepRunning = false;
        }
    }
}

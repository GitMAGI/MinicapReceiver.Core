using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DLL_Core
{
    public class BaseActivity
    {
        protected Serilog.Core.Logger _logger = Logger.GetInstance();
        protected bool _keepRunning = false;
        protected readonly string _activityName;
        public int WorkerLoopTimeSleeping { get; private set; }

        public BaseActivity(string activityName, int sleepingTime)
        {
            _activityName = activityName;
            WorkerLoopTimeSleeping = sleepingTime;
        }

        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                _logger.Information(string.Format("{0} Action Starting  ...", _activityName));
                _keepRunning = true;
                while (_keepRunning)
                {
                    _runner();
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
                _logger.Information(string.Format("{0} Action Completed in {1}", _activityName, Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }

        public void Stop()
        {
            _keepRunning = false;
        }

        virtual protected void _runner() { }
    }
}

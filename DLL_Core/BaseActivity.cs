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
                _initialize();
                _logger.Information(string.Format("{0} Action Starting  ...", _activityName));
                _keepRunning = true;
                while (_keepRunning)
                {
                    _runner();
                    Thread.Sleep(WorkerLoopTimeSleeping);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An Error occurred : {0}", ex.Message);
                _error();
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _cleaning();
                _logger.Information(string.Format("{0} Action Completed in {1}", _activityName, Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }

        public void Stop()
        {
            _keepRunning = false;
        }

        virtual protected void _initialize() { }

        virtual protected void _runner() { }

        virtual protected void _error() { }

        virtual protected void _cleaning() { }
    }
}

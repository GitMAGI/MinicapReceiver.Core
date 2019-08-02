using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CA_Main
{
    public class ProcessingActivity : BaseActivity
    {
        private Stack<byte[]> _inputStack;
        private Stack<byte[]> _outputStack;
        private int _limit;

        public ProcessingActivity(int limit, Stack<byte[]> inputStack, Stack<byte[]> outputStack, string activityName = "Processing", int sleepingTime = 1) : base(activityName, sleepingTime)
        {
            if (inputStack == null)
                throw new ArgumentNullException(string.Format("Input {0} cannot be null! Initialize it before to instantiate a {1} object", inputStack.GetType().FullName, this.GetType().FullName));

            if (outputStack == null)
                throw new ArgumentNullException(string.Format("Output {0} cannot be null! Initialize it before to instantiate a {1} object", outputStack.GetType().FullName, this.GetType().FullName));

            if(limit <= 0)
                throw new ArgumentNullException(string.Format("Limit {0} cannot be lower or equal to 0! Set it before to a value greater than 0 before to instantiate a {1} object", _limit, this.GetType().FullName));

            _limit = limit;
            _inputStack = inputStack;
            _outputStack = outputStack;
        }

        override protected void _runner()
        {
            if (_inputStack.Count == 0)
                return;

            _logger.Debug("Popping data from the Input Stack ...");
            byte[] data = null;
            if(!_inputStack.TryPop(out data))
            {
                _logger.Debug("Data popping failed!");
                return;
            }
            _logger.Debug("Data popped successfully");

            _logger.Debug("Pushing data into the Output Stack ...");
            if (_outputStack.Count > _limit)
                _outputStack.Clear();

            _outputStack.Push(data);
            _logger.Debug("Data pusched successfully");
        }
    }
}

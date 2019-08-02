using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CA_Main
{
    public class MonitoringActivity : BaseActivity
    {
        private Stack<byte[]> _inputStack;
        private Stack<byte[]> _outputStack;

        public MonitoringActivity(Stack<byte[]> inputStack, Stack<byte[]> outputStack, string activityName = "Monitoring", int sleepingTime = 1) : base(activityName, sleepingTime)
        {
            if (inputStack == null)
                throw new ArgumentNullException(string.Format("Input {0} cannot be null! Initialize it before to instantiate a {1} object", inputStack.GetType().FullName, this.GetType().FullName));

            if (outputStack == null)
                throw new ArgumentNullException(string.Format("Output {0} cannot be null! Initialize it before to instantiate a {1} object", outputStack.GetType().FullName, this.GetType().FullName));

            _inputStack = inputStack;
            _outputStack = outputStack;
        }

        override protected void _runner()
        {
            Console.Write("\rInput Stack Count {0} - Output Stack Count {1}", _inputStack.Count, _outputStack.Count);
        }
    }
}

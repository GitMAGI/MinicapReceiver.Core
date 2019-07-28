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

        public ProcessingActivity(Stack<byte[]> inputStack, Stack<byte[]> outputStack, string activityName = "Processing", int sleepingTime = 1) : base(activityName, sleepingTime)
        {
            _inputStack = inputStack;
            _outputStack = outputStack;
        }

        override protected void _runner()
        {
        }
    }
}

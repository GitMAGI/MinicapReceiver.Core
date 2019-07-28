using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CA_Main
{
    public class DisplayingActivity : BaseActivity
    {
        private Stack<byte[]> _outputStack;

        public DisplayingActivity(Stack<byte[]> outputStack, string activityName = "Displaying", int sleepingTime = 1) : base(activityName, sleepingTime)
        {
            _outputStack = outputStack;
        }

        override protected void _runner()
        {
        }
    }
}

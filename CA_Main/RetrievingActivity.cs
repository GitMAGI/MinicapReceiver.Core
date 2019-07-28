using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CA_Main
{
    public class RetrievingActivity : BaseActivity
    {
        private Stack<byte[]> _inputStack;

        public RetrievingActivity(Stack<byte[]> inputStack, string activityName = "Retrieving", int sleepingTime = 1) : base(activityName, sleepingTime)
        {
            _inputStack = inputStack;
        }

        override protected void _runner()
        {
        }
    }
}

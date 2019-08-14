using DLL_Core;
using OpenCvSharp;
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
        private Stack<Mat> _outputStack;

        private int _xCursor;
        private int _yCursor;

        private static readonly object _obj = new object();

        public MonitoringActivity(Stack<byte[]> inputStack, Stack<Mat> outputStack, string activityName = "Monitoring", int sleepingTime = 1) : base(activityName, sleepingTime)
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
            lock (_obj)
            {
                if (Console.CursorTop == this._yCursor)
                    Console.CursorTop--;

                string line = string.Format("Input Stack Count {0} - Output Stack Count {1}", _inputStack.Count, _outputStack.Count);
                Console.WriteLine(line);
                this._yCursor = Console.CursorTop;
            }
        }

        protected override void _initialize()
        {
            this._yCursor = Console.CursorTop;
        }

        protected override void _error()
        {
            Console.WriteLine("");
        }

        protected override void _cleaning()
        {
            
        }
    }
}

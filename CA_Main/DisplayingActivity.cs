using DLL_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using OpenCvSharp;

namespace CA_Main
{
    public class DisplayingActivity : BaseActivity
    {
        private Stack<byte[]> _outputStack;

        public DisplayingActivity(Stack<byte[]> outputStack, string activityName = "Displaying", int sleepingTime = 1) : base(activityName, sleepingTime)
        {
            if (outputStack == null)
                throw new ArgumentNullException(string.Format("Output {0} cannot be null! Initialize it before to instantiate a {1} object", outputStack.GetType().FullName, this.GetType().FullName));
            
            _outputStack = outputStack;
        }

        override protected void _runner()
        {
            if (_outputStack.Count == 0)
                return;

            _logger.Debug("Popping data from the Output Stack ...");
            byte[] datum = null;
            if(!_outputStack.TryPop(out datum))
            {
                _logger.Debug("Data popping failed!");
                return;
            }
            _logger.Debug("Data popped successfully");

            Mat src = Mat.ImDecode(datum, ImreadModes.Color);
            using (new Window("src image", src))
            {
                Cv2.WaitKey();
            }            
        }
    }
}

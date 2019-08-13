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
        private Stack<Mat> _outputStack;

        public DisplayingActivity(Stack<Mat> outputStack, string activityName = "Displaying", int sleepingTime = 1) : base(activityName, sleepingTime)
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
            Mat src = null;
            if(!_outputStack.TryPop(out src))
            {
                _logger.Debug("Data popping failed!");
                return;
            }
            _logger.Debug("Data popped successfully");

            Cv2.ImShow("Data", src);

            int keyPressed = Cv2.WaitKey(2);

            if(keyPressed == 27)
            {
                this.Stop();
                Cv2.DestroyAllWindows();
            }                
        }
    }
}

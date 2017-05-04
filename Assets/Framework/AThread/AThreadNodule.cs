using UnityEngine;
using System;
using System.Threading;
using System.Collections;

namespace Framework
{
    public class AThreadNodule
    {
        public Action onThreadFinish;
        public Action onThreadCancel;

        private AThreadStateType _threadState = AThreadStateType.NONE;
        public AThreadStateType threadState
        {
            get
            {
                return _threadState;
            }
        }

        private Thread _currentThread;


        public void StartThread(Action p_threadMethod, Action p_threadFinishCallback)
        {
            onThreadFinish = p_threadFinishCallback;

            _currentThread = new Thread(() =>
            {
                _threadState = AThreadStateType.ACTIVE;

                if (p_threadMethod != null)
                    p_threadMethod();

                _threadState = AThreadStateType.FINISHED;
            });

            _currentThread.Start();
        }

        public void CancelThread(Action p_threadCancelCallback = null)
        {
            onThreadCancel = p_threadCancelCallback;

            _threadState = AThreadStateType.CANCELLED;

            _currentThread.Abort();
        }
    }
}
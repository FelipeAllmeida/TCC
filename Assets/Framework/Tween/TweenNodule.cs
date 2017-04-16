using UnityEngine;
using System.Collections;
using System;

namespace Framework
{
    public class TweenNodule
    {
        public bool isAlive
        {
            get
            {
                return !finished;
            }
        }
        public bool finished;
        public bool paused;
        public bool onLoop;
        public float[] args;
        public event Action toDo;
        public event Action onFinished;
        public event Action onStop;
        public event Action onPause;
        public event Action onResume;
        public bool stoped = false;

        public TweenNodule()
        {
            finished = false;
            paused = false;
            args = new float[] { };
        }

        public void ATNUpdate()
        {
            if (!finished)
                toDo();
        }
        public void Pause()
        {
            if (onPause != null)
                onPause();

            paused = true;
        }

        public void Resume()
        {
            if (onResume != null)
                onResume();

            paused = false;
        }

        public void Stop()
        {
            if (onStop != null)
                onStop();

            stoped = true;
            finished = true;
            onLoop = false;
        }

        public void Dispatch(TweenEventType p_type)
        {
            switch (p_type)
            {
                case TweenEventType.FINISHED:
                    if (onFinished != null)
                        onFinished();
                    break;
                case TweenEventType.STOP:
                    if (onStop != null)
                        onStop();
                    break;
                case TweenEventType.PAUSE:
                    if (onPause != null)
                        onPause();
                    break;
                case TweenEventType.RESUME:
                    if (onResume != null)
                        onResume();
                    break;
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class TimerNodule
    {
        public bool isAlive
        {
            get
            {
                return !finished;
            }
        }
        public bool finished;
        public bool usingRealTime;
        public bool isLoopTimer;
        public float timer;
        public int repeatTime;
        public int framesToWait;
        public event Action function;
        public event Action onPaused;
        public event Action onResumed;
        public event Action onStop;
        public event Action onFinished;

        private bool _paused;
        private float _oldTime;
        private float _counter;
        private int _passedFrames;

        public TimerNodule()
        {
            finished = false;
            usingRealTime = false;
            isLoopTimer = false;
            _oldTime = Time.realtimeSinceStartup;
            _counter = 0;
            _passedFrames = 0;
            onPaused += delegate ()
            {
                _oldTime = Time.realtimeSinceStartup;
            };
        }

        public void ATNUpdate()
        {
            if (!finished && !_paused)
            {
                if (_passedFrames >= framesToWait)
                {
                    if (usingRealTime)
                    {
                        float __deltaTime = Time.realtimeSinceStartup - _oldTime;
                        _counter += __deltaTime;
                        _oldTime = Time.realtimeSinceStartup;
                    }
                    else
                        _counter += Time.deltaTime;

                    if (_counter >= timer)
                    {
                        if (function != null)
                            function();
                        else
                            Debug.LogWarning("No method was set to this timer. Nothing will happen.");

                        if (repeatTime > 1)
                        {
                            repeatTime--;
                            _counter = 0;
                        }
                        else
                        {
                            if (isLoopTimer)
                                _counter = 0;
                            else
                            {
                                finished = true;

                                if (onFinished != null)
                                    onFinished();

                                CoreTimer.aTimerCore.RemoveNodule(this);
                            }
                        }
                    }
                }
                else
                {
                    _passedFrames++;
                }
            }
        }
        
        public void Pause()
        {
            _paused = true;

            if (onPaused != null)
                onPaused();
        }
        
        public void Resume()
        {
            _paused = false;

            if (onResumed != null)
                onResumed();
        }
        
        public void Stop()
        {
            finished = true;

            if (onStop != null)
                onStop();

            CoreTimer.aTimerCore.RemoveNodule(this);
        }

        public void Antecipate()
        {
            if (function != null)
                function();
            else
                return;

            if (isLoopTimer)
                _counter = 0;
            else
            {
                finished = true;

                if (onFinished != null)
                    onFinished();

                CoreTimer.aTimerCore.RemoveNodule(this);
            }
        }
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    public class Timer
    {
        public static float realDeltaTime
        {
            get
            {
                return CoreTimer.aTimerCore.deltaTime;
            }
        }

        #region Wait Frames
        public static TimerNodule WaitFrames(int p_frames, Action p_method)
        {
            return TimedFunction(0, false, 0, p_frames, true, p_method);
        }
        #endregion


        #region Wait seconds    
        public static TimerNodule WaitSeconds(float p_time, Action p_method)
        {
            return WaitSeconds(p_time, true, p_method);
        }
        public static TimerNodule WaitSeconds(float p_time, bool p_useUnityTime, Action p_method)
        {
            return TimedFunction(p_time, false, 0, 0, p_useUnityTime, p_method);
        }
        #endregion

        #region Timed Function
        private static TimerNodule TimedFunction(float p_time, bool p_isLoopTimer, int p_repeatTime, int p_framesToWait, bool p_useUnityTime, Action p_method)
        {
            TimerNodule __nodule = new TimerNodule();
            
            __nodule.isLoopTimer = p_isLoopTimer;
            __nodule.function += p_method;
            __nodule.timer = p_time;
            __nodule.framesToWait = p_framesToWait;
            __nodule.repeatTime = p_repeatTime;
            __nodule.usingRealTime = !p_useUnityTime;
            
            CoreTimer.aTimerCore.AddNodule(__nodule);
            
            return CoreTimer.aTimerCore.GetPointerTo(__nodule);
        }
        #endregion
    }
}
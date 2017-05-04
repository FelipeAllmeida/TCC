using UnityEngine;
using System;
using System.Collections;

namespace Framework
{
    public enum AThreadStateType
    {
        NONE,
        ACTIVE,
        FINISHED,
        CANCELLED
    }

    public class AThread
    {
        public static AThreadNodule StartNewThread(Action p_threadMethod, Action p_threadFinishCallback = null)
        {
            AThreadNodule __threadNodule = new AThreadNodule();

            CoreAThread.instance.AddThreadNodule(__threadNodule);

            __threadNodule.StartThread(p_threadMethod, p_threadFinishCallback);

            return __threadNodule;
        }

        public static void RunMethodOnMainThread(Action p_method)
        {
            CoreAThread.instance.AddMainThreadMethod(p_method);
        }
    }
}
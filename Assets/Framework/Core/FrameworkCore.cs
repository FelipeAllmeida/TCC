using UnityEngine;
using System.Collections;
using System;

namespace Framework
{
    public class FrameworkCore : MonoBehaviour
    {
        private static GameObject _instance;
        public static GameObject instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("FrameworkCore", typeof(FrameworkCore));
                }
                return _instance;
            }
        }

        public event Action onAllCoresCleared;


        void Awake()
        {
            if (_instance != null)
            {
                if (_instance != this)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                _instance = this.gameObject;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public static void ClearAllCores(bool leaveASound)
        {
            if (_instance == null)
                return;

            CoreTimer timer = _instance.GetComponent<CoreTimer>();
            CoreTween tween = _instance.GetComponent<CoreTween>();

            if (timer != null)
            {
                Destroy(timer);
            }

            if (tween != null)
            {
                Destroy(tween);
            }
        }

        public static void ClearAllCoresASync(bool leaveASound)
        {
            FrameworkCore script = _instance.GetComponent<FrameworkCore>();
            script.StartCoroutine(script.ClearAllCoresAsyncFunction(leaveASound));
        }

        private IEnumerator ClearAllCoresAsyncFunction(bool leaveASound)
        {
            CoreTimer timer = GetComponent<CoreTimer>();
            yield return null;

            CoreTween tween = GetComponent<CoreTween>();
            yield return null;

            if (timer != null)
            {
                Destroy(timer);
                yield return null;
            }

            if (tween != null)
            {
                Destroy(tween);
                yield return null;
            }

            if (onAllCoresCleared != null)
                onAllCoresCleared();
        }

        public static Coroutine HostCoroutine(IEnumerator routine)
        {
            return _instance.GetComponent<FrameworkCore>().StartCoroutine(routine);
        }
    }
}
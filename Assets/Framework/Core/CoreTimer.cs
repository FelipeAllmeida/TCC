/*
 * Vers.: 1.2.12
 * Made by Ivan S. Cavalheiro (Unity 3D programmer)
 * This classe is part of Vox's Framework
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    public class CoreTimer : MonoBehaviour
    {

        private static CoreTimer _aTimerCore;
        public static CoreTimer aTimerCore
        {
            get
            {
                if (_aTimerCore == null)
                {
                    _aTimerCore = FrameworkCore.instance.AddComponent<CoreTimer>();
                }

                return _aTimerCore;
            }
        }

        public float deltaTime = 0;
        private float _lastFrameTime;
        public float lastFrameTime
        {
            get
            {
                return _lastFrameTime;
            }
        }

        private List<TimerNodule> _nodules = new List<TimerNodule>();

        void Awake()
        {
            _lastFrameTime = Time.realtimeSinceStartup;
            _aTimerCore = this;
        }

        void Update()
        {
            if (_nodules.Count > 0)
            {
                _nodules.RemoveAll(nodule => nodule == null);

                for (int i = 0;i < _nodules.Count;i++)
                {
                    if (_nodules[i].finished)
                    {
                        _nodules.Remove(_nodules[i]);
                        GC.SuppressFinalize(_nodules[i]);
                        i--;
                    }
                }

                for (int i = 0;i < _nodules.Count;i++)
                {
                    _nodules[i].ATNUpdate();
                }
            }
        }

        void LateUpdate()
        {
            deltaTime = Time.realtimeSinceStartup - _lastFrameTime;
            if (deltaTime < 0)
                deltaTime = 0;
            _lastFrameTime = Time.realtimeSinceStartup;
        }

        public TimerNodule GetPointerTo(TimerNodule p_nodule)
        {
            foreach (var nodule in _nodules)
            {
                if (nodule == p_nodule)
                    return p_nodule;
            }

            return null;
        }

        public void AddNodule(TimerNodule nodule)
        {
            if (_nodules == null)
                _nodules = new List<TimerNodule>();

            _nodules.Add(nodule);
        }

        public void RemoveNodule(TimerNodule nodule)
        {
            _nodules.Remove(nodule);
        }

    }
}
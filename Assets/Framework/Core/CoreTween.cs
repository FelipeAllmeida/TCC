using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class CoreTween : MonoBehaviour
    {
        private static CoreTween _aTweenInstance;
        public static CoreTween aTweenInstance
        {
            get
            {
                if (_aTweenInstance == null)
                {
                    _aTweenInstance = FrameworkCore.instance.AddComponent<CoreTween>();
                }
                return _aTweenInstance;
            }
        }

        private List<TweenNodule> _nodules = new List<TweenNodule>();

        void Start()
        {
            _aTweenInstance = this;
        }

        void Update()
        {
            for (int i = 0;i < _nodules.Count;i++)
            {
                if (_nodules[i].finished)
                {
                    if (!_nodules[i].stoped)
                        _nodules[i].Dispatch(TweenEventType.FINISHED);

                    _nodules.Remove(_nodules[i]);
                }
                else
                {
                    _nodules[i].ATNUpdate();
                }
            }
        }

        public void AddNodule(TweenNodule nodule)
        {
            if (_nodules == null)
                _nodules = new List<TweenNodule>();
            _nodules.Add(nodule);
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Framework
{
    public class Scene<T> : StateMachine<T>
    {
        private static Scene<T> _instance;
        public static Scene<T> instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Called during load time.
        /// </summary>
        public virtual void Awake()
        {
            //Defines singleton	
            if (instance != null)
            {
                Debug.LogError("Please, make sure you have only 1 AScene on the screen (Scene Manager).");
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        /// <summary>
        /// Called after the load time but before the first Update of the scene.
        /// </summary>
        public virtual void Start()
        {
            base.AInitialize();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public virtual void Update()
        {
            base.AUpdate();
        }

        /// <summary>
        /// Called every frame after the Update method.
        /// </summary>
        public virtual void LateUpdate()
        {
            base.ALateUpdate();
        }

        /// <summary>
        /// Called every Physics frame step.
        /// </summary>
        public virtual void FixedUpdate()
        {
            base.AFixedUpdate();
        }
    }
}
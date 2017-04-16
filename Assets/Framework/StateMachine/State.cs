using UnityEngine;
using System.Collections;

namespace Framework
{
    public enum StateType
    {
        NORMAL,
        PERSISTENT
    }

    public abstract class State<T> : MonoBehaviour 
    {
        public T state;

        [HideInInspector] public int priorityLevel = 0;

        [HideInInspector] public StateType type = StateType.NORMAL;

        private StateMachine<T> _manager;
        public StateMachine<T> stateMachine
        {
            get
            {
                return _manager;
            }
            set
            {
                _manager = value;
            }
        }

        public override string ToString()
        {
            return state.ToString();
        }

        /// <summary>
        /// Initialize the state.
        /// </summary>
        /// <remarks>
        /// This method is called during the load time or if the object is loaded at runtime its called during instantiation.
        /// </remarks>
        public virtual void AInitialize()
        {

        }

        /// <summary>
        /// Called every frame update.
        /// </summary>
        public virtual void AUpdate()
        {

        }

        /// <summary>
        /// Called every physics update.
        /// </summary>
        /// <remarks>
        /// It can be called once every 2 or 3 frames.<BR>
        /// Use this method only with physics modifications, like changing a rigidbody or a object with a collider.
        /// </remarks>
        public virtual void AFixedUpdate()
        {

        }

        /// <summary>
        /// Called after every frame update.
        /// </summary>
        public virtual void ALateUpdate()
        {

        }

        /// <summary>
        /// Called when manager enable this state.
        /// </summary>
        public virtual void Enable()
        {

        }

        /// <summary>
        /// Called when manager enable this state with a object as a enable parameter.
        /// </summary>
        public virtual void AOnEnable(object p_enableParameter)
        {

        }

        /// <summary>
        /// Called when scene disable this state.
        /// </summary>
        public virtual void Disable()
        {

        }

        /// <summary>
        /// Called on every GUI event.
        /// </summary>
        /// <remarks>
        /// Note that this method can be called more than once per frame.
        /// </remarks>
        public virtual void AOnGUI()
        {

        }
    }
}

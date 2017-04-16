using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Framework
{
    public abstract class StateMachine<T> : MonoBehaviour 
    {
        #region Public Data
        public T startState;
        #endregion

        #region Protected Data
        protected Dictionary<T, State<T>> _dictStates;

        protected T[] _arrayStatesType;

        protected List<State<T>> _listActivatedStates;
        #endregion

        #region Private Data
        private static bool _sceneHasStates = false;
        #endregion

        public virtual void AInitialize()
        {
            _listActivatedStates = new List<State<T>>();

            InitializeStatesDictionary();
            if (_sceneHasStates == false)
                return;
            InitializeStates();
            ChangeToState(startState);
        }

        private void InitializeStatesDictionary()
        {
            _dictStates = new Dictionary<T, State<T>>();

            Transform __transform = this.transform;
            if (__transform.FindChild("<States>") != null)
            {
                _sceneHasStates = true;
                State<T>[] __arrayStatesFound = __transform.GetComponentsInChildren<State<T>>();

                for (int i = 0;i < __arrayStatesFound.Length;i++)
                {
                    if (_dictStates.ContainsKey(__arrayStatesFound[i].state) == true)
                    {
                        Debug.LogError("You already have a state of type " + __arrayStatesFound[i].state + " in the scene. Please remove one.");
                    }
                    else
                    {
                        _dictStates.Add(__arrayStatesFound[i].state, __arrayStatesFound[i]);    
                    }
                }
                _arrayStatesType = new T[_dictStates.Count];
                _dictStates.Keys.CopyTo(_arrayStatesType, 0);
            }
            else
            {
                Debug.LogError("No states found in the scene");
            }
        }

        private void InitializeStates()
        {
            for (int i = 0; i < _dictStates.Count; i++)
            {
                _dictStates[_arrayStatesType[i]].stateMachine = this;
                _dictStates[_arrayStatesType[i]].AInitialize();
            }
        }

        protected virtual void OnStateActivated(State<T> p_activatedState)
        {
        }

        protected virtual void OnStateDeactivated(State<T> p_deactivatedState)
        {
        }

        public virtual void AUpdate()
        {
            for (int i = 0;i < _listActivatedStates.Count;i++)
            {
                _listActivatedStates[i].AUpdate();
            }
        }

        public virtual void ALateUpdate()
        {
            for (int i = 0;i < _listActivatedStates.Count;i++)
            {
                _listActivatedStates[i].ALateUpdate();
            }
        }

        public virtual void AFixedUpdate()
        {
            for (int i = 0;i < _listActivatedStates.Count;i++)
            {
                _listActivatedStates[i].AFixedUpdate();
            }
        }

        public virtual List<State<T>> GetActiveStates()
        {
            return _listActivatedStates;
        }

        public State<T> GetState(T p_stateType)
        {
            return _dictStates[p_stateType];
        }

        public virtual bool IsStateActivated(T p_stateType)
        {
            bool __found = false;

            foreach (var activeState in _listActivatedStates)
            {
                if (activeState.state.ToString() == p_stateType.ToString())
                    __found = true;
            }

            return __found;
        }

        public virtual void DisableState(T p_state)
        {
            if (_dictStates[p_state].type != StateType.PERSISTENT)
            {
                _dictStates[p_state].Disable();
                OnStateDeactivated(_dictStates[p_state]);
                if (_listActivatedStates.Contains(_dictStates[p_state]))
                {
                    _listActivatedStates.Remove(_dictStates[p_state]);
                }
            }
        }

        public virtual void EnableState(T p_state)
        {
            if (!_listActivatedStates.Contains(_dictStates[p_state]))
            {
                _listActivatedStates.Add(_dictStates[p_state]);
                _dictStates[p_state].Enable();
                OnStateActivated(_dictStates[p_state]);
            }
        }

        public virtual void ChangeToState(T p_state)
        {
            for (var i = 0;i < _dictStates.Count;i++)
            {
                DisableState(_arrayStatesType[i]);
            }
            EnableState(p_state);
        }
    }

}

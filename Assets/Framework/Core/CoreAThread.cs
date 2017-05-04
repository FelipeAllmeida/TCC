using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Framework;

public class CoreAThread : MonoBehaviour
{	    
	public List<AThreadNodule> _listThreadNodules = new List<AThreadNodule>();
	public List<Action> _listMainThreadMethods = new List<Action>();

	private static CoreAThread _instance;
	public static CoreAThread instance
	{
		get 
		{
			if(_instance == null ) 
			{
				_instance = FrameworkCore.instance.AddComponent<CoreAThread>() ;
			}

			return _instance; 
		}
	}


	void Start()
	{
		_instance = this;
	}
	 
	void Update()
	{
		for (int i = 0; i < _listMainThreadMethods.Count; i++)
		{
			_listMainThreadMethods[i]();

			_listMainThreadMethods.RemoveAt(i);

			i--;
		}

		for(int i = 0; i < _listThreadNodules.Count; i++)
		{
			switch (_listThreadNodules[i].threadState)
			{
				case AThreadStateType.FINISHED:
					if (_listThreadNodules[i].onThreadFinish != null) _listThreadNodules[i].onThreadFinish();
					_listThreadNodules.RemoveAt(i);
					i--;
					break;

				case AThreadStateType.CANCELLED:
					if (_listThreadNodules[i].onThreadCancel != null) _listThreadNodules[i].onThreadCancel();
					_listThreadNodules.RemoveAt(i);
					i--;
					break;
			}
		}
	}

	public void AddThreadNodule(AThreadNodule p_threadNodule)
	{
		if(_listThreadNodules == null)
		{
			_listThreadNodules = new List<AThreadNodule>();
		}

		_listThreadNodules.Add(p_threadNodule);
	}

	public void AddMainThreadMethod(Action p_method)
	{
		if (p_method == null) return;

		if (_listMainThreadMethods == null)
		{
			_listMainThreadMethods = new List<Action>();
		}

		_listMainThreadMethods.Add(p_method);
	}

	void OnApplicationQuit()
	{
		for(int i = 0; i < _listThreadNodules.Count; i++)
		{
			_listThreadNodules[i].CancelThread();
		}
	}
}
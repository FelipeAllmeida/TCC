using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    NONE,
    MOVE,
    GATHER,
    BUILD
}

public class CommandController : MonoBehaviour 
{
    #region Private-Serialize Data
    [SerializeField] private List<CommandType> _listCommandType;
    #endregion

    private CommandType _currentCommand;

    private Command _command;

    public void AUpdate()
    {
        if (_command != null)
        {
            _command.AUpdate();
        }
    }

    public void MoveTo(Entity p_actor, Vector3 p_targetPosition, Action p_callbackFinish = null)
    {
        if (_listCommandType.Contains(CommandType.MOVE) == true)
        {
            _command = new MoveCommand(p_actor, p_targetPosition, p_callbackFinish);
            _currentCommand = _command.Execute();
        }
    }

    public void GatherResource(Entity p_actor, Resource p_resource, Action p_callbackFinish = null)
    {
        if (_listCommandType.Contains(CommandType.GATHER) == true)
        {
            _command = new GatherCommand(p_actor, p_resource, p_callbackFinish);
            _currentCommand = _command.Execute();
        }
    }

    public void StopCurrentCommand()
    {
        _command.Stop();
        _currentCommand = CommandType.NONE;
    }

    public void SetListAvaiableCommands(List<CommandType> p_listCommandType)
    {
        _listCommandType = p_listCommandType;
    }

    public List<CommandType> GetListAvaiableCommands()
    {
        return _listCommandType;
    }
}

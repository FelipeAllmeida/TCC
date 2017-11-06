using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    NONE,
    MOVE,
    GATHER,
    ATTACK,
    BUILD
}

public class CommandController : MonoBehaviour 
{
    #region Events
    public Action onCurrentCommandFinish;
    #endregion

    #region Private-Serialize Data
    [SerializeField] private List<CommandType> _listCommandType;
    #endregion

    private CommandType _currentCommand;

    private Command _command;

    public void AInitialize()
    {
    }

    public void AUpdate()
    {
        if (_command != null)
        {
            _command.AUpdate();
        }
    }

    public void MoveTo(Entity p_actor, Vector3 p_targetPosition)
    {
        if (_listCommandType.Contains(CommandType.MOVE) == true)
        {
            _command = new MoveCommand(p_actor, p_targetPosition, onCurrentCommandFinish);
            _currentCommand = _command.Execute();
        }
    }

    public void GatherResource(Entity p_actor, Resource p_resource)
    {
        if (_listCommandType.Contains(CommandType.GATHER) == true)
        {
            Debug.Log("GatherResource");
            _command = new GatherCommand(p_actor, p_resource, onCurrentCommandFinish);
            _currentCommand = _command.Execute();
        }
    }

    public void AttackEntity(Entity p_actor, Entity p_other)
    {
        Debug.Log("AttackEntity");
        if (_listCommandType.Contains(CommandType.ATTACK) == true)
        {
            _command = new AttackCommand(p_actor, p_other);
            _currentCommand = _command.Execute();
        }
    }

    public CommandType GetCurrentCommand()
    {
        return _currentCommand;
    }

    public void StopCurrentCommand()
    {
        if (_command != null)
        {
            _command.Stop();
            _command = null;
        }

        if (onCurrentCommandFinish != null) onCurrentCommandFinish = null;

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

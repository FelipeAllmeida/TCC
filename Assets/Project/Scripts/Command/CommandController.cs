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
    public Action<CommandType> onCommandFinish;
    #endregion

    #region Private-Serialize Data
    [SerializeField] private List<CommandType> _listAvaiableCommands;
    #endregion

    private Dictionary<CommandType, Command> _dictCommands;

    public void AInitialize(List<CommandType> p_listCommandType)
    {
        _listAvaiableCommands = p_listCommandType;

        _dictCommands = new Dictionary<CommandType, Command>();
        for (int i = 0;i < _listAvaiableCommands.Count;i++)
        {
            _dictCommands.Add(_listAvaiableCommands[i], null);
        }
    }

    public void AUpdate()
    {
        if (_dictCommands != null)
        {
            foreach (Command __command in _dictCommands.Values)
            {
                if (__command != null)
                {
                    __command.AUpdate();
                }
            }
        }
    }

    public void MoveTo(Entity p_actor, Vector3 p_targetPosition)
    {
        if (_dictCommands.ContainsKey(CommandType.MOVE) == true)
        {
            if (_dictCommands[CommandType.MOVE] != null) _dictCommands[CommandType.MOVE].Stop();
            _dictCommands[CommandType.MOVE] = new MoveCommand(p_actor, p_targetPosition, onCommandFinish);
            _dictCommands[CommandType.MOVE].Execute();
        }
    }

    public void GatherResource(Entity p_actor, Resource p_resource)
    {
        if (_dictCommands.ContainsKey(CommandType.GATHER) == true)
        {
            if (_dictCommands[CommandType.GATHER] != null) _dictCommands[CommandType.GATHER].Stop();
            _dictCommands[CommandType.GATHER] = new GatherCommand(p_actor, p_resource, onCommandFinish);
            _dictCommands[CommandType.GATHER].Execute();
        }
    }

    public void AttackEntity(Entity p_actor, Entity p_other)
    {
        if (_dictCommands.ContainsKey(CommandType.ATTACK) == true)
        {
            if (_dictCommands[CommandType.ATTACK] != null) _dictCommands[CommandType.ATTACK].Stop();
            _dictCommands[CommandType.ATTACK] = new AttackCommand(p_actor, p_other, onCommandFinish);
            _dictCommands[CommandType.ATTACK].Execute();
        }
    }

    public void StopAllCommands()
    {
        if (_dictCommands != null)
        {
            for (int i = 0;i < _listAvaiableCommands.Count;i++)
            {
                if (_dictCommands[_listAvaiableCommands[i]] != null)
                {
                    _dictCommands[_listAvaiableCommands[i]].Stop();
                    _dictCommands[_listAvaiableCommands[i]] = null;
                }
            }
        }
    }

    public void StopTargetCommand(CommandType p_commandType)
    {
        if (_dictCommands != null && _dictCommands.ContainsKey(p_commandType) == true && _dictCommands[p_commandType] != null)
        {
            _dictCommands[p_commandType].Stop();
            _dictCommands[p_commandType] = null;
        }
    }

    public List<CommandType> GetListRunningCommands()
    {
        List<CommandType> __listRunningCommands = new List<CommandType>();
        foreach (var k in _dictCommands)
        {
            if (k.Value != null)
            {
                __listRunningCommands.Add(k.Key);
            }
        }
        return __listRunningCommands;
    }

    public List<CommandType> GetListAvaiableCommands()
    {
        return _listAvaiableCommands;
    }
}

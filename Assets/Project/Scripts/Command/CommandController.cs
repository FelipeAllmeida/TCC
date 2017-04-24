using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    NONE,
    MOVE,
    BUILD
}

public class CommandController : MonoBehaviour 
{
    #region Private-Serialize Data
    [SerializeField] private List<CommandType> _listCommandType;
    #endregion

    private CommandType _currentCommand;

    private Command _command;

    public void MoveTo(Entity p_actor, Vector3 p_targetPosition)
    {
        if (_listCommandType.Contains(CommandType.MOVE) == true)
        {
            _command = new MoveCommand(p_actor, p_targetPosition);
            _currentCommand = _command.Execute();
        }
    }

    public void StopCurrentCommand()
    {
        _command.Stop();
        _currentCommand = CommandType.NONE;
    }

    public List<CommandType> GetListAvaiableCommands()
    {
        return _listCommandType;
    }
}

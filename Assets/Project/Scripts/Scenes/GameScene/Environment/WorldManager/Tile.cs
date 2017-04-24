using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TyleType
{
    GROUND_TEST,
    NONE
}

public class Tile 
{
    public Tile(Vector2 p_position, TyleType p_tileType)
    {
        _position = p_position;
        _tileType = p_tileType;
        _width = 1;
        _height = 1;
    }

    private TyleType _tileType;
    private Vector2 _position;

    private float _width;
    private float _height;

    public bool IsUnitInsideTile(Entity p_unit)
    {
        return false;
    }
}

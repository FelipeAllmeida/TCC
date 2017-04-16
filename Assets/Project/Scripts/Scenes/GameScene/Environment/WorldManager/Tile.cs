using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TyleType
{
    GROUND_TEST,
    NONE
}

public class Tile : MonoBehaviour 
{
    public Tile(Vector2 p_position, TyleType p_tileType)
    {
    
    }

    private TyleType tileType;
    private Vector2 position;
}

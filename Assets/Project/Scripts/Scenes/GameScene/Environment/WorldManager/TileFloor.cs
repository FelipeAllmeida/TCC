using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFloor : MonoBehaviour 
{

    private int _floor;
    private Tile[,] _arrayFloorTiles;
    
    public void Initialize(int p_floor, Tile[,] p_arrayFloorTiles)
    {
        _floor = p_floor;
        _arrayFloorTiles = p_arrayFloorTiles;
    }

    public void SetTile(int p_x, int p_z, Tile p_tile)
    {
        _arrayFloorTiles[p_x, p_z] = p_tile;
        _arrayFloorTiles[p_x, p_z].gameObject.name = p_x + " | " + _floor + " | " + p_z;
        _arrayFloorTiles[p_x, p_z].transform.position = new Vector3(p_x, _floor, p_z);
    }

    public Tile GetTile(int p_x, int p_y)
    {
        return _arrayFloorTiles[p_x, p_y];
    }
}

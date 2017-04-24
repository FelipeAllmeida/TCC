using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFloor : MonoBehaviour 
{

    private int _floorIndex;
    private int _floorPositionY;
    private Tile[,] _arrayFloorTiles;
    
    public void Initialize(int p_floorIndex, int p_floorPositionY, Tile[,] p_arrayFloorTiles)
    {
        _floorIndex = p_floorIndex;
        _floorPositionY = p_floorPositionY;
        _arrayFloorTiles = p_arrayFloorTiles;
    }

    public void SetTile(TyleType p_tileType, int p_x, int p_z)
    {
        _arrayFloorTiles[p_x, p_z] = new Tile(new Vector2(p_x, p_z), p_tileType);
    }

    public Tile GetTile(int p_x, int p_y)
    {
        return _arrayFloorTiles[p_x, p_y];
    }

    public void AddEntityToFloor(Entity p_entity)
    {
        p_entity.transform.SetParent(transform);
    }
}

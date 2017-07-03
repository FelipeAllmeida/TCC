using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour 
{
    #region Const Data
    private const int _constMaxWorldHeight = 7;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private LocalNavMeshBuilder _localNavMeshBuilder;
    #endregion

    #region Private Data
    private static Dictionary<int, TileFloor> _dictWorldTiles;

    private int _worldWidth;
    private int _worldLenght;
    #endregion

    public void Initialize(int p_width, int p_height, int p_lenght)
    {    
        _worldWidth = p_width;
        _worldLenght = p_lenght;
        _dictWorldTiles = new Dictionary<int, TileFloor>();
        int __nextFloorPositionY = 0;
        for (int i = 0; i < _constMaxWorldHeight; i++)
        {
            GameObject __floor = new GameObject("Floor " + i);
            __floor.AddComponent<TileFloor>();
            __floor.transform.SetParent(this.transform);
            Tile[,] __arrayFloorTiles = new Tile[_worldWidth, _worldLenght];
            for (int j = 0; j < _worldWidth; j++)
            {
                for (int n = 0; n < _worldLenght; n++)
                {
                    __arrayFloorTiles[i, j] = null;
                }
            }
            __floor.GetComponent<TileFloor>().Initialize(i, __nextFloorPositionY, __arrayFloorTiles);
            _dictWorldTiles.Add(i, __floor.GetComponent<TileFloor>());
            __nextFloorPositionY += 3;
        }
        _localNavMeshBuilder.transform.position = new Vector3(0, 12.7f, 0);
        _localNavMeshBuilder.m_Size = new Vector3(p_width, __nextFloorPositionY, p_lenght);
    }

    public void BuildWorld()
    {
        for (int i = 0; i < _worldWidth;i++)
        {
            for (int j = 0; j < _worldLenght;j++)
            {
                _dictWorldTiles[0].SetTile(TyleType.GROUND_TEST, i, j);
            }
        }
    }

    public static void AddEntityToFloor(int p_floor, Entity p_entity)
    {
        if (_dictWorldTiles.ContainsKey(p_floor) == true)
        {
            _dictWorldTiles[p_floor].AddEntityToFloor(p_entity);
        }
    }
}

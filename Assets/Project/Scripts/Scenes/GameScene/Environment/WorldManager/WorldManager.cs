using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour 
{
    #region REMOVE
    public GameObject tile;
    #endregion

    #region Const Data
    private const int _constMaxWorldHeight = 7;
    #endregion

    #region Private Data
    private Dictionary<int, TileFloor> _dictWorldTiles;

    private int _worldWidth;
    private int _worldLenght;
    #endregion


    public void Initialize(int p_width, int p_lenght)
    {
        _worldWidth = p_width;
        _worldLenght = p_lenght;
        _dictWorldTiles = new Dictionary<int, TileFloor>();
        for (int i = 0; i < _constMaxWorldHeight; i++)
        {
            GameObject __floor = new GameObject("Flor " + i);
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
            __floor.GetComponent<TileFloor>().Initialize(i, __arrayFloorTiles);
            _dictWorldTiles.Add(i, __floor.GetComponent<TileFloor>());
        }
    }

    public void BuildWorld()
    {
        return;
        for (int i = 0; i < _worldWidth;i++)
        {
            for (int j = 0; j < _worldLenght;j++)
            {
                _dictWorldTiles[0].SetTile(i, j, CreateTile(_dictWorldTiles[0].transform));
            }
        }
    }

    private Tile CreateTile(Transform p_floorTransform)
    {
        Tile __tile = (Tile)Instantiate(tile, p_floorTransform).GetComponent<Tile>();
        __tile.gameObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        return __tile;
    }
}

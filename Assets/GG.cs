using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

/*
 * Você quer comprar um bolo, mas não lembra onde fica a confeitaria. Você lembra apenas
 * que, somando o número das casas (1, 2, 3 ...) do início da rua até a confeitaria, e da casa logo após a confeitaria
 * até o fim da rua, ambas as somas são iguais. Infelizmente, você não lembra a rua, e a cidade tem ruas muito
 * longas, com até 400 milhões de casas. Portanto, você precisa descobrir os possíveis números da confeitaria
 * para ruas de tamanho arbitrário (de 1 a 400 milhões). Por exemplo: a confeitaria poderia estar em uma rua de
 * tamanho 8, e seu endereço seria 6 (pois 1+2+3+4+5 = 7+8). Agora só falta encontrar as outras possibilidades
 */

public class GG : MonoBehaviour 
{
    Camera _camera;
    Terrain _terrain;
    int _numberOfTextures = 0;
    int _numberOfTexturesToUse = 3;
    bool _revertTextureOrder;

    List<string> _listTexturesUsed = new List<string>();

    void Start()
    {
        _camera = Camera.main;
        _terrain = GetComponent<Terrain>();
    }

    void Update()
    {
    
    }

    void OnGUI()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        {
           // GUI.backgroundColor = new Color(0f, 0f, 0f, 45f);
            GUI.color = Color.green;
                         
            if (GUILayout.Button("Build New World"))
            {
                BuildNewTerrain();
            }
            GUILayout.Label("Terrain Size: " + _terrain.terrainData.size.x + " | " + _terrain.terrainData.size.y + " | " + _terrain.terrainData.size.z);

            GUILayout.Space(20f);
            GUILayout.Label("Number of Textures to use: " + _numberOfTexturesToUse);
            GUILayout.Label("Number of Textures in use: " + _numberOfTextures);
            _revertTextureOrder = GUILayout.Toggle(_revertTextureOrder, new GUIContent("Revert Tex Order"));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Textures: ");

            for (int i = 0;i < _listTexturesUsed.Count;i++)
            {
                GUILayout.Label(_listTexturesUsed[i]);
            }
        }
        GUILayout.EndVertical();
    }

    private void BuildNewTerrain()
    {
        _camera.gameObject.SetActive(false);
        Texture2D[] __listLoadedTextures = Resources.LoadAll<Texture2D>("HeightMaps");
        List<Texture2D> __temp = __listLoadedTextures.ToList<Texture2D>();
        int __width = __listLoadedTextures[0].width;
        int __height = __listLoadedTextures[0].height;

        List<Texture2D> __listTextures = new List<Texture2D>();

        _numberOfTextures = 0;
        _listTexturesUsed.Clear();
        while (__listTextures.Count < _numberOfTexturesToUse)
        {
            int __randomIndex = Random.Range(0, __temp.Count);
            __listTextures.Add(__temp[__randomIndex]);
            _listTexturesUsed.Add(__temp[__randomIndex].name);
            __temp.RemoveAt(__randomIndex);
            _numberOfTextures++;
        }

        Texture2D __newTex = new Texture2D(__width, __height);

        for (int i = 0;i < __width;i++)
        {
            for (int j = 0;j < __height;j++)
            {
                Vector3 __colorVector = Vector3.zero;
                for (int k = 0;k < __listTextures.Count;k++)
                {
                    Color __color = __listTextures[k].GetPixel(i, j);
                    __colorVector += new Vector3(__color.r, __color.g, __color.b);
                }

                Color __newColor = new Color(__colorVector.x / __listTextures.Count, __colorVector.y / __listTextures.Count, __colorVector.z / __listTextures.Count, 1f);
                __newTex.SetPixel(i, j, __newColor);
            }
        }


        _terrain.terrainData.size = new Vector3(100, 10, 100);

        _terrain.terrainData.heightmapResolution = __newTex.width;
        float[,] __heightmap = new float[__newTex.width, __newTex.height];
        for (int i = 0;i < __newTex.width;i++)
        {
            for (int j = 0;j < __newTex.height;j++)
            {
                __heightmap[i, j] = ConvertColorToGrayScale(__newTex.GetPixel(i, j));
                
            }
        }

        for (int i = 0;i < _terrain.terrainData.size.x;i++)
        {
            for (int j = 0;j < _terrain.terrainData.size.y;j++)
            {
            
            }
        }

        Debug.Log("Finish, saved at: " + Application.dataPath);
        File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", __newTex.EncodeToPNG());

        
        _terrain.terrainData.SetHeights(0, 0, __heightmap);

        AssignSplatMap();

        _camera.gameObject.SetActive(true);
        _camera.transform.LookAt(new Vector3(_terrain.terrainData.size.x/2f, 0, _terrain.terrainData.size.z/2f));
    }

    private void AssignSplatMap()
    {
        // LoadTextures
        Texture2D[] __listTileTextures = Resources.LoadAll<Texture2D>("TileTextures");

        // Get a reference to the terrain data
        TerrainData __terrainData = _terrain.terrainData;

        SplatPrototype[] __splatPrototypes = new SplatPrototype[__listTileTextures.Length-1];

        if (_revertTextureOrder == true)
        {
            for (int i = __splatPrototypes.Length; i > 0 ; i--)
            {
                Debug.Log("i: " + i);
                __splatPrototypes[i-1] = new SplatPrototype();
                __splatPrototypes[i - 1].texture = __listTileTextures[i];
                __splatPrototypes[i - 1].tileSize = new Vector2(15f, 15f);
                __splatPrototypes[i - 1].tileOffset = Vector2.zero;
            }
        }
        else
        {
            for (int i = 0;i < __splatPrototypes.Length;i++)
            {
                Debug.Log("i: " + i);
                __splatPrototypes[i] = new SplatPrototype();
                __splatPrototypes[i].texture = __listTileTextures[i];
                __splatPrototypes[i].tileSize = new Vector2(15f, 15f);
                __splatPrototypes[i].tileOffset = Vector2.zero;
            }
        }
        
        __terrainData.splatPrototypes = __splatPrototypes;



        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[__terrainData.alphamapWidth, __terrainData.alphamapHeight, __terrainData.alphamapLayers];

        for (int y = 0;y < __terrainData.alphamapHeight;y++)
        {
            for (int x = 0;x < __terrainData.alphamapWidth;x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)__terrainData.alphamapHeight;
                float x_01 = (float)x / (float)__terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = __terrainData.GetHeight(Mathf.RoundToInt(y_01 * __terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * __terrainData.heightmapWidth));

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = __terrainData.GetInterpolatedNormal(y_01, x_01);

                // Calculate the steepness of the terrain
                float steepness = __terrainData.GetSteepness(y_01, x_01);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[__terrainData.alphamapLayers];
                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                // Texture[0] has constant influence
                splatWeights[0] = 0.5f;

                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01((__terrainData.heightmapHeight - height));

                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (__terrainData.heightmapHeight / 5.0f));

                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                splatWeights[3] = height * Mathf.Clamp01(normal.z);

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0;i < __terrainData.alphamapLayers;i++)
                {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        __terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private float ConvertColorToGrayScale(Color p_color)
    {
        
        return (p_color.r + p_color.g + p_color.b) / 3;
        
        //if (p_color.r >= p_color.g && p_color.r >= p_color.b)
        //{
        //    return p_color.r;
        //}
        //else if (p_color.g >= p_color.r && p_color.g >= p_color.b)
        //{
        //    return p_color.g;
        //}
        //else
        //{
        //    return p_color.b;
        //}
    }
}

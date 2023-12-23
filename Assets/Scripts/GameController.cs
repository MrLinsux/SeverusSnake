using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField]
    Tilemap area;
    [SerializeField]
    Tile[] defaultTiles;
    [SerializeField]
    Vector2Int defaultAreaSize = Vector2Int.zero;

    void Start()
    {
        area.SetTiles(GenerateAreaDefault(defaultAreaSize.x, defaultAreaSize.y, -new Vector3Int(defaultAreaSize.x/2, defaultAreaSize.y/2)), true);
    }

    TileChangeData[] GenerateAreaDefault(int wigth, int height, Vector3Int offset)
    {
        TileChangeData[] tiles = new TileChangeData[wigth*height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < wigth; x++)
            {
                var tile = defaultTiles[(x + y) % defaultTiles.Length];
                tiles[x+ wigth * y] = new TileChangeData(new Vector3Int(x, y)+offset, tile, tile.color, tile.transform);
            }
        }

        return tiles;
    }
}

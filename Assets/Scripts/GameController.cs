using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField]
    Tilemap area;
    [SerializeField]
    Tile[] defaultTiles;
    [SerializeField]
    Vector2Int defaultAreaSize = Vector2Int.zero;
    [SerializeField]
    GameObject apple;

    void Start()
    {
        area.SetTiles(GenerateAreaDefault(defaultAreaSize.x, defaultAreaSize.y, -new Vector3Int(defaultAreaSize.x/2, defaultAreaSize.y/2)), true);
        SpawnFood();
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

    public void SpawnFood()
    {
        if (Segment.snakeLen >= defaultAreaSize.x * defaultAreaSize.y * 0.8)
        {
            Debug.Log("You Win!");
            return;
        }
        var offset = new Vector2Int(defaultAreaSize.x / 2, defaultAreaSize.y / 2);
        bool isClear;
        Vector2 randPos;
        do
        {
            Debug.DrawLine((Vector2)(defaultAreaSize - 3*offset), (Vector2)(defaultAreaSize-offset-Vector2.one), Color.red, 10);
            var randX = UnityEngine.Random.Range((defaultAreaSize - 3 * offset).x, (defaultAreaSize - offset - Vector2Int.one).x);
            var randY = UnityEngine.Random.Range((defaultAreaSize - 3 * offset).y, (defaultAreaSize - offset - Vector2Int.one).y);
            randPos = new Vector2Int(randX, randY);
            var raycastRes = Physics2D.Raycast(randPos - new Vector2(0.4f, 0.4f), randPos + new Vector2(0.4f, 0.4f));
            isClear = !(raycastRes.transform != null && raycastRes.transform.CompareTag("Player"));
            Debug.DrawLine(new Vector2(randX - 0.4f, randY - 0.4f), new Vector2(randX + 0.4f, randY + 0.4f), Color.red, 10);
        }
        while (!isClear);

        Instantiate(apple, randPos, Quaternion.identity);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LayeredBiomeGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile deepWaterTile;
    public Tile waterTile;
    public Tile sandTile;
    public Tile grassTile;
    public Tile hillTile;
    public Tile mountainTile;
    public Tile snowTile;

    public int worldWidth = 100;
    public int worldHeight = 50;
    public float scale = 0.1f;

    void Start()
    {
        GenerateLayeredBiomes();
        Tilemap terrainTilemap = GameObject.Find("terrainTilemap").GetComponent<Tilemap>();
        Tilemap riverTilemap = GameObject.Find("riverTilemap").GetComponent<Tilemap>();
        GenerateRivers(terrainTilemap, riverTilemap, waterTile, worldWidth, worldHeight, scale);
    }

    void GenerateLayeredBiomes()
    {
        float offsetX = Random.Range(0f, 1000f);
        float offsetY = Random.Range(0f, 1000f);

        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        {
            for (int y = -worldHeight / 2; y < worldHeight / 2; y++)
            {
                float perlinValue = Mathf.PerlinNoise((x + offsetX) * scale, (y + offsetY) * scale);
                perlinValue = Mathf.SmoothStep(0.0f, 1.0f, perlinValue); 
                Tile chosenTile = ChooseTile(perlinValue);
                tilemap.SetTile(new Vector3Int(x, y, 0), chosenTile);
            }
        }
    }

    void GenerateRivers(Tilemap terrainTilemap, Tilemap riverTilemap, Tile riverTile, int worldWidth, int worldHeight, float scale)
    {
        float riverOffsetX = Random.Range(0f, 1000f);
        float riverOffsetY = Random.Range(0f, 1000f);

        Vector3Int start = new Vector3Int(-worldWidth / 2, Random.Range(-worldHeight / 4, worldHeight / 4), 0);
        Vector3Int currentPosition = start;

        for (int i = 0; i < worldWidth; i++)
        {
            float perlinValue = Mathf.PerlinNoise((currentPosition.x + riverOffsetX) * scale, (currentPosition.y + riverOffsetY) * scale);
            currentPosition.x++;

            if (perlinValue < 0.4)
            {
                currentPosition.y--;
            }
            else if (perlinValue > 0.6)
            {
                currentPosition.y++;
            }

            if (currentPosition.x >= -worldWidth / 2 && currentPosition.x < worldWidth / 2 &&
                currentPosition.y >= -worldHeight / 2 && currentPosition.y < worldHeight / 2)
            {
                HashSet<TileBase> impassableTiles = new HashSet<TileBase> { mountainTile, snowTile };

                TileBase terrainTile = terrainTilemap.GetTile(currentPosition);
                if (terrainTile != null && !impassableTiles.Contains(terrainTile))
                {
                    riverTilemap.SetTile(currentPosition, riverTile);
                }
            }
        }
    }

    Tile ChooseTile(float perlinValue)
    {
        if (perlinValue < 0.05f)
            return deepWaterTile;
        else if (perlinValue < 0.15f)
            return waterTile;
        else if (perlinValue < 0.2f)
            return sandTile;
        else if (perlinValue < 0.7f)
            return grassTile;
        else if (perlinValue < 0.85f)
            return hillTile;
        else if (perlinValue < 0.95f)
            return mountainTile;
        else
            return snowTile;
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LayeredBiomeGenerator : MonoBehaviour
{
    public Tilemap terrainTilemap;
    public Tilemap riverTilemap;
    public Tilemap roadTilemap;
    public Tilemap settlementTilemap;
    public Tile deepWaterTile;
    public Tile waterTile;
    public Tile sandTile;
    public Tile grassTile;
    public Tile hillTile;
    public Tile mountainTile;
    public Tile snowTile;
    public Tile settlementTile;
    public Tile roadTile;

    public int worldWidth = 100;
    public int worldHeight = 50;
    public float scale = 0.1f;

    private List<Vector3Int> settlements = new List<Vector3Int>();

    void Start()
    {
        GenerateLayeredBiomes();
        GenerateRivers(terrainTilemap, riverTilemap, waterTile, worldWidth, worldHeight, scale);
        GenerateSettlements(2);
        GenerateRoadsBetweenSettlements();
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
                terrainTilemap.SetTile(new Vector3Int(x, y, 0), chosenTile);
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

    void GenerateSettlements(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3Int location = PlaceSettlement(settlementTilemap);
            settlements.Add(location);
        }
    }

    Vector3Int PlaceSettlement(Tilemap settlementTilemap)
    {
        Vector3Int basePosition;
        int tilesCount = Random.Range(1, 4); 

        do
        {
            int x = Random.Range(-worldWidth / 2, worldWidth / 2);
            int y = Random.Range(-worldHeight / 2, worldHeight / 2);
            basePosition = new Vector3Int(x, y, 0);

            TileBase terrainTile = terrainTilemap.GetTile(basePosition);
            if (terrainTile != null && terrainTile != mountainTile && terrainTile != snowTile && terrainTile != deepWaterTile && terrainTile != waterTile)
            {
                settlementTilemap.SetTile(basePosition, settlementTile);
                int placedTiles = 1;

                while (placedTiles < tilesCount)
                {
                    Vector3Int offset = new Vector3Int(Random.Range(-1, 2), Random.Range(-1, 2), 0);
                    Vector3Int newPosition = basePosition + offset;

                    if (terrainTilemap.GetTile(newPosition) == grassTile || terrainTilemap.GetTile(newPosition) == sandTile)
                    {
                        settlementTilemap.SetTile(newPosition, settlementTile);
                        placedTiles++;
                    }
                }

                break;
            }
        } while (true);

        return basePosition;
    }

    void GenerateRoadsBetweenSettlements()
    {
        foreach (Vector3Int start in settlements)
        {
            foreach (Vector3Int end in settlements)
            {
                if (start != end)
                {
                    GenerateRoad(start, end);
                }
            }
        }
    }

    void GenerateRoad(Vector3Int start, Vector3Int end)
    {
        int x = start.x;
        int y = start.y;
        while (x != end.x || y != end.y)
        {
            if (x < end.x) x++;
            else if (x > end.x) x--;
            if (y < end.y) y++;
            else if (y > end.y) y--;
            roadTilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
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
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

    public int worldWidth = 500;
    public int worldHeight = 250;
    public float scale = 0.1f; 

    void Start()
    {
        GenerateLayeredBiomes();
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

    Tile ChooseTile(float perlinValue)
    {
        if (perlinValue < 0.1f)
            return deepWaterTile; 
        else if (perlinValue < 0.2f)
            return waterTile; 
        else if (perlinValue < 0.3f)
            return sandTile;
        else if (perlinValue < 0.7f)
            return grassTile; 
        else if (perlinValue < 0.85f)
            return hillTile; 
        else
            return mountainTile;
    }
}
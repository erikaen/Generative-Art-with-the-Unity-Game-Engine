using UnityEngine;

public class ContinentMap : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject cellPrefab;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;
    GameObject[,] cellObjects;
    Color[,] colors;

    float updateInterval = 0.6f;
    float nextColorUpdateTime;
    float nextUpdateTime;

    void Start()
    {
        map = new int[width, height];
        colors = new Color[width, height];
        cellObjects = new GameObject[width, height];

        GenerateMap();
        nextColorUpdateTime = Time.time + updateInterval;
        nextUpdateTime = Time.time + updateInterval;
    }

    void Update()
    {
        if (Time.time >= nextColorUpdateTime)
        {
            UpdateColors();
            nextColorUpdateTime = Time.time + updateInterval;
        }
        if (Time.time >= nextUpdateTime)
        {
            EvolveMap();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    // Increase the wall count if it's out of bounds - assuming out of bounds is always a wall
                    wallCount++;
                }
            }
        }
        return wallCount;
    }


    void SmoothMap()
    {
        int[,] newMap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (map[x, y] == 1 && neighbourWallTiles >= 4)
                    newMap[x, y] = 1;
                else if (map[x, y] == 0 && neighbourWallTiles > 5)
                    newMap[x, y] = 1;
                else
                    newMap[x, y] = 0;
            }
        }
        map = newMap; // Update the map after smoothing
    }

    void GenerateMap()
    {
        RandomFillMap();
        InitializeColors();
        CreateCellObjects();

        for (int i = 0; i < 10; i++)
        {
            SmoothMap();
            UpdateCellObjects();
        }
    }

    void UpdateCellObjects()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cellObjects[x, y].GetComponent<Renderer>().material.color = colors[x, y] = GetRandomPastelColor(map[x, y] == 1);
            }
        }
    }


    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }


    void InitializeColors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colors[x, y] = map[x, y] == 1 ? GetRandomPastelColor(true) : GetRandomPastelColor(false);
            }
        }
    }


    void CreateCellObjects()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cellObjects[x, y] = cell;
                cell.GetComponent<Renderer>().material.color = colors[x, y];
            }
        }
    }

    void UpdateColors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (UnityEngine.Random.Range(0, 100) < 20) // 20% chance to change color
                {
                    colors[x, y] = GetRandomPastelColor(map[x, y] == 1);
                    cellObjects[x, y].GetComponent<Renderer>().material.color = colors[x, y];
                }
            }
        }
    }

    Color GetRandomPastelColor(bool isWall)
    {
        float r = UnityEngine.Random.Range(0.7f, 1.0f);
        float g = UnityEngine.Random.Range(0.7f, 1.0f);
        float b = UnityEngine.Random.Range(0.7f, 1.0f);
        return new Color(r * (isWall ? 0.5f : 1), g * (isWall ? 0.5f : 1), b * (isWall ? 0.5f : 1));
    }

    void EvolveMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int currentCell = map[x, y];
                if (UnityEngine.Random.Range(0, 100) < 10) // 10% chance to change
                {
                    map[x, y] = (currentCell == 1) ? 0 : 1;
                    colors[x, y] = GetRandomPastelColor(map[x, y] == 1);
                    cellObjects[x, y].GetComponent<Renderer>().material.color = colors[x, y];
                }
            }
        }
    }
}

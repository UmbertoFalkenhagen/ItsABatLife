using UnityEngine;

public class ProceduralMaze : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab; // Prefab for the wall segments
    public Material mat;
    public float wallSize = 1f;

    private int[,] maze;

    void Start()
    {
        GenerateMaze();
        BuildMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // Initialize maze with walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1; // 1 means wall, 0 means path
            }
        }

        // Carve out paths using a simple random walk algorithm
        int currentX = 1;
        int currentY = 1;
        maze[currentX, currentY] = 0; // Start point

        System.Random rand = new System.Random();
        for (int i = 0; i < (width * height); i++)
        {
            int direction = rand.Next(4);

            switch (direction)
            {
                case 0: // Up
                    if (currentY - 2 > 0)
                    {
                        if (maze[currentX, currentY - 2] == 1)
                        {
                            maze[currentX, currentY - 1] = 0;
                            maze[currentX, currentY - 2] = 0;
                            currentY -= 2;
                        }
                    }
                    break;
                case 1: // Down
                    if (currentY + 2 < height - 1)
                    {
                        if (maze[currentX, currentY + 2] == 1)
                        {
                            maze[currentX, currentY + 1] = 0;
                            maze[currentX, currentY + 2] = 0;
                            currentY += 2;
                        }
                    }
                    break;
                case 2: // Left
                    if (currentX - 2 > 0)
                    {
                        if (maze[currentX - 2, currentY] == 1)
                        {
                            maze[currentX - 1, currentY] = 0;
                            maze[currentX - 2, currentY] = 0;
                            currentX -= 2;
                        }
                    }
                    break;
                case 3: // Right
                    if (currentX + 2 < width - 1)
                    {
                        if (maze[currentX + 2, currentY] == 1)
                        {
                            maze[currentX + 1, currentY] = 0;
                            maze[currentX + 2, currentY] = 0;
                            currentX += 2;
                        }
                    }
                    break;
            }
        }
    }

    void BuildMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 1)
                {
                    Vector3 position = new Vector3(x * wallSize, 0, y * wallSize);
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int width = 21;   // Chiều rộng (số lẻ để dễ tạo đối xứng)
    public int height = 21;  // Chiều cao (số lẻ để dễ tạo đối xứng)

    public Tilemap tilemap;         // Tilemap chính
    public Tile wallTile;           // Tile cho tường
    public RuleTile pelletTile;         // Tile cho viên thức ăn
    public GameObject pacmanPrefab;
    private List<Vector3> pelletPositions = new List<Vector3>();

    private int[,] map; // 0 = đường đi, 1 = tường

    void Start()
    {
        GenerateMap();
        DrawMap();
        PlacePacman();
    }

    // 1️⃣ Tạo mê cung ngẫu nhiên
    public void GenerateMap()
    {
        map = new int[width, height];

        // Khởi tạo toàn bộ là tường
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // //ghost house (8,8) (8,10) (12,8) (12,12)
        map[8, 8] = 1;
        map[8, 10] = 1;
        map[12, 8] = 1;
        map[12, 12] = 1;

        // Tạo đường đi bằng thuật toán đệ quy
        CarvePath(1, 1);

        // Đối xứng bản đồ để tạo cảm giác quen thuộc như Pac-Man
        MirrorMap();

        for (int j = 0;j < height; j++)
        {
            map[0, j] = 1;
            map[20, j] = 1;
        }

        for (int i = 0; i < height; i++)
        {
            map[i, 0] = 1;
            map[i, 20] = 1;
        }

        for (int j = 1; j < height-1; j++)
        {
            map[1, j] = 0;
            map[19, j] = 0;
        }
        for (int j = 1; j < height - 1; j++)
        {
            map[j, 19] = 0;
        }

        //ghost house
        for (int i = 7; i<=13; i++)
        for(int j = 7; j<=11; j++)
        {
                map[i, j] = 2;
                if (((i >=8 && i<=12) &&(j ==8 || j ==10)) || (i ==8 && j ==9) || (i == 12 && j == 9))
                {
                    map[i, j] = 1;
                }
        }
        map[10, 10] = 2;

    }

    // 2️⃣ Đào đường đi chính
    void CarvePath(int x, int y)
    {
        map[x, y] = 0;  // Đánh dấu là đường đi

        // Hướng di chuyển: lên, xuống, trái, phải (random thứ tự)

       
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 2),
            new Vector2Int(2, 0),
            new Vector2Int(0, -2),
            new Vector2Int(-2, 0)
        };
        List<Vector2Int> addMore = new List<Vector2Int>
        {
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1)
        };
        Shuffle(directions);  // Random hướng đi để tạo tính ngẫu nhiên
        Shuffle(addMore);
        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            if (IsInBounds(nx, ny) && map[nx, ny] == 1)
            {
                if (Random.Range(0, 100) <60)
                {
                    map[x + dir.x / 2, y + dir.y / 2] = 0;
                }
                    map[nx, ny + addMore[0].y] = 0;
                    map[nx + addMore[1].x, ny] = 0;
                CarvePath(nx, ny);                   
            }
        }

            
    }

    // 4️⃣ Kiểm tra giới hạn bản đồ
    bool IsInBounds(int x, int y)
    {
        return x > 0 && x < width - 1 && y > 0 && y < height - 1;
    }

    // 5️⃣ Tạo đối xứng bản đồ (theo trục dọc)
    void MirrorMap()
    {
        for (int x = 0; x < width / 2; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[width - 1 - x, y] = map[x, y];
            }
        }
    }

    // 6️⃣ Vẽ bản đồ lên scene
    public void DrawMap()
    {
        tilemap.ClearAllTiles(); // Xóa tile cũ trước khi vẽ mới

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (map[x, y] == 1)
                {
                    tilemap.SetTile(tilePosition, wallTile); // Đặt tường
                }
                else if (map[x, y] == 0)
                {
                    tilemap.SetTile(tilePosition, pelletTile); // Đặt thức ăn
                    pelletPositions.Add(tilePosition);
                }
            }
        }
    }

    // Hàm trộn danh sách để random hướng đi
    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void PlacePacman()
    {
        if (pelletPositions.Count > 0)
        {
            Vector3Int pacmanTilePos = Vector3Int.FloorToInt(pelletPositions[Random.Range(0, pelletPositions.Count)]);

            tilemap.SetTile(pacmanTilePos, null);

            pelletPositions.Remove(pacmanTilePos);

            Instantiate(pacmanPrefab, pacmanTilePos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        }
    }

}

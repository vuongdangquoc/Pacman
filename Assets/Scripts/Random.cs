using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public int width = 31;   // Chiều rộng (số lẻ để dễ tạo đối xứng)
    public int height = 31;  // Chiều cao (số lẻ để dễ tạo đối xứng)

    public Tilemap walls;         // Tilemap chính
    public Tilemap pellets;         // Tilemap pellet
    public Tilemap nodes;         // Tilemap pellet

    public Tile wallTile;           // Tile cho tường
    public RuleTile pelletTile;         // Tile cho viên thức ăn
    public RuleTile node;
    public GameObject pacmanPrefab;
    public GameObject nodePrefab;
    private List<Vector3> pelletPositions = new List<Vector3>();
    public Ghost[] ghosts;
    private int[,] map; // 0 = đường đi, 1 = tường

    void Start()
    {
        GenerateMap();
        PlaceGhostHouse();
        DrawMap();
        PlacePacman();
        PlaceNodes();
        
    }
    // 1️⃣ Tạo mê cung ngẫu nhiên
    public void GenerateMap()
    {
        map = new int[width, height];

        // Khởi tạo toàn bộ là tường
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // Tạo đường đi bằng thuật toán đệ quy
        CarvePath(1, 1);

        // Đối xứng bản đồ để tạo cảm giác quen thuộc như Pac-Man
        MirrorMap();

        for (int j = 0; j < height; j++)
        {
            map[0, j] = 1;
            map[30, j] = 1;
        }

        for (int i = 0; i < height; i++)
        {
            map[i, 0] = 1;
            map[i, 30] = 1;
        }

        for (int j = 1; j < height - 1; j++)
        {
            map[1, j] = 0;
            map[29, j] = 0;
        }
        for (int j = 1; j < height - 1; j++)
        {
            map[j, 29] = 0;
        }     
        
        PlaceGhostHouse();

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
                if (Random.Range(0, 100) < 60)
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
        walls.ClearAllTiles(); // Xóa tile cũ trước khi vẽ mới
        pellets.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (map[x, y] == 1)
                {
                    walls.SetTile(tilePosition, wallTile); // Đặt tường
                }
                else if (map[x, y] == 0)
                {
                    pellets.SetTile(tilePosition, pelletTile); // Đặt thức ăn
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

    //tim diem dat pacman
    public void PlacePacman()
    {
        if (pelletPositions.Count > 0)
        {
            Vector3Int pacmanTilePos = Vector3Int.FloorToInt(pelletPositions[Random.Range(0, pelletPositions.Count)]);
            walls.SetTile(pacmanTilePos, null);
            pelletPositions.Remove(pacmanTilePos);
            Debug.Log(pacmanTilePos);
            var newPacman = Instantiate(pacmanPrefab, pacmanTilePos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            GameManager.Instance.SetPacman(newPacman.GetComponent<Pacman>());
            foreach (Ghost ghost in ghosts)
            {
                ghost.SetPacman(newPacman);
            }
        }
    }


    //ghost house
    public void PlaceGhostHouse()
    {
        //ghost house
        for (int j = 12; j <= 17; j++)
            for (int i = 12; i <= 18; i++)
            {
                map[i, j] = 2;
                if (((i >= 13 && i <= 17) && (j == 13 || j == 16)) || ((j == 14 || j == 15) && (i == 13 || i == 17)))
                {
                    map[i, j] = 1;
                }
            }
    }

    //tim diem dat node
    public void PlaceNodes()
    {
        nodes.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                    if (map[x, y] != 1)
                    {
                        Vector3Int position = new Vector3Int(x, y, 0);

                        // Kiểm tra nếu vị trí là ngã ba/ngã tư
                        if (IsIntersection(position))
                        {
                            nodes.SetTile(position, node);                          
                        }

                    }

            }
        }
    }

    // Kiểm tra xem vị trí có phải ngã re
    private bool IsIntersection(Vector3Int position)
    {
        if (map[position.x + 1, position.y] != 1)
        {
            if (map[position.x, position.y + 1] != 1)
            {
                return true;
            }
            else if (map[position.x, position.y - 1] != 1)
            {
                return true;
            }
        }

        if (map[position.x - 1, position.y] != 1)
        {
            if (map[position.x, position.y + 1] != 1)
            {
                return true;
            }
            else if (map[position.x, position.y - 1] != 1)
            {
                return true;
            }
        }
        return false;
    }
}

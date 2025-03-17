using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int width = 31;   // Chiều rộng (số lẻ để dễ tạo đối xứng)
    public int height = 31;  // Chiều cao (số lẻ để dễ tạo đối xứng)

    public Tilemap walls;         // Map tường
    public Tilemap pellets;       // Map viên pellet
    public Tilemap nodes;         // Map cho các node
    public Tilemap diamonds;      // Map cho kim cương

    public Tile wallTile;           // Tile cho tường
    public RuleTile pelletTile;         // Tile cho viên thức ăn
    public RuleTile powerPelletTile;         // Tile cho power viên thức ăn
    public RuleTile diamondTile;
    public RuleTile node;
    public GameObject pacmanPrefab;
    public GameObject fruitPrefab;
    private List<Vector3Int> pelletPositions = new List<Vector3Int>();
    public Ghost[] ghosts;
    private int[,] map; // mảng 2 chiều cho đường đi và tường. recommend để là boolean, true là tường, false là đường đi.

    public void GenerateAll()
    {
        GenerateMap();
        PlaceGhostHouse();
        DrawMap();
        PlacePacman();
        PlaceNodes();
        PlaceDiamondPellets();
        PlaceFruit();
    }

    // 1. Tạo mê cung ngẫu nhiên
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

        // Tạo tường bao quanh
        for (int i = 0; i < height; i++)
        {
            map[0, i] = 1;
            map[i, 0] = 1;
            map[30, i] = 1;
            map[i, 30] = 1;
        }

        for (int j = 1; j < height - 1; j++)
        {
            map[1, j] = 0;
            map[29, j] = 0;
            map[j, 29] = 0;
        }

        PlaceGhostHouse();

    }

    // 2. Đào đường đi chính
    void CarvePath(int x, int y)
    {
        map[x, y] = 0;  // Đánh dấu là đường đi

        // Hướng di chuyển: lên, xuống, trái, phải (random thứ tự)

        // problem: tại sao là 2 mà không phải là 1?
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

    // 3. Kiểm tra giới hạn bản đồ
    bool IsInBounds(int x, int y)
    {
        return x > 0 && x < width - 1 && y > 0 && y < height - 1;
    }

    // 4. Tạo đối xứng bản đồ (theo trục dọc)
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

    // 6. Vẽ bản đồ
    public void DrawMap()
    {
        //Xóa hết các tile trên bản đồ
        walls.ClearAllTiles();
        pellets.ClearAllTiles();
        pelletPositions.Clear();
        nodes.ClearAllTiles();
        diamonds.ClearAllTiles();
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

    // Random list các hướng đi
    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            Vector2Int temp = list[i];
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
            pellets.SetTile(pacmanTilePos, null);
            pelletPositions.Remove(pacmanTilePos);
            var newPacman = Instantiate(pacmanPrefab, pacmanTilePos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            GameManager.Instance.SetPacman(newPacman.GetComponent<Pacman>());
            LightSystem.Instance.SetPacman(newPacman.GetComponent<Pacman>());
            foreach (Ghost ghost in ghosts)
            {
                ghost.SetPacman(newPacman);
            }
        }
    }


    //5. Nhà của Pacman
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

    public void PlaceDiamondPellets()
    {
        List<Vector3Int>[] quadrants = new List<Vector3Int>[4]
        {
        new List<Vector3Int>(), // Góc trên trái
        new List<Vector3Int>(), // Góc trên phải
        new List<Vector3Int>(), // Góc dưới trái
        new List<Vector3Int>()  // Góc dưới phải
        };

        Vector3Int center = new Vector3Int(width / 2, height / 2, 0);

        // Chia pelletPositions thành 4 phần bản đồ
        foreach (var pos in pelletPositions)
        {
            if (pos.x < center.x && pos.y >= center.y)
                quadrants[0].Add(pos); // Góc trên trái
            else if (pos.x >= center.x && pos.y >= center.y)
                quadrants[1].Add(pos); // Góc trên phải
            else if (pos.x < center.x && pos.y < center.y)
                quadrants[2].Add(pos); // Góc dưới trái
            else
                quadrants[3].Add(pos); // Góc dưới phải
        }

        // Đặt 3 diamond trong mỗi góc
        for (int i = 0; i < 4; i++)
        {
            if (quadrants[i].Count > 0)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3Int diamondPos = quadrants[i][Random.Range(0, quadrants[i].Count)];
                    diamonds.SetTile(diamondPos, diamondTile); // Đặt viên đặc biệt
                    quadrants[i].Remove(diamondPos);
                    pelletPositions.Remove(diamondPos); // Xóa khỏi danh sách viên thức ăn thường
                }

                Vector3Int powerPelletPos = quadrants[i][Random.Range(0, quadrants[i].Count)];
                pellets.SetTile(powerPelletPos, powerPelletTile);
                pelletPositions.Remove(powerPelletPos);
            }
        }
    }

    public void PlaceFruit()
    {
        if (pelletPositions.Count > 0)
        {
            Vector3Int pacmanTilePos = Vector3Int.FloorToInt(pelletPositions[Random.Range(0, pelletPositions.Count)]);
            pellets.SetTile(pacmanTilePos, null);
            pelletPositions.Remove(pacmanTilePos);
            var newFruit = Instantiate(fruitPrefab, pacmanTilePos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            GameManager.Instance.SetFruit(newFruit.GetComponent<Fruit>());
        }
    }
}

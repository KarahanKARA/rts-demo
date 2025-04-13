using Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeField] private Grid layoutGrid;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tilemap overlayTilemap;
        [SerializeField] private GameSettings gameSettings;

        private bool[,] occupied;
        private bool[,] buildingGrid;
        private Camera mainCam;

        public Grid LayoutGrid => layoutGrid;
        public int GridWidth => gameSettings.gridWidth;
        public int GridHeight => gameSettings.gridHeight;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            Instance = this;
            mainCam = Camera.main;
        }

        private void Start()
        {
            GenerateGrid();
            CenterCamera();
            DrawGridOverlay();
        }

        private void GenerateGrid()
        {
            tilemap.ClearAllTiles();
            overlayTilemap.ClearAllTiles();
            occupied = new bool[gameSettings.gridWidth, gameSettings.gridHeight];
            buildingGrid = new bool[gameSettings.gridWidth, gameSettings.gridHeight];

            for (int x = 0; x < gameSettings.gridWidth; x++)
            {
                for (int y = 0; y < gameSettings.gridHeight; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), gameSettings.gridTile);
                }
            }
        }
        
        private void CenterCamera()
        {
            float gridWidth = gameSettings.gridWidth;
            float gridHeight = gameSettings.gridHeight;

            mainCam.transform.position = new Vector3(gridWidth / 2f, gridHeight / 2f, -10f);

            float visibleScreenWidth = Screen.width * 0.55f;
            float screenHeight = Screen.height;

            float visibleAspect = visibleScreenWidth / screenHeight;

            float gridAspect = gridWidth / gridHeight;

            if (visibleAspect >= gridAspect)
            {
                mainCam.orthographicSize = gridHeight / 2f + 1f;
            }
            else
            {
                mainCam.orthographicSize = (gridWidth / visibleAspect) / 2f + 1f;
            }
        }


        public void DrawGridOverlay()
        {
            overlayTilemap.ClearAllTiles();

            for (int x = 0; x < gameSettings.gridWidth; x++)
            {
                for (int y = 0; y < gameSettings.gridHeight; y++)
                {
                    overlayTilemap.SetTile(new Vector3Int(x, y, 0), gameSettings.outlineTile);
                }
            }
        }

        public Vector3Int GetBottomLeftCell(Vector3Int centerCell, Vector2Int size)
        {
            int offsetX = Mathf.FloorToInt(size.x / 2f);
            int offsetY = Mathf.FloorToInt(size.y / 2f);
            return new Vector3Int(centerCell.x - offsetX, centerCell.y - offsetY, 0);
        }

        public Vector3 GetSnappedPosition(Vector3Int centerCell, Vector2Int size)
        {
            Vector3Int bottomLeftCell = GetBottomLeftCell(centerCell, size);
            Vector3 bottomLeftWorld = layoutGrid.CellToWorld(bottomLeftCell);
            return bottomLeftWorld + new Vector3(size.x / 2f, size.y / 2f, 0f);
        }

        public bool IsAreaFree(Vector3Int centerCell, Vector2Int size)
        {
            Vector3Int bottomLeft = GetBottomLeftCell(centerCell, size);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int gx = bottomLeft.x + x;
                    int gy = bottomLeft.y + y;

                    if (!IsCellInBounds(gx, gy) || buildingGrid[gx, gy])
                        return false;
                }
            }

            return true;
        }

        public void OccupyArea(Vector3Int centerCell, Vector2Int size)
        {
            Vector3Int bottomLeft = GetBottomLeftCell(centerCell, size);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int gx = bottomLeft.x + x;
                    int gy = bottomLeft.y + y;

                    if (IsCellInBounds(gx, gy))
                    {
                        buildingGrid[gx, gy] = true;
                        occupied[gx, gy] = true;
                    }
                }
            }
        }

        public void FreeArea(Vector3 worldPos, Vector2Int size)
        {
            Vector3Int centerCell = layoutGrid.WorldToCell(worldPos);
            Vector3Int bottomLeft = GetBottomLeftCell(centerCell, size);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int gx = bottomLeft.x + x;
                    int gy = bottomLeft.y + y;

                    if (IsCellInBounds(gx, gy))
                    {
                        buildingGrid[gx, gy] = false;
                        occupied[gx, gy] = false;
                    }
                }
            }
        }

        public bool IsCellInBounds(int x, int y)
        {
            return x >= 0 && x < GridWidth && y >= 0 && y < GridHeight;
        }

        public bool IsCellOccupied(Vector3Int cell)
        {
            if (!IsCellInBounds(cell.x, cell.y)) return true;
            return buildingGrid[cell.x, cell.y];
        }

        public bool[,] GetOccupiedGrid() => buildingGrid;
    }
}

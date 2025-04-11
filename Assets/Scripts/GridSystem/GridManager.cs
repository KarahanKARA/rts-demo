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

        private bool[,] _occupied;
        private Camera _mainCam;

        public Grid LayoutGrid => layoutGrid;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            Instance = this;
            _mainCam = Camera.main;
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
            _occupied = new bool[gameSettings.gridWidth, gameSettings.gridHeight];

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
            float w = gameSettings.gridWidth;
            float h = gameSettings.gridHeight;
            _mainCam.transform.position = new Vector3(w / 2f, h / 2f, -10f);
            _mainCam.orthographicSize = Mathf.Max(w, h) / 2f + 1;
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
            int offsetX = size.x % 2 == 0 ? (size.x / 2 - 1) : (size.x / 2);
            int offsetY = size.y % 2 == 0 ? (size.y / 2 - 1) : (size.y / 2);
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

                    if (gx < 0 || gx >= _occupied.GetLength(0) || gy < 0 || gy >= _occupied.GetLength(1))
                        return false;

                    if (_occupied[gx, gy])
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

                    if (gx >= 0 && gx < _occupied.GetLength(0) && gy >= 0 && gy < _occupied.GetLength(1))
                        _occupied[gx, gy] = true;
                }
            }
        }
        
        public bool[,] GetOccupiedGrid()
        {
            return (bool[,])_occupied.Clone();
        }
    }
}

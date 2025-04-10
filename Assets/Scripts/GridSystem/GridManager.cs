using Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Grid layoutGrid; 
        
        private Camera _gameCamera;
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
            
            _gameCamera = Camera.main;
        }

        void Start()
        {
            GenerateGrid();
            AdjustCameraToGrid();
        }

        private void GenerateGrid()
        {
            tilemap.ClearAllTiles();

            for (int x = 0; x < gameSettings.gridWidth; x++)
            {
                for (int y = 0; y < gameSettings.gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePos, gameSettings.gridTile);
                }
            }
        }

        private void AdjustCameraToGrid()
        {
            var width = gameSettings.gridWidth * gameSettings.cellSize;
            var height = gameSettings.gridHeight * gameSettings.cellSize;

            _gameCamera.transform.position = new Vector3(width / 2f, height / 2f, -10f);
            var screenRatio = (float)Screen.width / Screen.height;
            var targetRatio = width / height;

            if (screenRatio >= targetRatio)
            {
                _gameCamera.orthographicSize = height / 2f + 1;
            }
            else
            {
                _gameCamera.orthographicSize = (width / screenRatio) / 2f + 1;
            }
        }
    }
}
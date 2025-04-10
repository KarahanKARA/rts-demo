using Core.Factories;
using Data.Buildings;
using GridSystem;
using UnityEngine;

namespace Managers
{
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer previewRenderer;
        [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.5f);
        [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

        private GameObject currentGhost;
        private IBuildingFactory buildingFactory;
        private Camera cam;
        private BaseBuildingData currentData;
        
        private void Start()
        {
            cam = Camera.main;
            buildingFactory = new BuildingFactory();
        }

        public void StartPlacing(BaseBuildingData data)
        {
            currentData = data;
            previewRenderer.sprite = data.icon;
            previewRenderer.transform.localScale = GetScaleFromSize(data.size);
            previewRenderer.gameObject.SetActive(true);
        }
        
        private void Update()
        {
            if (currentData == null) return;

           
            
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = GridManager.Instance.layoutGrid.WorldToCell(mousePos);
            Vector3 worldPos = GridManager.Instance.layoutGrid.CellToWorld(cellPos);

            previewRenderer.transform.position = worldPos;

            bool isValid = IsPlacementValid(cellPos, currentData.size);
            previewRenderer.color = isValid ? validColor : invalidColor;

            if (isValid && Input.GetMouseButtonDown(0))
            {
                Vector3 spawnPos = worldPos + new Vector3(currentData.size.x / 2f - 0.5f, currentData.size.y / 2f - 0.5f, 0f);
                buildingFactory.CreateBuilding(currentData, spawnPos);
                previewRenderer.gameObject.SetActive(false);
                currentData = null;
            }
        }

        private bool IsPlacementValid(Vector3Int startCell, Vector2Int size)
        {
            return true;
        }
        
        public void PlaceBuilding(BaseBuildingData data, Vector3 worldPosition)
        {
            Vector3 spawnPos = worldPosition + new Vector3(
                data.size.x / 2f - 0.5f,
                data.size.y / 2f - 0.5f,
                0f
            );

            Instantiate(data.prefab, spawnPos, Quaternion.identity);
        }


        
        private Vector3 GetScaleFromSize(Vector2Int size)
        {
            return new Vector3(size.x, size.y, 1f);
        }
    }
}

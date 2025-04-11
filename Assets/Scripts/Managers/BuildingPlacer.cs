using Core.Factories;
using Data.Buildings;
using GridSystem;
using UnityEngine;

namespace Managers
{
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private Color validColor = new(0f, 1f, 0f, 0.4f);
        [SerializeField] private Color invalidColor = new(1f, 0f, 0f, 0.4f);

        private BaseBuildingData currentData;
        private GameObject ghost;
        private SpriteRenderer ghostRenderer;
        private int originalSortingOrder;
        private Camera cam;
        
        [SerializeField] private MonoBehaviour factorySource;
        private IBuildingFactory buildingFactory;

        private void Awake()
        {
            cam = Camera.main;
            buildingFactory = factorySource as IBuildingFactory;
        }

        private void Update()
        {
            if (currentData == null || ghost == null) return;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            Vector3 snapped = GridManager.Instance.GetSnappedPosition(cell, currentData.size);

            ghost.transform.position = snapped;

            bool isValid = GridManager.Instance.IsAreaFree(cell, currentData.size);
            ghostRenderer.color = isValid ? validColor : invalidColor;

            if (Input.GetMouseButtonDown(0))
            {
                if (isValid)
                {
                    buildingFactory.CreateBuilding(currentData, snapped);
                    GridManager.Instance.OccupyArea(cell, currentData.size);
                }

                Destroy(ghost);
                ghost = null;
                currentData = null;
            }
        }

        public void StartPlacing(BaseBuildingData data)
        {
            if (ghost != null) Destroy(ghost);

            currentData = data;
            ghost = Instantiate(data.prefab);
            ghostRenderer = ghost.GetComponentInChildren<SpriteRenderer>();
            originalSortingOrder = ghostRenderer.sortingOrder;
            ghostRenderer.sortingOrder = originalSortingOrder + 1;
            ghostRenderer.color = invalidColor;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            ghost.transform.position = GridManager.Instance.GetSnappedPosition(cell, data.size);
        }

        public void PlaceBuilding(BaseBuildingData data, Vector3 worldPosition)
        {
            Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(worldPosition);

            if (!GridManager.Instance.IsAreaFree(cell, data.size))
                return;

            Vector3 snapped = GridManager.Instance.GetSnappedPosition(cell, data.size);
            Instantiate(data.prefab, snapped, Quaternion.identity);
            GridManager.Instance.OccupyArea(cell, data.size);
        }
    }
}

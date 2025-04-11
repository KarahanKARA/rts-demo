using Data.Buildings;
using GridSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public class DragManager : MonoBehaviour
    {
        [SerializeField] private BuildingPlacer placer;
        [SerializeField] private Color dragColor = new(1f, 1f, 1f, 0.6f);

        private Camera cam;
        private BaseBuildingData draggingData;
        private GameObject ghostObject;
        private SpriteRenderer ghostRenderer;

        private void Start()
        {
            cam = Camera.main;
        }

        public void StartDrag(BaseBuildingData data)
        {
            draggingData = data;
            ghostObject = Instantiate(data.prefab);
            ghostRenderer = ghostObject.GetComponentInChildren<SpriteRenderer>();
            ghostRenderer.color = dragColor;
        }

        private void Update()
        {
            if (draggingData == null || ghostObject == null) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            Vector3 snapped = GridManager.Instance.GetSnappedPosition(cell, draggingData.size);

            ghostObject.transform.position = snapped;

            bool isValid = GridManager.Instance.IsAreaFree(cell, draggingData.size);
            ghostRenderer.color = isValid ? dragColor : new Color(1f, 0.3f, 0.3f, dragColor.a);

            if (Input.GetMouseButtonUp(0))
            {
                if (isValid)
                {
                    Instantiate(draggingData.prefab, snapped, Quaternion.identity);
                    GridManager.Instance.OccupyArea(cell, draggingData.size);
                }

                Destroy(ghostObject);
                ghostObject = null;
                draggingData = null;
            }
        }
    }
}
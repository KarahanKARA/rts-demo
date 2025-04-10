using Data.Buildings;
using GridSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public class DragManager : MonoBehaviour
    {
        [SerializeField] private BuildingPlacer placer;

        private Camera cam;
        private BaseBuildingData draggingData;
        private GameObject ghostObject;

        private void Start()
        {
            cam = Camera.main;
        }

        public void StartDrag(BaseBuildingData data)
        {
            draggingData = data;
            ghostObject = Instantiate(data.prefab);
            ghostObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f);
        }

        private void Update()
        {
            if (draggingData == null || ghostObject == null)
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cellPos = GridManager.Instance.layoutGrid.WorldToCell(mouseWorld);
            Vector3 placePos = GridManager.Instance.layoutGrid.CellToWorld(cellPos);

            Vector3 offset = new Vector3(
                draggingData.size.x / 2f - 0.5f,
                draggingData.size.y / 2f - 0.5f,
                0f
            );
            ghostObject.transform.position = placePos + offset;

            if (Input.GetMouseButtonUp(0))
            {
                placer.PlaceBuilding(draggingData, placePos);
                Destroy(ghostObject);
                ghostObject = null;
                draggingData = null;
            }
        }
    }
}
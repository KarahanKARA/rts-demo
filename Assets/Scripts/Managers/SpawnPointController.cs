using UnityEngine;
using GridSystem;
using Data.Buildings;
using System.Collections;
using Core.Enums;
using Managers;

namespace Managers
{
    public class SpawnPointController : MonoBehaviour
    {
        [SerializeField] private GameObject flagSprite;
        [SerializeField] private GameObject crossPersistentPrefab;
        [SerializeField] private GameObject crossPopupPrefab;
        [SerializeField] private Transform crossSignParentTransform;

        private const string CrossPopupKey = "CrossPopup";

        private BaseBuildingData _currentBuilding;
        private UnitSpawnPointHolder _currentSpawnHolder;
        private Vector3Int _spawnCell;
        private GameObject _persistentCrossInstance;
        private Camera _mainCamera;


        private IEnumerator Start()
        {
            _mainCamera = Camera.main;
            flagSprite?.SetActive(false);

            if (SelectionManager.Instance != null)
                SelectionManager.Instance.OnSelectedChanged += OnSelectionChanged;
            
            yield return new WaitUntil(() => ObjectPoolManager.Instance != null);
            PreloadPopupPool();
        }

        private void Update()
        {
            if (_currentBuilding == null || !_currentBuilding.CanProduceUnits) return;

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 world = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                world.z = 0;

                Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(world);

                if (!IsCellValid(cell))
                {
                    InfoPopupManager.Instance.ShowPopup(InfoPopupType.InvalidSpawnPoint);
                    ShowPopupCross(world);
                    return;
                }

                _spawnCell = cell;
                _currentSpawnHolder?.SetSpawnCell(_spawnCell);
                UpdateFlagAndCross();
            }

            UpdateFlagAndCross();
        }

        private void PreloadPopupPool()
        {
            ObjectPoolManager.Instance.Preload(crossPopupPrefab, 5, crossSignParentTransform);
        }

        private void ShowPopupCross(Vector3 worldPos)
        {
            GameObject cross = ObjectPoolManager.Instance.Get(CrossPopupKey, crossPopupPrefab, crossSignParentTransform);
            cross.transform.position = worldPos;
            cross.SetActive(true);

            StartCoroutine(HideAfterDelay(cross));
        }

        private IEnumerator HideAfterDelay(GameObject go)
        {
            yield return new WaitForSeconds(1f);
            go.SetActive(false);
            ObjectPoolManager.Instance.Return(CrossPopupKey, go);
        }

        private void OnSelectionChanged(GameObject selected)
        {
            flagSprite?.SetActive(false);
            _persistentCrossInstance?.SetActive(false);

            if (selected == null || !selected.TryGetComponent(out BuildingDataHolder holder) || !holder.Data.CanProduceUnits)
            {
                _currentBuilding = null;
                _currentSpawnHolder = null;
                return;
            }

            _currentBuilding = holder.Data;

            if (!selected.TryGetComponent(out UnitSpawnPointHolder spawnHolder))
                return;

            _currentSpawnHolder = spawnHolder;
            _spawnCell = _currentSpawnHolder.SpawnCell;
            UpdateFlagAndCross();
        }

        private void UpdateFlagAndCross()
        {
            if (_currentSpawnHolder == null) return;

            bool valid = IsCellValid(_currentSpawnHolder.SpawnCell);
            _currentSpawnHolder.SetProductionAllowed(valid);

            Vector3 world = GridManager.Instance.LayoutGrid.CellToWorld(_currentSpawnHolder.SpawnCell) + new Vector3(0.5f, 0.5f, 0f);

            if (valid)
            {
                flagSprite?.SetActive(true);
                flagSprite.transform.position = world;

                if (_persistentCrossInstance != null)
                    _persistentCrossInstance.SetActive(false);
            }
            else
            {
                flagSprite?.SetActive(false);

                if (_persistentCrossInstance == null)
                {
                    _persistentCrossInstance = Instantiate(crossPersistentPrefab, crossSignParentTransform);
                }

                _persistentCrossInstance.transform.position = world;
                _persistentCrossInstance.SetActive(true);
            }
        }

        private bool IsCellValid(Vector3Int cell)
        {
            return cell.x >= 0 && cell.x < GridManager.Instance.GridWidth &&
                   cell.y >= 0 && cell.y < GridManager.Instance.GridHeight &&
                   GridManager.Instance.IsAreaFree(cell, Vector2Int.one);
        }
    }
}

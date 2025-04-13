using Core.Enums;
using Core.Interfaces;
using Data.Buildings;
using Data.Units;
using GridSystem;
using UnityEngine;

namespace Managers
{
    public class UnitProducer : MonoBehaviour
    {
        private IUnitFactory _unitFactory;
        private UnitSpawnPointHolder _spawnPointHolder;
        [SerializeField] private InfoPopupManager infoPopupManager;

        private void Awake()
        {
            _spawnPointHolder = GetComponent<UnitSpawnPointHolder>();
        }

        public void SetFactory(IUnitFactory factory) => _unitFactory = factory;

        public void Produce(UnitData unitData)
        {
            if (!_spawnPointHolder.CanProduce)
            {
                InfoPopupManager.Instance.ShowPopup(InfoPopupType.ObstructedSpawnArea);
                return;
            }

            if (_unitFactory == null || _spawnPointHolder == null) return;

            Vector3 finalSpawnPos = GridManager.Instance.LayoutGrid.CellToWorld(_spawnPointHolder.SpawnCell) + new Vector3(0.5f, 0.5f, 0f);
            Vector2 offset = Random.insideUnitCircle * 0.2f;
            finalSpawnPos += new Vector3(offset.x, offset.y, 0f);

            var unitGO = _unitFactory.CreateUnit(unitData, finalSpawnPos);

            if (unitGO.TryGetComponent(out UnitController controller))
                controller.Initialize(unitData, finalSpawnPos);
        }
    }
}
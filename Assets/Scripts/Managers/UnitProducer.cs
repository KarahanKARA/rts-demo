using Core.Factories;
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

        private void Awake()
        {
            _spawnPointHolder = GetComponent<UnitSpawnPointHolder>();
        }

        public void SetFactory(IUnitFactory factory)
        {
            _unitFactory = factory;
        }

        public void Produce(UnitData unitData)
        {
            if (_unitFactory == null || _spawnPointHolder == null) return;
            if (!_spawnPointHolder.CanProduce) return;

            Vector3 spawnPos = GridManager.Instance.LayoutGrid.CellToWorld(_spawnPointHolder.SpawnCell) + new Vector3(0.5f, 0.5f, 0f);

            Vector2 randomOffset = Random.insideUnitCircle * 0.2f;
            spawnPos += new Vector3(randomOffset.x, randomOffset.y, 0f);

            var unitGO = _unitFactory.CreateUnit(unitData, spawnPos);

            if (unitGO.TryGetComponent(out UnitController controller))
                controller.Initialize(unitData, spawnPos);
        }

    }
}
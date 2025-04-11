using UnityEngine;

namespace Data.Buildings
{
    public class UnitSpawnPointHolder : MonoBehaviour
    {
        public Vector3Int SpawnCell { get; private set; }

        private bool _canProduce = true;
        public bool CanProduce => _canProduce;

        public void SetSpawnCell(Vector3Int cell)
        {
            SpawnCell = cell;
        }

        public void SetProductionAllowed(bool allowed)
        {
            _canProduce = allowed;
        }
    }
}
using UnityEngine;

namespace Data.Buildings
{
    public class UnitSpawnPointHolder : MonoBehaviour
    {
        public Vector3Int SpawnCell { get; private set; }

        public void SetSpawnCell(Vector3Int cell)
        {
            SpawnCell = cell;
        }
    }
}
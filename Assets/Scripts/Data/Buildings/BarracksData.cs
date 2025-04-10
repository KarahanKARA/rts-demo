using System.Collections.Generic;
using Data.Units;
using UnityEngine;

namespace Data.Buildings
{
    [CreateAssetMenu(menuName = "Buildings/Barracks")]
    public class BarracksData : BaseBuildingData
    {
        public List<UnitData> producibleUnits;

        public override bool CanProduceUnits => true;
        public override bool IsObstacle => true;
    }
}
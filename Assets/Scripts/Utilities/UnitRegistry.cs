using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class UnitRegistry
    {
        private static readonly List<GameObject> _allUnits = new();

        public static IReadOnlyList<GameObject> AllUnits => _allUnits;

        public static void Register(GameObject unit)
        {
            if (!_allUnits.Contains(unit))
                _allUnits.Add(unit);
        }

        public static void Unregister(GameObject unit)
        {
            if (_allUnits.Contains(unit))
                _allUnits.Remove(unit);
        }
    }
}
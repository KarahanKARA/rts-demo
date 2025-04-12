using Core.Interfaces;
using UnityEngine;
using Utilities;

namespace Data.Buildings
{
    public class BuildingDataHolder : MonoBehaviour, ISelectable
    {
        public BaseBuildingData Data;

        private BuildingHealth _health;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _health = GetComponent<BuildingHealth>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public BuildingHealth Health => _health;

        public void OnSelect()
        {
            if (_renderer != null)
                _renderer.color = SelectionColorsUtility.SelectedColor;
        }

        public void OnDeselect()
        {
            if (_renderer != null)
                _renderer.color = SelectionColorsUtility.DeselectedColor;
        }
    }
}
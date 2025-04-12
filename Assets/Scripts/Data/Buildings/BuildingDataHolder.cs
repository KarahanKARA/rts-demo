using Core.Interfaces;
using UnityEngine;

namespace Data.Buildings
{
    public class BuildingDataHolder : MonoBehaviour, ISelectable
    {
        public BaseBuildingData Data;

        private BuildingHealth _health;
        private SpriteRenderer _renderer;
        private Color _originalColor;
        private Color _selectedColor = new Color(0.65f, 0.8f, 1f, 1f);

        private void Awake()
        {
            _health = GetComponent<BuildingHealth>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
            if (_renderer != null)
                _originalColor = _renderer.color;
        }

        public BuildingHealth Health => _health;

        public void OnSelect()
        {
            if (_renderer != null)
                _renderer.color = _selectedColor;
        }

        public void OnDeselect()
        {
            if (_renderer != null)
                _renderer.color = _originalColor;
        }
    }
}
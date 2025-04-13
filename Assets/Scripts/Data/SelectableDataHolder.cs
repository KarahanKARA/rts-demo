using Core.Health;
using Core.Interfaces;
using UnityEngine;
using Utilities;

namespace Data
{
    public abstract class SelectableDataHolder : MonoBehaviour, ISelectable, IDisplayInfoProvider
    {
        protected SpriteRenderer _renderer;

        protected virtual void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public abstract string DisplayName { get; }
        public abstract Sprite Icon { get; }
        public abstract int? AttackValue { get; }
        public abstract HealthBase Health { get; }

        public void OnSelect()
        {
            if (_renderer != null)
                _renderer.color = AlphaColors.SelectedColor;
        }

        public void OnDeselect()
        {
            if (_renderer != null)
                _renderer.color = AlphaColors.DeselectedColor;
        }
    }
}
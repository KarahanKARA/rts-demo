using Core.Input;
using Core.Interfaces;
using UnityEngine;

namespace Managers
{
    public class UnitMovementCommander : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelector;

        private void Start()
        {
            ClickInputRouter.Instance.OnRightClickDown += HandleRightClick;
        }

        private void HandleRightClick(Vector3 worldPos)
        {
            var selected = unitSelector.GetSelected();

            foreach (var selectable in selected)
            {
                if (selectable is MonoBehaviour mb && mb.TryGetComponent<IControllable>(out var controller))
                {
                    controller.MoveTo(worldPos);
                }
            }
        }
    }
}
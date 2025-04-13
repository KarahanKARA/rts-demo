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
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            var selected = unitSelector.GetSelected();

            if (hit.collider != null && hit.collider.TryGetComponent<IAttackable>(out var target))
            {
                foreach (var unit in selected)
                {
                    if (unit is MonoBehaviour mb && mb.TryGetComponent<UnitController>(out var uc))
                    {
                        uc.AttackTarget(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                foreach (var unit in selected)
                {
                    if (unit is MonoBehaviour mb && mb.TryGetComponent<IControllable>(out var controller))
                    {
                        controller.MoveTo(worldPos);
                    }
                }
            }
        }

    }
}
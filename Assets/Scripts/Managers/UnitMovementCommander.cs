using System.Collections;
using Core.Input;
using Core.Interfaces;
using UnityEngine;

namespace Managers
{
    public class UnitMovementCommander : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelector;
        [SerializeField] private GameObject moveSprite;
        private Coroutine moveSpriteCoroutine;
        
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

                ShowMoveSprite(worldPos);
            }
        }
        
        private void ShowMoveSprite(Vector3 position)
        {
            if (moveSpriteCoroutine != null)
            {
                StopCoroutine(moveSpriteCoroutine);
            }

            moveSprite.transform.position = new Vector3(position.x, position.y, moveSprite.transform.position.z);
            moveSprite.SetActive(true);
            moveSpriteCoroutine = StartCoroutine(HideMoveSpriteAfterDelay());
        }

        private IEnumerator HideMoveSpriteAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            moveSprite.SetActive(false);
            moveSpriteCoroutine = null;
        }


    }
}
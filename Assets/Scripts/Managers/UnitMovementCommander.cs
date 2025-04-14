using System.Collections;
using Core.Input;
using Core.Interfaces;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Issues movement or attack commands to selected units based on right-click.
    /// Displays UI indicators for feedback (sword, flag).
    /// </summary>

    public class UnitMovementCommander : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelector;
        [SerializeField] private GameObject locationSprite;
        [SerializeField] private GameObject swordSprite;
        
        private Coroutine _locationSpriteCoroutine;
        private Coroutine _swordSpriteCoroutine;
        
        private void Start()
        {
            ClickInputRouter.Instance.OnRightClickDown += HandleRightClick;
        }
        
       private void HandleRightClick(Vector3 worldPos)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            var selected = unitSelector.GetSelected();

            bool hasControllable = false;

            if (hit.collider != null && hit.collider.TryGetComponent<IAttackable>(out var target))
            {
                if (selected.Count > 0)
                {
                    ShowSwordSprite(worldPos);
                }

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
                        hasControllable = true;
                    }
                }

                if (hasControllable)
                    ShowMoveSprite(worldPos);
            }
        }

        private void ShowMoveSprite(Vector3 position)
        {
            if (_locationSpriteCoroutine != null)
                StopCoroutine(_locationSpriteCoroutine);

            locationSprite.transform.position = new Vector3(position.x, position.y, locationSprite.transform.position.z);
            locationSprite.SetActive(true);
            _locationSpriteCoroutine = StartCoroutine(HideAfterDelay(locationSprite, () => _locationSpriteCoroutine = null));
        }

        private void ShowSwordSprite(Vector3 position)
        {
            if (_swordSpriteCoroutine != null)
                StopCoroutine(_swordSpriteCoroutine);

            swordSprite.transform.position = new Vector3(position.x, position.y, swordSprite.transform.position.z);
            swordSprite.SetActive(true);
            _swordSpriteCoroutine = StartCoroutine(HideAfterDelay(swordSprite, () => _swordSpriteCoroutine = null));
        }

        private IEnumerator HideAfterDelay(GameObject sprite, System.Action onComplete)
        {
            yield return new WaitForSeconds(1f);
            sprite.SetActive(false);
            onComplete?.Invoke();
        }
    }
}
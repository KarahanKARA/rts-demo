using UnityEngine;
using Utilities;

namespace Data.Units
{
    [RequireComponent(typeof(Collider2D))]
    public class UnitSeparation : MonoBehaviour
    {
        [SerializeField] private float separationRadius = 0.4f;
        [SerializeField] private float pushStrength = 1.5f;
        [SerializeField] private LayerMask unitLayer;

        private int frameSkip = 0;
        private const int separationEveryNFrames = 5;
        
        private void FixedUpdate()
        {
            frameSkip++;
            if (frameSkip % separationEveryNFrames != 0) return;
            frameSkip = 0;
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, separationRadius, unitLayer);

            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                Vector2 away = (Vector2)(transform.position - hit.transform.position);
                Vector3 proposedMove = transform.position + (Vector3)(away.normalized * (pushStrength * Time.fixedDeltaTime));

                Vector3Int cell = GridSystem.GridManager.Instance.LayoutGrid.WorldToCell(proposedMove);

                if (GridUtility.IsValidCell(cell, Vector2Int.one))
                {
                    transform.position = proposedMove;
                }
            }
        }
    }
}
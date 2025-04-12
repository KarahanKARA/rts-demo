using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;

namespace Managers
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private LayerMask unitLayer;

        private Vector2 _startPos;
        private Camera _cam;
        private List<GameObject> _currentlySelected = new();

        private void Start()
        {
            _cam = Camera.main;
            selectionBox.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startPos = Input.mousePosition;
                selectionBox.gameObject.SetActive(true);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 currentMouse = Input.mousePosition;
                UpdateSelectionBox(_startPos, currentMouse);
            }

            if (Input.GetMouseButtonUp(0))
            {
                SelectUnitsInBox();
                selectionBox.gameObject.SetActive(false);
            }
        }

        private void UpdateSelectionBox(Vector2 start, Vector2 end)
        {
            Vector2 size = end - start;

            selectionBox.anchoredPosition = start;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            selectionBox.anchoredPosition += new Vector2(size.x > 0 ? 0 : size.x, size.y > 0 ? 0 : size.y);
        }

        private void SelectUnitsInBox()
        {
            DeselectAll();

            Vector2 min = selectionBox.anchoredPosition;
            Vector2 max = min + selectionBox.sizeDelta;

            foreach (var unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_cam, unit.transform.position);

                if (screenPos.x >= min.x && screenPos.x <= max.x &&
                    screenPos.y >= min.y && screenPos.y <= max.y)
                {
                    Select(unit);
                }
            }
        }

        private void Select(GameObject go)
        {
            _currentlySelected.Add(go);

            if (go.TryGetComponent(out SpriteRenderer rend))
            {
                rend.color = Color.cyan;
            }
        }

        private void DeselectAll()
        {
            foreach (var go in _currentlySelected)
            {
                if (go != null && go.TryGetComponent(out SpriteRenderer rend))
                    rend.color = Color.white;
            }

            _currentlySelected.Clear();
        }
    }
}

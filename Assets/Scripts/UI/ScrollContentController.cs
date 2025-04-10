using System.Collections.Generic;
using Data.Buildings;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScrollContentController : MonoBehaviour
    {
        [Header("UI Setup")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

        [Header("Grid Settings")]
        [SerializeField] private int columns = 2;
        [SerializeField] private float itemWidth = 160f;
        [SerializeField] private float itemHeight = 172f;
        [SerializeField] private float spacing = 20f;
        [SerializeField] private int extraRows = 2;

        [Header("Dependencies")]
        [SerializeField] private BuildingPlacer placer;
        [SerializeField] private DragManager dragManager;
        
        private List<BaseBuildingData> dataList;
        private List<BuildingItemUI> pool = new();

        private float viewHeight;
        private int totalRows;
        private int totalVisibleRows;
        private int totalPoolItems;

        private void Start()
        {
            dataList = new List<BaseBuildingData>(Resources.LoadAll<BaseBuildingData>("ScriptableObjects/Buildings"));

            viewHeight = ((RectTransform)scrollRect.viewport).rect.height;
            totalVisibleRows = Mathf.CeilToInt(viewHeight / (itemHeight + spacing)) + extraRows;
            totalRows = dataList.Count / columns + 1;
            totalPoolItems = totalVisibleRows * columns;

            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
            scrollRect.onValueChanged.AddListener(OnScroll);

            SetupContentHeight();
            CreatePool();
        }

        private void SetupContentHeight()
        {
            float totalHeight = totalRows * (itemHeight + spacing);
            content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
        }

        private void CreatePool()
        {
            for (int i = 0; i < totalPoolItems; i++)
            {
                var go = Instantiate(itemPrefab, content);
                var ui = go.GetComponent<BuildingItemUI>();
                ui.Init(placer, dragManager); 
                pool.Add(ui);
            }

            UpdatePool();
        }

        private void OnScroll(Vector2 _)
        {
            UpdatePool();
        }

        private void UpdatePool()
        {
            var scrollY = content.anchoredPosition.y;
            var firstVisibleRow = Mathf.FloorToInt(scrollY / (itemHeight + spacing));

            for (int i = 0; i < pool.Count; i++)
            {
                var rowOffset = i / columns;
                var col = i % columns;
                var dataIndex = ((firstVisibleRow + rowOffset) * columns + col) % dataList.Count;

                if (dataIndex < 0) dataIndex += dataList.Count;

                var item = pool[i];
                item.Setup(dataList[dataIndex]);

                var x = col * (itemWidth + spacing);
                var y = -(firstVisibleRow + rowOffset) * (itemHeight + spacing);

                var totalGridWidth = columns * (itemWidth + spacing);
                var startX = -(totalGridWidth / 2f) + (itemWidth / 2f);

                var rt = item.rectTransform;
                rt.anchoredPosition = new Vector2(startX + x, y);
            }
        }
    }
}

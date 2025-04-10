using Data.Buildings;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    using UnityEngine.EventSystems;

    public class BuildingItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform rectTransform;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI buildNameTmp;

        private BaseBuildingData data;
        private BuildingPlacer placer;
        private DragManager dragManager;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (dragManager != null)
                dragManager.StartDrag(data);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Gerekirse ileride ghost objeyi mouse’a yapıştırmak için kullanılır
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Bırakma kontrolü DragManager tarafından zaten yapılıyor
        }


        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void Init(BuildingPlacer placerRef, DragManager dragRef)
        {
            placer = placerRef;
            dragManager = dragRef;
        }

        public void Setup(BaseBuildingData newData)
        {
            data = newData;
            iconImage.sprite = data.icon;
            buildNameTmp.text = data.buildingName;
        }

        public void OnClick()
        {
            if (dragManager != null)
                dragManager.StartDrag(data);
        }
    }
}
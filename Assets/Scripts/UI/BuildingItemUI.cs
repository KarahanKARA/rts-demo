using Data.Buildings;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildingItemUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI buildNameTmp;

        public RectTransform rectTransform;
        private BaseBuildingData data;
        private BuildingPlacer placer;
        private DragManager dragManager;

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
            if (data != null)
                placer.StartPlacing(data);
        }
    }
}
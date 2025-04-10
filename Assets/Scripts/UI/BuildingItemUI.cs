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
        
        private BaseBuildingData data;
        private BuildingPlacer placer;

        public RectTransform rectTransform;

        public void Init(BuildingPlacer placerRef)
        {
            placer = placerRef;
        }

        public void Setup(BaseBuildingData newData)
        {
            data = newData;
            iconImage.sprite = data.icon;
            buildNameTmp.text = data.buildingName;
        }

        public void OnClick()
        {
            if (placer != null)
                placer.StartPlacing(data);
        }
    }
}
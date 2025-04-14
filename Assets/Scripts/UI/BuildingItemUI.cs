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
        private BaseBuildingData _data;
        private BuildingPlacer _placer;

        public void Init(BuildingPlacer placerRef, DragManager dragRef)
        {
            _placer = placerRef;
        }

        public void Setup(BaseBuildingData newData)
        {
            _data = newData;
            iconImage.sprite = _data.icon;
            buildNameTmp.text = _data.buildingName;
        }

        public void OnClick()
        {
            if (_data != null)
                _placer.StartPlacing(_data);
        }
    }
}
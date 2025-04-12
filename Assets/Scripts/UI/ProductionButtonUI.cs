using Data.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProductionButtonUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button button;
        public UnitData UnitData => unitData;

        private UnitData unitData;

        public void Setup(UnitData data, System.Action<UnitData> onClick)
        {
            unitData = data;
            iconImage.sprite = data.icon;
            nameText.text = data.unitName;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(unitData));
        }
    }
}
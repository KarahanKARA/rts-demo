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
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI damageText;
        
        public UnitData UnitData => unitData;

        private UnitData unitData;

        public void Setup(UnitData data, System.Action<UnitData> onClick)
        {
            unitData = data;
            iconImage.sprite = data.icon;
            nameText.text = data.unitName;

            hpText.text = $"HP: \n{data.health}";
            damageText.text = $"DMG: \n{data.damage}";

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(unitData));
        }
    }
}
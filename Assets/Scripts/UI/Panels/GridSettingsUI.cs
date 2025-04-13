using Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Panels
{
    public class GridSettingsUI : MonoBehaviour
    {
        [Header("References")]
        public TMP_InputField inputFieldX;
        public TMP_InputField inputFieldY;
        public GameSettings gameSettings;

        private void Start()
        {
            inputFieldX.text = gameSettings.gridWidth.ToString();
            inputFieldY.text = gameSettings.gridHeight.ToString();
        }

        public void OnClickSave()
        {
            if (!int.TryParse(inputFieldX.text, out int newX) || newX <= 0) return;
            if (!int.TryParse(inputFieldY.text, out int newY) || newY <= 0) return;

            gameSettings.gridWidth = newX;
            gameSettings.gridHeight = newY;

            ReloadCurrentScene();
        }

        private void ReloadCurrentScene()
        {
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }
    }
}
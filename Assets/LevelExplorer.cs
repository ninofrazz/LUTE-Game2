using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoGaCulture.LUTE
{
    public class LevelExplorer : MonoBehaviour
    {
        private int currentScene;
        public TMP_Text displayedText;

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene().buildIndex;
            displayedText.text = SceneManager.GetActiveScene().name;
        }

        public void ResetScene()
        {
            SceneManager.LoadScene(currentScene);
        }

        public void NextScene()
        {
            SceneManager.LoadScene(currentScene + 1);
        }

        public void PreviousScene()
        {
            SceneManager.LoadScene(currentScene - 1);
        }
    }
}

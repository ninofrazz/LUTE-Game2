using UnityEngine;
using UnityEngine.SceneManagement;
public class GoBackToMap : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }
}


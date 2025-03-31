using UnityEngine;

namespace LoGaCulture.LUTE
{

    public class FlowerButton : MonoBehaviour
    {

        public GameObject[] gameObjects;
        public void ToggleObjects()
        {
            foreach (GameObject obj in gameObjects)
            {
                if (obj != null)
                {
                    // Toggle the active state
                    obj.SetActive(!obj.activeSelf);
                }
            }
        }

    }
}

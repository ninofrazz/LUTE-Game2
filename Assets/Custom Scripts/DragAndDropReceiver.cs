using UnityEngine;

namespace LoGaCulture.LUTE
{
    public class DragAndDropReceiver : MonoBehaviour
    {
        public bool isTriggered;

        HitChecker hitChecker;
        void Start()
        {
            hitChecker = GameObject.FindObjectOfType<HitChecker>();

        }

        // Update is called once per frame
        void Update()
        {
            if (isTriggered)
            {
                hitChecker.Hit = true;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Draggable")
            {
                isTriggered = true;

                if (gameObject.GetComponent<Renderer>() != null)
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(0f, 2f, 0f, 0.3f);
                }
            }
        }

    }
}

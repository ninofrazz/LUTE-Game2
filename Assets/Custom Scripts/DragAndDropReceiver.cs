using UnityEngine;


public class DragAndDropReceiver : MonoBehaviour
{
    public bool isTriggered;
    public Color objectColor;

    HitChecker hitChecker;
    void Start()
    {
        hitChecker = GameObject.FindObjectOfType<HitChecker>();
        objectColor = gameObject.GetComponent<Renderer>().material.color;


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

            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (gameObject.GetComponent<Renderer>() != null)
            {
                objectColor = new Color(0f, 2f, 0f, 0.3f);
                renderer.material.color = objectColor;
            }
        }
    }

}


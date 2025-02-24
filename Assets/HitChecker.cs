using UnityEngine;

public class HitChecker : MonoBehaviour
{
    public Material[] materials;
    public bool Hit;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Hit)
        {
            // Assign a random material to the hit object
            GetComponent<Renderer>().material.color = Color.green;

        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

}

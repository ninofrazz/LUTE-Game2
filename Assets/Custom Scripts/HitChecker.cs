using UnityEngine;

public class HitChecker : MonoBehaviour
{
    public GameObject[] particlesystems;
    public bool Hit;

    ParticleSystem[] childParticleSystems;

    // Update is called once per frame
    void Update()
    {
        if (particlesystems.Length == 2)
        {
            if (Hit)
            {
                particlesystems[0].SetActive(false);
                particlesystems[1].SetActive(true);
            }
            else
            {
                particlesystems[1].SetActive(false);
                particlesystems[0].SetActive(true);
            }
        }
    }
}

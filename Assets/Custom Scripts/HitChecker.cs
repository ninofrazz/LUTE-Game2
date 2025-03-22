using UnityEngine;

public class HitChecker : MonoBehaviour
{
    public GameObject[] particlesystems;
    public bool Hit;

    ParticleSystem[] childParticleSystems;


    public MouseHoldHandler mouseHoldHandlerscript;

    // Update is called once per frame
    void Update()
    {

        mouseHoldHandlerscript = FindAnyObjectByType<MouseHoldHandler>();

        if (particlesystems.Length >= 2)
        {
            if (Hit)
            {
                particlesystems[0].SetActive(false);
                particlesystems[1].SetActive(true);

                if (mouseHoldHandlerscript != null)
                {
                    particlesystems[2].SetActive(false);
                }
            }
            else
            {
                particlesystems[1].SetActive(false);
                particlesystems[0].SetActive(true);
            }


            if (mouseHoldHandlerscript != null)
            {
                if (mouseHoldHandlerscript.isHolding & !Hit)
                {
                    particlesystems[0].SetActive(false);
                    particlesystems[2].SetActive(true);
                }
                if (!mouseHoldHandlerscript.isHolding & !Hit)
                {
                    particlesystems[0].SetActive(true);
                    particlesystems[2].SetActive(false);
                }
            }
        }
    }
}

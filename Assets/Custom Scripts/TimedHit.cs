using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TimedHit : MonoBehaviour
{
    HitChecker checker;
    public ObjectInteraction objectInteraction;
    public TMP_Text counterText;
    public Image radialBar;


    public float duration;
    float timer = 0f;
    public bool Triggered;
    public bool canContinue;


    void Start()
    {
        checker = GetComponent<HitChecker>();
        objectInteraction = FindAnyObjectByType<ObjectInteraction>();
        counterText = GameObject.Find("Counter").GetComponent<TMP_Text>();
        radialBar = GameObject.Find("RadialBar").GetComponent<Image>();


        canContinue = true;
        radialBar.enabled = false;
        RefreshTimer();

    }

    public void RefreshTimer()
    {
        timer = duration;
        checker.Hit = false;
        counterText.text = timer.ToString("N00");
        radialBar.fillAmount = timer / duration;
    }

    void Update()
    {

        if (checker.Hit)
        {

            // Update the counter text with the rounded interpolated value
            counterText.text = timer.ToString("N00");
        }

        if (objectInteraction.allActive)
        {
            StopTimer();
        }

        if (checker.Hit == true)
        {
            StartTimer();
            radialBar.transform.position = transform.position;
        }

        Transform cameraTransform = Camera.main.transform;

        radialBar.transform.LookAt(cameraTransform);
    }

    public void StartTimer()
    {
        if (timer > 0 && canContinue)
        {
            timer -= Time.deltaTime;
            radialBar.fillAmount = timer / duration;
            radialBar.enabled = true;
        }


        if (timer <= 0f)
        {
            counterText.text = 0.ToString("N00");
            radialBar.enabled = false;
            RefreshTimer();
        }
    }

    public void StopTimer()
    {
        radialBar.enabled = false;
        canContinue = false;
    }




}

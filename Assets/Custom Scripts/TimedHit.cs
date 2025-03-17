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
    public Image radialBarPrefab; // Reference to the radial bar prefab
    private Image radialBarInstance; // Instance of the radial bar

    public float duration;
    float timer = 0f;
    public bool Triggered;
    public bool canContinue;

    void Start()
    {
        checker = GetComponent<HitChecker>();
        objectInteraction = FindAnyObjectByType<ObjectInteraction>();
        counterText = GameObject.Find("Counter").GetComponent<TMP_Text>();

        // Instantiate the radial bar
        radialBarInstance = Instantiate(radialBarPrefab, FindObjectOfType<Canvas>().transform);
        radialBarInstance.enabled = false;

        canContinue = true;
        RefreshTimer();
    }

    public void RefreshTimer()
    {
        timer = duration;
        checker.Hit = false;
        counterText.text = timer.ToString("N00");
        if (radialBarInstance != null)
        {
            radialBarInstance.fillAmount = timer / duration;
        }
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
            radialBarInstance.transform.position = transform.position;
        }

        Transform cameraTransform = Camera.main.transform;
        radialBarInstance.transform.LookAt(cameraTransform);
    }

    public void StartTimer()
    {
        if (timer > 0 && canContinue)
        {
            timer -= Time.deltaTime;
            if (radialBarInstance != null)
            {
                radialBarInstance.fillAmount = timer / duration;
                radialBarInstance.enabled = true;
            }
        }

        if (timer <= 0f)
        {
            counterText.text = 0.ToString("N00");
            if (radialBarInstance != null)
            {
                radialBarInstance.enabled = false;
            }
            RefreshTimer();
        }
    }

    public void StopTimer()
    {
        if (radialBarInstance != null)
        {
            radialBarInstance.enabled = false;
        }
        canContinue = false;
    }

    void OnDestroy()
    {
        // Clean up the radial bar instance when the object is destroyed
        if (radialBarInstance != null)
        {
            Destroy(radialBarInstance.gameObject);
        }
    }
}
using Mapbox.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;

public class XRHelper : MonoBehaviour
{

    private static GameObject xrObjectPrefab;
    private static GameObject spawnedXRObject;

    private static bool isInitialised = false;



    // Start is called before the first frame update
    void Start()
    {

        //xrRig = GameObject.Find("XR Origin (XR Rig)");

        //deactivate the two objects
        //xrRig.SetActive(false);
        //arSession.SetActive(false);

        //StartCoroutine(disableafter(.5f));

    }

    public static bool initiliaseXR()
    {

        if (isInitialised)
        {
            return false;
        }

        if (XRGeneralSettings.Instance == null)
        {
            //XRGeneralSettings.Instance = XRGeneralSettings.CreateInstance<XRGeneralSettings>();
        }

        //if (XRGeneralSettings.Instance.Manager == null)
        //{
        //    yield return new WaitUntil(() => XRGeneralSettings.Instance.Manager != null);
        //}

        XRGeneralSettings.Instance?.Manager?.InitializeLoaderSync();

        if (XRGeneralSettings.Instance?.Manager?.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            XRGeneralSettings.Instance?.Manager?.StartSubsystems();
        }

        if (xrObjectPrefab == null)
        {
            xrObjectPrefab = Resources.Load<GameObject>("Prefabs/XR");
        }

        //if the xrObject is not in the scene, then add it
        if (GameObject.Find("XR") == null)
        {
            spawnedXRObject = GameObject.Instantiate(xrObjectPrefab);
            spawnedXRObject.name = "XR";
            isInitialised = true;
            return true;
        }
        else
        {
            spawnedXRObject = GameObject.Find("XR");
            isInitialised = true;
            return true;
        }
        return false;
    }

    //IEnumerator disableafter(float seconds)
    //{
    //    yield return new WaitForSeconds(seconds);
    //    //xrRig.SetActive(false);
    //    arSession.SetActive(false);
    //}



    public static bool toggleXR()
    {

        if (!isInitialised)
        {
            initiliaseXR();
        }

        //if the xrObject is in the scene, then remove it
        if (spawnedXRObject.activeSelf)
        {
            spawnedXRObject.SetActive(false);
            return false;
        }
        else
        {
            spawnedXRObject.SetActive(true);
            return true;
        }
        ////toggle the two objects on and off
        //if(arSession.activeSelf)
        //{
        //    xrRig.SetActive(false);
        //    arSession.SetActive(false);

        //    //get the camera game object and set it to true
        //    GameObject camera = GameObject.Find("Camera");
        //    if(camera != null)
        //    {
        //        camera.SetActive(true);
        //    }

        //    return false;
        //}
        //else
        //{
        //    xrRig.SetActive(true);
        //    arSession.SetActive(true);

        //    //get the camera game object and set it to false
        //    GameObject camera = GameObject.Find("Camera");
        //    if(camera != null)
        //    {
        //        camera.SetActive(false);
        //    }

        //    return true;
        //}


    }

    public static bool setXRActive(bool active)
    {

        if (!isInitialised)
        {
            initiliaseXR();
        }



        if (active)
        {
            //main camera is not needed
            GameObject camera = GameObject.Find("Camera");
            if (camera != null)
            {
                camera.SetActive(false);
            }
        }
        else
        {
            //main camera is needed
            GameObject camera = GameObject.Find("Camera");
            if (camera != null)
            {
                camera.SetActive(true);
            }
        }

        spawnedXRObject.SetActive(active);



        return active;
    }

    public static XRHelper getXRScript()
    {
        if (!isInitialised)
        {
            initiliaseXR();
        }
        return spawnedXRObject.GetComponent<XRHelper>();
    }

    public bool TogglePlaneDetection(bool active)
    {
        if (!isInitialised)
        {
            initiliaseXR();
        }
        ARPlaneManager planeManager = GetComponentInChildren<ARPlaneManager>();
        if (planeManager == null)
        {
            return false;
        }

        planeManager.enabled = active;

        SetAllPlanesActive(planeManager.enabled);


        return true;

    }

    public bool TogglePointCloud(bool active)
    {
        if (!isInitialised)
        {
            initiliaseXR();
        }

        ARPointCloudManager pointCloudManager = GetComponentInChildren<ARPointCloudManager>();
        if (pointCloudManager == null)
        {
            return false;
        }

        pointCloudManager.enabled = active;

        return true;
    }


    private void SetAllPlanesActive(bool active)
    {

        if (!isInitialised)
        {
            initiliaseXR();
        }

        ARPlaneManager planeManager = GetComponentInChildren<ARPlaneManager>();
        if (planeManager == null)
        {
            return;
        }

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(active);
        }
    }

    //on destroy, deinitialise the xr object
    private void OnDestroy()
    {
        isInitialised = false;
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

    }



    // Update is called once per frame
    void Update()
    {

    }
}


//using UnityEditor.EditorTools;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

[OrderInfo("XR",
              "PlaceObjectOnPlane",
              "")]
[AddComponentMenu("")]
public class PlaceObjectXR : Order
{
    [Tooltip("The 3D object to place when clicked")]
    [SerializeField] protected GameObject m_PrefabToPlace;

    [SerializeField] private string m_ObjectName;

    [SerializeField]
    [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
    ARRaycastHitEventAsset raycastHitEvent;

    [SerializeField]

    public bool automaticallyPlaceObject = true;


 

    [SerializeField]
    public bool rotateable = true;
    [SerializeField]
    public bool scaleable = true;
    [SerializeField]
    public bool moveable = true;

    [SerializeField]
    public PlaneAlignment planeAlignment;


    /// <summary>
    /// The prefab to be placed or moved.
    /// </summary>
    public GameObject prefabToPlace
    {
        get => m_PrefabToPlace;
        set => m_PrefabToPlace = value;
    }

    /// <summary>
    /// The spawned prefab instance.
    /// </summary>
    public GameObject spawnedObject
    {
        get => m_SpawnedObject;
        set => m_SpawnedObject = value;
    }

    GameObject m_SpawnedObject;


    ARPlaneManager planeManager;

 
    private void OnObjectSpawned(GameObject obj)
    {
       
        Debug.Log("Object spawned");

        //get the objectspawner from teh XR game object and remove the object from the object manager
        ObjectSpawner objectSpawner = GameObject.Find("XR").GetComponentInChildren<ObjectSpawner>();
        objectSpawner.objectSpawned -= OnObjectSpawned;

        //get the XRGrabInteractable component of the object
        XRGrabInteractable grabInteractable = obj.GetComponentInChildren<XRGrabInteractable>();

        //set the track position, track rotation and track scale to the rotateable , moveable and scaleable variables
        grabInteractable.trackPosition = moveable;
        grabInteractable.trackRotation = rotateable;
        grabInteractable.trackScale = scaleable;

       //remove the object from the object spawner objectPrefabs list
       objectSpawner.objectPrefabs.Remove(m_PrefabToPlace);

        ObjectSpawner.IsCurrentlyPlacingObject = false;


        //add the object to the object manager
        XRObjectManager.AddObject(m_ObjectName, obj);


        Continue();

       
    }


    public override void OnEnter()
    {

        //get the XR game object
        GameObject arObjectInstance = GameObject.Find("XR");
        //get the AR plane manager
        planeManager = arObjectInstance.GetComponentInChildren<ARPlaneManager>();

        Debug.Log(planeManager.gameObject);

      
        //get the objectsopawner script in the children of the XR game object and subscribe to the event
        ObjectSpawner objectSpawner = arObjectInstance.GetComponentInChildren<ObjectSpawner>();
        objectSpawner.objectSpawned += OnObjectSpawned;

        //add the object to the object spawner objectPrefabs list
        objectSpawner.objectPrefabs.Add(m_PrefabToPlace);

        ObjectSpawner.IsCurrentlyPlacingObject = true;


        //register evenr for plane detection
        if (raycastHitEvent == null || m_PrefabToPlace == null)
        {
            Debug.LogWarning($"{nameof(ARPlaceObject)} component on {name} has null inputs and will have no effect in this scene.", this);
            return;
        }

        if (raycastHitEvent != null)
        {
            if (automaticallyPlaceObject)
            {
                planeManager.planesChanged += OnPlaneDetected;
            }
            else
            {
                //raycastHitEvent.eventRaised += MoveObject;
                //raycastHitEvent.eventRaised += PlaceObjectAt;
            }
        }
        

        

    }

    private void OnPlaneDetected(ARPlanesChangedEventArgs args)
    {
       
        foreach (var plane in args.added)
        {
            //if plabe is horizontal
            if (plane.alignment == planeAlignment)
            {
                //create a new game object
                GameObject go = Instantiate(m_PrefabToPlace, plane.transform.position, plane.transform.rotation);
                //set the parent of the game object to the plane
                go.transform.parent = plane.transform;

                //add the object to the object manager
                XRObjectManager.AddObject(m_ObjectName, go);

                Continue();
            }

        }

    }

    private void PlaceObjectAt(object sender, ARRaycastHit hitPose)
    {
        if (m_SpawnedObject == null)
        {
            m_SpawnedObject = Instantiate(m_PrefabToPlace, hitPose.pose.position, hitPose.pose.rotation, hitPose.trackable.transform.parent);

            var interactable = m_SpawnedObject.AddComponent<InteractableARObject>();

            interactable.isScaleable = scaleable;
            interactable.isMovable = moveable;
            interactable.isRotatable = rotateable;

            XRObjectManager.AddObject(m_ObjectName, m_SpawnedObject);

            Continue();
        }
        else
        {
            //m_SpawnedObject.transform.position = hitPose.pose.position;
            //m_SpawnedObject.transform.parent = hitPose.trackable.transform.parent;
        }
    }

    //raycasthit event for moving object    
    public void MoveObject(object sender, ARRaycastHit hitPose)
    {
        if (moveable)
        {
            if (m_SpawnedObject != null)
            {
                
                m_SpawnedObject.transform.position = hitPose.pose.position;
                m_SpawnedObject.transform.parent = hitPose.trackable.transform.parent;

            }
        }
    }

    private void Update()
    {
        //check for rotating object using touch and two fingers
        if (rotateable)
        {
            //if (Input.touchCount == 2)
            //{
            //    Touch touchZero = Input.GetTouch(0);
            //    Touch touchOne = Input.GetTouch(1);

            //    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            //    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            //    float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            //    float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            //    float difference = currentMagnitude - prevMagnitude;

            //    if (m_SpawnedObject != null)
            //    {
            //        m_SpawnedObject.transform.Rotate(Vector3.up, difference * 10);
            //    }
            //}
        
        }
    }

    public override string GetSummary()
    {
        //you can use this to return a summary of the order which is displayed in the inspector of the order
        return "Places an object on a detected plane, automatically or manually";
    }




}



//Object Manager class that manages all the AR objects that are placed in the scene
//it's static so that it can be accessed from anywhere in the scene
public static class XRObjectManager
{

    //using a dictionary to store the objects, name to object
    public static Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    public static void AddObject(string name, GameObject obj)
    {
        Debug.Log("Adding object to object manager");
        objects.Add(name, obj);
    }

    public static void RemoveObject(string name)
    {
        Debug.Log("Removing object from object manager");
        objects.Remove(name);
    }

    public static GameObject GetObject(string name)
    {

        if (!objects.ContainsKey(name))
        {
            Debug.LogError("Object with name " + name + " not found");
            return null;
        }
        return objects[name];
    }

}
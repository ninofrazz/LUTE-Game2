using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[OrderInfo("XR",
              "XRDragInteraction",
              "")]
[AddComponentMenu("")]
public class XRDragInteraction : Order
{


    public string objectName;

    private GameObject gameObjectToDrag;
    private GameObject transparentObject;

    public Material transparentMaterial;


    private Vector3 scaleOfObject;

    [Tooltip("The offset to spawn the transparent object at")]
    public Vector3 dragOffset;

    //int variable for overlap percentage, make it so it can't go lower than 0 and can't go higher than 100
    
    [Range(0, 100)]
    public float minimumOverlapPercentage = 75;




    public void OnPuzzleSolved()
    {

        //Destroy the transparent object
        Destroy(transparentObject);

        //get the grabInteractable component of the object to drag and disable it
        XRGrabInteractable grabInteractable = gameObjectToDrag.GetComponent<XRGrabInteractable>();
        grabInteractable.enabled = false;


        Continue();
    }

   
    


    //function that takes in the object to drag, and instantiates a transparent version of it at the drag offset so the user then has to drag the normal object 
    public void StartDrag()
    {
        //instantiate a transparent version of the object to drag
        transparentObject = Instantiate(gameObjectToDrag, gameObjectToDrag.transform.position + dragOffset, gameObjectToDrag.transform.rotation);

        //transparentObject.transform.localScale = Vector3.Scale(transparentObject.transform.localScale, scaleOfObject);

        //set tag to Piece so that the overlap detector can detect it
        gameObjectToDrag.tag = "DragPiece";

        //attach the overlap detector script to the transparent object
        var overlapDetector = transparentObject.AddComponent<OverlapDetector>();
        overlapDetector.minimumOverlapPercentage = minimumOverlapPercentage;
        //set the callback function to be the OnPuzzleSolved function
        overlapDetector.PuzzleSolved += OnPuzzleSolved;

        //set the box collider of the transparent object to be a trigger
        transparentObject.GetComponent<BoxCollider>().isTrigger = true;

        //add rigidbody to the transparent object
        transparentObject.AddComponent<Rigidbody>();
        Rigidbody rigidbody = transparentObject.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        //go through all the children of the transparent object (children can be nested) and set their materials to transparent
        foreach (Renderer renderer in transparentObject.GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = transparentMaterial;
            }
            renderer.materials = materials;
        }
       
        
        //transparentObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
        //set the object to drag to the transparent object
       // gameObjectToDrag = transparentObject;
    }

    public override void OnEnter()
    {


        //instantiaite the object to drag
        gameObjectToDrag = XRObjectManager.GetObject(objectName);
        //set the scale of the object to the scale of the object to drag
        scaleOfObject = gameObjectToDrag.transform.localScale;


        //this code gets executed when the order is called
        StartDrag();
      //this code gets executed as the order is called
      //some orders may not lead to another node so you can call continue if you wish to move to the next order after this one   
      //Continue();
    }

  public override string GetSummary()
  {
 //you can use this to return a summary of the order which is displayed in the inspector of the order
      return "";
  }
}
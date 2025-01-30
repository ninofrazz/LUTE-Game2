using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Generic",
              "Hide and Show Object",
              "Hides or shows a provided object in the scene.")]
    [AddComponentMenu("")]
    public class HideShowObject : Order
    {
        [Tooltip("The object to hide")]
        [SerializeField] protected GameObject objectToHideOrShow;
        [Tooltip("Time to wait until the object is hidden")]
        [SerializeField] protected float delay = 0f;
        public override void OnEnter()
        {
            if (objectToHideOrShow == null)
            {
                Continue();
                return;
            }

            Invoke("DelayHideShow", delay);
            Continue();
        }

        private void DelayHideShow()
        {
            bool active = objectToHideOrShow.activeSelf;
            objectToHideOrShow.SetActive(!active);
        }

        public override string GetSummary()
        {
            if (objectToHideOrShow == null)
            {
                return "Error: Object is not provided";
            }
            else
            {
                return "Hide/Show: " + objectToHideOrShow.name + " in " + delay + " seconds";
            }
        }
    }
}

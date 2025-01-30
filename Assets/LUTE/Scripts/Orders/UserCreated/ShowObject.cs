using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Generic",
              "Show Object",
              "Reveals a provided object in the scene")]
    [AddComponentMenu("")]
    public class ShowObject : Order
    {
        [Tooltip("The object to show")]
        [SerializeField] protected GameObject objectToShow;
        [Tooltip("Time to wait until the object is shown")]
        [SerializeField] protected float delay = 0f;
        public override void OnEnter()
        {
            if (objectToShow == null)
            {
                return;
            }

            Invoke("DelayShowObject", delay);
            Continue();
        }

        private void DelayShowObject()
        {
            objectToShow.SetActive(true);
        }

        public override string GetSummary()
        {
            if (objectToShow == null)
            {
                return "Error: Object to show is not provided";
            }
            else
            {
                return "Show: " + objectToShow.name + " in " + delay + " seconds";
            }
        }
    }
}

using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Scripting", "Call Method", "Calls a named method on the selected GameObject.")]
    [AddComponentMenu("")]
    public class CallMethod : Order
    {
        [Tooltip("The GameObject to call the method on, should be a monobehaviour derived object.")]
        [SerializeField] protected GameObject targetObject;
        [Tooltip("The name of the method to call.")]
        [SerializeField] protected string methodName;
        [Tooltip("The delay (seconds) to wait until calling the method.")]
        [SerializeField] protected float delay;

        protected virtual void CallTheMethod()
        {
            targetObject.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
        }

        public override void OnEnter()
        {
            if (targetObject == null ||
                methodName.Length == 0)
            {
                Continue();
                return;
            }

            if (Mathf.Approximately(delay, 0f))
            {
                CallTheMethod();
            }
            else
            {
                Invoke("CallTheMethod", delay);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (targetObject == null)
            {
                return "Error: No target GameObject specified";
            }

            if (methodName.Length == 0)
            {
                return "Error: No named method specified";
            }

            return targetObject.name + " : " + methodName;
        }

        public override Color GetButtonColour()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}

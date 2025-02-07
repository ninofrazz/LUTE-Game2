using System;
using UnityEngine;
using UnityEngine.Events;
using static BooleanVariable;
using static FloatVariable;

namespace LoGaCulture.LUTE
{
    /// Supported types of method invocation.
    public enum InvokeType
    {
        /// <summary> Call a method with an optional constant value parameter. </summary>
        Static,         // 
        /// <summary> Call a method with an optional boolean constant / variable parameter. </summary>
        DynamicBoolean,
        /// <summary> Call a method with an optional integer constant / variable parameter. </summary>
        DynamicInteger,
        /// <summary> Call a method with an optional float constant / variable parameter. </summary>
        DynamicFloat,
        /// <summary> Call a method with an optional string constant / variable parameter. </summary>
        DynamicString
    }

    [OrderInfo("Scripting", "Invoke Event", "Calls an event from a componenent via the Unity Event System (akin to the Unity UI). Faster than 'Invoke Method' Order but can only pass single parameters and does nto support return values.")]
    [AddComponentMenu("")]
    public class InvokeEvent : Order
    {
        [Tooltip("Override description of what this Order does. Appears in the Order summary.")]
        [SerializeField] protected string description = "";

        [Tooltip("Delay (in seconds) before the methods will be called")]
        [SerializeField] protected float delay;

        [Tooltip("Selects type of method parameter to pass")]
        [SerializeField] protected InvokeType invokeType;

        [Tooltip("List of methods to call. Supports methods with no parameters or exactly one string, int, float or object parameter.")]
        [SerializeField] protected UnityEvent staticEvent = new UnityEvent();

        [Tooltip("Boolean parameter to pass to the invoked methods.")]
        [SerializeField] protected BooleanData booleanParameter;

        [Tooltip("List of methods to call. Supports methods with one boolean parameter.")]
        [SerializeField] protected BooleanEvent booleanEvent = new BooleanEvent();

        [Tooltip("Integer parameter to pass to the invoked methods.")]
        [SerializeField] protected IntegerData integerParameter;

        [Tooltip("List of methods to call. Supports methods with one integer parameter.")]
        [SerializeField] protected IntegerEvent integerEvent = new IntegerEvent();

        [Tooltip("Float parameter to pass to the invoked methods.")]
        [SerializeField] protected FloatData floatParameter;

        [Tooltip("List of methods to call. Supports methods with one float parameter.")]
        [SerializeField] protected FloatEvent floatEvent = new FloatEvent();

        [Tooltip("String parameter to pass to the invoked methods.")]
        [SerializeField] protected StringData stringParameter;

        [Tooltip("List of methods to call. Supports methods with one string parameter.")]
        [SerializeField] protected StringEvent stringEvent = new StringEvent();

        protected virtual void DoInvoke()
        {
            switch (invokeType)
            {
                default:
                case InvokeType.Static:
                    staticEvent.Invoke();
                    break;
                case InvokeType.DynamicBoolean:
                    booleanEvent.Invoke(booleanParameter.Value);
                    break;
                case InvokeType.DynamicInteger:
                    integerEvent.Invoke(integerParameter.Value);
                    break;
                case InvokeType.DynamicFloat:
                    floatEvent.Invoke(floatParameter.Value);
                    break;
                case InvokeType.DynamicString:
                    stringEvent.Invoke(stringParameter.Value);
                    break;
            }
        }

        [Serializable] public class BooleanEvent : UnityEvent<bool> { }
        [Serializable] public class IntegerEvent : UnityEvent<int> { }
        [Serializable] public class FloatEvent : UnityEvent<float> { }
        [Serializable] public class StringEvent : UnityEvent<string> { }

        public override void OnEnter()
        {
            if (Mathf.Approximately(delay, 0f))
            {
                DoInvoke();
            }
            else
            {
                Invoke("DoInvoke", delay);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            string summary = invokeType.ToString() + " ";

            switch (invokeType)
            {
                default:
                case InvokeType.Static:
                    summary += staticEvent.GetPersistentEventCount();
                    break;
                case InvokeType.DynamicBoolean:
                    summary += booleanEvent.GetPersistentEventCount();
                    break;
                case InvokeType.DynamicInteger:
                    summary += integerEvent.GetPersistentEventCount();
                    break;
                case InvokeType.DynamicFloat:
                    summary += floatEvent.GetPersistentEventCount();
                    break;
                case InvokeType.DynamicString:
                    summary += stringEvent.GetPersistentEventCount();
                    break;
            }

            return summary + " methods";
        }

        public override Color GetButtonColour()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return booleanParameter.booleanRef == variable || integerParameter.integerRef == variable || floatParameter.floatRef == variable || stringParameter.stringRef == variable || base.HasReference(variable);
        }
    }
}
using MarkerMetro.Unity.WinLegacy.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Scripting", "Invoke Method", "Invokes a named method of a given component. Supports multiple parameters and storing the return value of the given method.")]
    public class InvokeMethod : Order
    {
        [Tooltip("An optional description to override the default Order summary.")]
        [SerializeField] protected string description;
        [Tooltip("The GameObject to call the method on, ideally a monobehaviour derived object.")]
        [SerializeField] protected GameObject targetObject;
        [HideInInspector]
        [Tooltip("Name of assembly containing the target component.")]
        [SerializeField] protected string targetComponentAssemblyName;
        [HideInInspector]
        [Tooltip("Full name of the target component.")]
        [SerializeField] protected string targetComponentFullname;
        [HideInInspector]
        [Tooltip("Display name of the target component.")]
        [SerializeField] protected string targetComponentText;
        [HideInInspector]
        [Tooltip("Name of target method to invoke on the target component.")]
        [SerializeField] protected string targetMethod;
        [HideInInspector]
        [Tooltip("Display name of target method to invoke on the target component.")]
        [SerializeField] protected string targetMethodText;
        [HideInInspector]
        [Tooltip("List of parameters to pass to the invoked method.")]
        [SerializeField] protected InvokeMethodParameter[] methodParameters;
        [HideInInspector]
        [Tooltip("If true, store the return value of the invoked method in a LUTE variable of the SAME type.")]
        [SerializeField] protected bool saveReturnValue;
        [HideInInspector]
        [Tooltip("THe name of the variable to store the return value in.")]
        [SerializeField] protected string returnValueVariableKey;
        [HideInInspector]
        [Tooltip("The type of the return value.")]
        [SerializeField] protected string returnValueType;
        [HideInInspector]
        [Tooltip("If true, list all inherited methods of the target component.")]
        [SerializeField] protected bool showInherited;
        [HideInInspector]
        [Tooltip("The coroutine call behavior for methods that return IEnumerator")]
        [SerializeField] protected CallMode callMode;

        protected Type componentType;
        protected Component objComponent;
        protected Type[] parameterTypes = null;
        protected MethodInfo objMethod;

        protected virtual void Awake()
        {
            if (componentType == null)
            {
                componentType = ReflectionHelper.GetType(targetComponentAssemblyName);
            }

            if (objComponent == null)
            {
                objComponent = targetObject.GetComponent(componentType);
            }

            if (parameterTypes == null)
            {
                parameterTypes = GetParameterTypes();
            }

            if (objMethod == null)
            {
                objMethod = UnityEvent.GetValidMethodInfo(objComponent, targetMethod, parameterTypes);
            }
        }

        protected virtual IEnumerator ExecuteCoroutine()
        {
            yield return StartCoroutine((IEnumerator)objMethod.Invoke(objComponent, GetParameterValues()));

            if (callMode == CallMode.WaitUntilFinished)
            {
                Continue();
            }
        }

        protected virtual Type[] GetParameterTypes()
        {
            Type[] types = new Type[methodParameters.Length];
            for (int i = 0; i < methodParameters.Length; i++)
            {
                var item = methodParameters[i];
                var objType = ReflectionHelper.GetType(item.objValue.typeAssemblyname);

                types[i] = objType;
            }
            return types;
        }

        protected virtual object[] GetParameterValues()
        {
            object[] values = new object[methodParameters.Length];
            var engine = GetEngine();

            for (int i = 0; i < methodParameters.Length; i++)
            {
                var item = methodParameters[i];

                if (string.IsNullOrEmpty(item.variableKey))
                {
                    values[i] = item.objValue.GetValue();
                }
                else
                {
                    object objValue = null;

                    switch (item.objValue.typeFullname)
                    {
                        case "System.Int32":
                            var intvalue = engine.GetVariable<IntegerVariable>(item.variableKey);
                            if (intvalue != null)
                                objValue = intvalue.Value;
                            break;
                        case "System.Boolean":
                            var boolean = engine.GetVariable<BooleanVariable>(item.variableKey);
                            if (boolean != null)
                                objValue = boolean.Value;
                            break;
                        case "System.Single":
                            var floatvalue = engine.GetVariable<FloatVariable>(item.variableKey);
                            if (floatvalue != null)
                                objValue = floatvalue.Value;
                            break;
                        case "System.String":
                            var stringvalue = engine.GetVariable<StringVariable>(item.variableKey);
                            if (stringvalue != null)
                                objValue = stringvalue.Value;
                            break;
                        case "UnityEngine.Sprite":
                            var sprite = engine.GetVariable<SpriteVariable>(item.variableKey);
                            if (sprite != null)
                                objValue = sprite.Value;
                            break;
                            // Must create object variable type
                            //default:
                            //    var obj = engine.GetVariable<ObjectVariable>(item.variableKey);
                            //    if (obj != null)
                            //        objValue = obj.Value;
                            //    break;
                    }

                    values[i] = objValue;
                }
            }
            return values;
        }

        protected virtual void SetVariable(string key, object value, string returnType)
        {
            var engine = GetEngine();

            switch (returnType)
            {
                case "System.Int32":
                    engine.GetVariable<IntegerVariable>(key).Value = (int)value;
                    break;
                case "System.Boolean":
                    engine.GetVariable<BooleanVariable>(key).Value = (bool)value;
                    break;
                case "System.Single":
                    engine.GetVariable<FloatVariable>(key).Value = (float)value;
                    break;
                case "System.String":
                    engine.GetVariable<StringVariable>(key).Value = (string)value;
                    break;
                case "UnityEngine.Sprite":
                    engine.GetVariable<SpriteVariable>(key).Value = (UnityEngine.Sprite)value;
                    break;
                    // Must create object variable type
                    //default:
                    //    flowChart.GetVariable<ObjectVariable>(key).Value = (UnityEngine.Object)value;
                    //    break;
            }
        }

        // Gameobject containing the component to call the method on
        // Globally accessible
        public virtual GameObject TargetObject { get { return targetObject; } }

        public override void OnEnter()
        {
            try
            {
                if (targetObject == null || string.IsNullOrEmpty(targetComponentAssemblyName) || string.IsNullOrEmpty(targetMethod))
                {
                    Continue();
                    return;
                }

                if (returnValueType != "System.Collections.IEnumerator")
                {
                    var objReturnValue = objMethod.Invoke(objComponent, GetParameterValues());

                    if (saveReturnValue)
                    {
                        SetVariable(returnValueVariableKey, objReturnValue, returnValueType);
                    }
                    Continue();
                }
                else
                {
                    StartCoroutine(ExecuteCoroutine());
                    if (callMode == CallMode.Continue)
                    {
                        Continue();
                    }
                    else if (callMode == CallMode.Stop)
                    {
                        StopParentNode();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }
        }

        public override Color GetButtonColour()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            if (targetObject == null)
            {
                return "Error: targetObject is not assigned";
            }

            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            return targetObject.name + "." + targetComponentText + "." + targetMethodText;
        }

        [System.Serializable]
        public class InvokeMethodParameter
        {
            [SerializeField] public ObjectValue objValue;
            [SerializeField] public string variableKey;
        }

        [System.Serializable]
        public class ObjectValue
        {
            public string typeAssemblyname;
            public string typeFullname;

            public int intValue;
            public bool boolValue;
            public float floatValue;
            public string stringValue;

            public Color colorValue;
            public GameObject gameObjectValue;
            public Material materialValue;
            public UnityEngine.Object objectValue;
            public Sprite spriteValue;
            public Texture textureValue;
            public Vector2 vector2Value;
            public Vector3 vector3Value;

            public object GetValue()
            {
                switch (typeFullname)
                {
                    case "System.Int32":
                        return intValue;
                    case "System.Boolean":
                        return boolValue;
                    case "System.Single":
                        return floatValue;
                    case "System.String":
                        return stringValue;
                    case "UnityEngine.Color":
                        return colorValue;
                    case "UnityEngine.GameObject":
                        return gameObjectValue;
                    case "UnityEngine.Material":
                        return materialValue;
                    case "UnityEngine.Sprite":
                        return spriteValue;
                    case "UnityEngine.Texture":
                        return textureValue;
                    case "UnityEngine.Vector2":
                        return vector2Value;
                    case "UnityEngine.Vector3":
                        return vector3Value;
                    default:
                        var objType = ReflectionHelper.GetType(typeAssemblyname);

                        if (objType.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            return objectValue;
                        }
                        else if (objType.IsEnum())
                            return System.Enum.ToObject(objType, intValue);

                        break;
                }

                return null;
            }
        }
    }
    public static class ReflectionHelper
    {
        static Dictionary<string, Type> types = new Dictionary<string, Type>();

        public static Type GetType(string AssemblyQualifiedNameTypeName)
        {
            if (types.ContainsKey(AssemblyQualifiedNameTypeName) && types[AssemblyQualifiedNameTypeName] != null)
            {
                return types[AssemblyQualifiedNameTypeName];
            }

            types[AssemblyQualifiedNameTypeName] = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(x => x.GetTypes()).FirstOrDefault(t => t.AssemblyQualifiedName == AssemblyQualifiedNameTypeName);

            return types[AssemblyQualifiedNameTypeName];
        }
    }
}

using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    /// <summary>
    /// A class that handles the failure of the location service (i.e. the user cannot accses a location)
    /// Provides a series of methods that can be called to handle inaccessibility of the location
    /// When player reaches a location that cannot be accessed, the location searches for this object and matching location
    /// It then acts according to the priority of the methods defined in this class (this list can be modified by designers in the inspector)
    /// </summary>
    public class LocationFailureHandler : MonoBehaviour
    {
        [SerializeField] protected List<FailureMethod> failureMethods = new List<FailureMethod>();

        public List<FailureMethod> FailureMethods { get => failureMethods; set => failureMethods = value; }

        public enum FailureHandlingOutcome
        {
            Continue, // Continue to the next method in the list
            Stop, // Stop the execution of the methods, consider the failure as handled
            Abort // Abort the execution of the methods, consider the failure unhandled
        }

        private static Dictionary<string, MethodInfo> availableMethods = new Dictionary<string, MethodInfo>();

        private void Awake()
        {
            RegisterFailureHandlingMethods();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            RegisterFailureHandlingMethods();
        }

        private static void RegisterFailureHandlingMethods()
        {
            if (availableMethods.Count > 0)
            {
                return;
            }

            var methods = typeof(LocationFailureHandler).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attribute = Attribute.GetCustomAttribute(method, typeof(FailureHandlingMethodAttribute)) as FailureHandlingMethodAttribute;
                if (attribute != null)
                {
                    availableMethods[method.Name] = method;
                }
            }
        }

        public static string[] GetAvailableMethods()
        {
            RegisterFailureHandlingMethods();
            return new List<string>(availableMethods.Keys).ToArray();
        }

        public bool HandleFailure(Vector2d location)
        {
            if (location == null)
            {
                // There are no locations to handle
                return false;
            }
            foreach (FailureMethod method in failureMethods)
            {
                var location2d = Conversions.StringToLatLon(method.QueriedLocation.Value);
                if (Vector2d.Equals(location2d, location))
                {
                    foreach (string methodName in method.PriorityMethods)
                    {
                        if (availableMethods.TryGetValue(methodName, out var methodInfo))
                        {
                            FailureHandlingOutcome outcome = InvokeMethod(methodInfo, method);
                            switch (outcome)
                            {
                                case FailureHandlingOutcome.Stop:
                                    return true;
                                case FailureHandlingOutcome.Abort:
                                    return false;
                                case FailureHandlingOutcome.Continue:
                                    continue;
                            }
                        }
                    }
                    return false; // If we've gone through all methods without a Stop or Abort
                }
            }
            return false; // If no matching FailureMethod was found
        }

        private FailureHandlingOutcome InvokeMethod(MethodInfo methodInfo, FailureMethod failureMethod)
        {
            var parameters = methodInfo.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(FailureMethod))
                {
                    args[i] = failureMethod;
                }
                else if (parameters[i].ParameterType == typeof(LocationVariable))
                {
                    args[i] = failureMethod.QueriedLocation;
                }
                else if (parameters[i].ParameterType == typeof(List<LocationVariable>))
                {
                    args[i] = failureMethod.BackupLocations;
                }
                // Add more parameter types as needed
            }

            object result = methodInfo.Invoke(this, args);

            // Interpret the result
            if (result is FailureHandlingOutcome outcome)
            {
                return outcome;
            }
            else if (result is bool boolResult)
            {
                return boolResult ? FailureHandlingOutcome.Stop : FailureHandlingOutcome.Continue;
            }
            else if (methodInfo.ReturnType == typeof(void))
            {
                return FailureHandlingOutcome.Continue;
            }
            else
            {
                // For any other return type, consider it a success if non-null
                return result != null ? FailureHandlingOutcome.Stop : FailureHandlingOutcome.Continue;
            }
        }

        [FailureHandlingMethod]
        private FailureHandlingOutcome UseBackupLocation(FailureMethod failureMethod)
        {
            // Check if there are any backup locations
            if (failureMethod.BackupLocations == null || failureMethod.BackupLocations.Count == 0)
            {
                return FailureHandlingOutcome.Continue;
            }

            // Iterate through all backup locations
            foreach (var backupLocation in failureMethod.BackupLocations)
            {
                // Check if the backup location is the same as the queried location
                if (backupLocation.Equals(failureMethod.QueriedLocation))
                {
                    Debug.Log($"Skipping backup location {backupLocation} as it's the same as the queried location.");
                    continue;
                }

                // Check if the backup location is accessible
                if (IsLocationAccessible(backupLocation))
                {
                    var engine = failureMethod.GetEngine();
                    if (engine != null)
                    {
                        var map = engine.GetMap();
                        if (map != null)
                        {
                            map.HideLocationMarker(failureMethod.QueriedLocation);
                            bool updateText = failureMethod.UpdateLocationText;
                            map.ShowLocationMarker(backupLocation, updateText, failureMethod.QueriedLocation.Key);
                        }
                    }

                    failureMethod.QueriedLocation.Apply(SetOperator.Assign, backupLocation);
                    failureMethod.IsHandled = true;
                    return FailureHandlingOutcome.Stop;
                }
                else
                {
                    Debug.Log($"Backup location {backupLocation} is inaccessible. Trying next backup location.");
                }
            }

            // If we've gone through all backup locations and none were suitable
            return FailureHandlingOutcome.Continue;
        }

        private bool IsLocationAccessible(LocationVariable location)
        {
            // Implement your logic to check if a location is accessible using some server side or client side logic
            // This could involve checking GPS coordinates, network connectivity, or any other relevant factors
            // Return true if the location is accessible, false otherwise

            // For now, we'll just return true for demonstration purposes
            return true;
        }

        [FailureHandlingMethod]
        private void IncreaseRadius(float radiusIncrease)
        {
            // Increase the radius of the location - we need to ensure that we run a boolean check to see if the radius has been increased
            // if it has we need to move to next method in the list - unless continued increase is desired
        }

        [FailureHandlingMethod]
        private void ExecuteAnyway()
        {
            // Execute the node anyway - likely needs to be generic to allow for node or order execution
            // If we find the location on a node we simply remove the location requirement - almost need a list to store this info?
            // sets ishandled to true when executed
        }

        [FailureHandlingMethod]
        private void UseNearestLocation()
        {
            // Use the nearest location by getting all location variables from the engine
            // sets ishandled to true when executed
        }

        [FailureHandlingMethod]
        private FailureHandlingOutcome JumpToNode(FailureMethod failureMethod)
        {
            var engine = failureMethod.GetEngine();
            // If we find a node and the engine is available then we can jump to the node
            if (engine != null && failureMethod.BackupNode != null)
            {
                int index = 0;
                if (failureMethod.StartIndex >= 0 && failureMethod.StartIndex <= failureMethod.BackupNode.OrderList.Count)
                {
                    index = failureMethod.StartIndex;
                }
                engine.ExecuteNode(failureMethod.BackupNode, index);
                // Do we hide the location also?
                failureMethod.IsHandled = true;
                return FailureHandlingOutcome.Stop;
            }

            // Otherwise, we continue to the next method
            return FailureHandlingOutcome.Continue;
        }

        [FailureHandlingMethod]
        private void DisbaleLocationBehaviour()
        {
            // Disable the behaviour related to given location - likely needs to be generic to allow for node or order execution
            // sets ishandled to true when executed
        }
    }

    [InitializeOnLoad]
    public class LocationFailureHandlerInitializer
    {
        static LocationFailureHandlerInitializer()
        {
            LocationFailureHandler.GetAvailableMethods(); // This will trigger method registration
        }
    }

    [Serializable]
    public class FailureMethod
    {
        [Tooltip("The location that this method is associated with")]
        [SerializeField] protected LocationVariable queriedLocation;
        [Tooltip("A list of locations that can be used as alternatives")]
        [SerializeField] protected List<LocationVariable> backupLocations = new List<LocationVariable>();
        [Tooltip("A list of methods that can be executed to handle the failure")]
        [SerializeField] protected List<string> priorityMethods = new List<string>();
        [Tooltip("Whether the failure has been handled")]
        [SerializeField] protected bool isHandled = false;
        [Tooltip("Whether the radius of the location can be increased more than once")]
        [SerializeField] protected bool allowContinuousIncrease = false;
        [Tooltip("Whether the location text should be updated when the location is changed")]
        [SerializeField] protected bool updateLocationText = false;
        [Tooltip("The node to jump to if the location is inaccessible")]
        [SerializeField] protected Node backupNode;
        [Tooltip("The index of the order list to start from on the backup node")]
        [SerializeField] protected int startIndex = 0;

        // This is a special case where we need to ensure that the radius has been increased
        private bool hasIncreased = false;

        public LocationVariable QueriedLocation { get => queriedLocation; }
        public List<LocationVariable> BackupLocations { get => backupLocations; }
        public List<string> PriorityMethods { get => priorityMethods; }
        public bool IsHandled { get => isHandled; set => isHandled = value; }
        public bool AllowContinuousIncrease { get => allowContinuousIncrease; }
        public bool UpdateLocationText { get => updateLocationText; }
        public bool HasIncreased { get => hasIncreased; }
        public Node BackupNode { get => backupNode; }
        public int StartIndex { get => startIndex; }

        public BasicFlowEngine GetEngine()
        {
            var engine = BasicFlowEngine.CachedEngines.Find(e => e != null);
            if (engine == null)
            {
                engine = UnityEngine.Object.FindObjectOfType<BasicFlowEngine>();
            }
            return engine;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    /// <summary>
    /// In this case this class spawns a location info panel with a button
    /// Thus the Order requires an event to set the button to trigger
    /// </summary>
    [OrderInfo("Maps", "Show Location Info Panel - Nino", "Show the location info panel for the current location with a button")]
    public class ShowLocationInfoPanel_Nino : ShowLocationInfoPanel
    {
        [HideInInspector]
        [Tooltip("The Node to execute upon button click. This will override any other action the button may have.")]
        [SerializeField] protected Node executeNode;
        [HideInInspector]
        [Tooltip("The event to trigger when the button is clicked. This will not be used if a Node is set.")]
        [SerializeField] protected UnityEngine.Events.UnityEvent buttonEvent;

        public override void OnEnter()
        {
            if (customPanelPrefab != null)
            {
                LocationInfoPanel_Nino.CustomLocationPrefab = customPanelPrefab;
            }

            // When passing in 'true' to GetLocationInfoPanel we are requesting a LocationInfoPanel_Nino prefab
            LocationInfoPanel_Nino panel = LocationInfoPanel.GetLocationInfoPanel(true) as LocationInfoPanel_Nino;
            bool hasInfo = false;
            if (panel != null)
            {
                if (customLocation.Value != null)
                {
                    panel.SetLocationInfo(customLocation.Value);
                    hasInfo = true;

                    // If a Node is set then we set the button event to execute the Node
                    if (executeNode != null)
                    {
                        panel.SetButtonEvent(() => executeNode.StartExecution());
                    }
                    else if (buttonEvent != null && buttonEvent.GetPersistentEventCount() > 0)
                    {
                        panel.SetButtonEvent(buttonEvent.Invoke);
                    }
                }
                else
                {
                    var locInfo = ParentNode.NodeLocation;
                    if (locInfo == null)
                    {
                        LocationClickEventHandler handler = ParentNode._EventHandler as LocationClickEventHandler;
                        if (handler != null)
                        {
                            locInfo = handler.Location.locationRef;
                        }
                    }
                    if (locInfo != null)
                    {
                        panel.SetLocationInfo(locInfo.Value);
                        hasInfo = true;

                        // If a Node is set then we set the button event to execute the Node
                        if (executeNode != null)
                        {
                            panel.SetButtonEvent(() => executeNode.StartExecution());
                        }
                        else if (buttonEvent != null && buttonEvent.GetPersistentEventCount() > 0)
                        {
                            panel.SetButtonEvent(buttonEvent.Invoke);
                        }
                    }
                }
                if (hasInfo)
                {
                    panel.ToggleMenu();
                }
            }
            Continue();
        }

        public override Color GetButtonColour()
        {
            return new Color(0.5f, 0.5f, 1.0f);
        }

        public override string GetSummary()
        {
            string location = customLocation.Value != null ? customLocation.Value.Name : "current location";
            return $"   Show location info panel for {location} with a button";
        }

        public override void GetConnectedNodes(ref List<Node> connectedNodes)
        {
            if (executeNode != null)
            {
                connectedNodes.Add(executeNode);
            }
        }
    }
}

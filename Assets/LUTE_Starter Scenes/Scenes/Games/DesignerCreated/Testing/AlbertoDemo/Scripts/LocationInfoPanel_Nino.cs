using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LoGaCulture.LUTE
{
    /// <summary>
    /// For Nino
    /// Essentially a location info panel but includes the use of a button that will trigger an event (typically exeution of another Node)
    /// Used in combination with the Order: 'ShowLocationInfoPanel_Nino'
    /// </summary>
    public class LocationInfoPanel_Nino : LocationInfoPanel
    {
        // The behaviour that will trigger when the button is pressed; typically execution of a Node
        private UnityAction buttonEvent;

        [Tooltip("The button that this panel owns; an attempt to find this automatically will be made if not set in the inspector")]
        [SerializeField] protected Button panelButton;
        [Tooltip("Whether to disable this object upon button click")]
        [SerializeField] protected bool disableOnButtonClick = true;

        protected override void Start()
        {
            base.Start();

            // If there is no button set in the inspector then attempt to find one either on the object or in children
            if (panelButton == null)
            {
                panelButton = GetComponent<Button>();
            }
            if (panelButton == null)
            {
                panelButton = GetComponentInChildren<Button>();
            }
        }

        protected override void Update()
        {
            // Ensure we call update on the base class to set other properties and check null refs
            base.Update();

            // If there is no button found then skip
            if (panelButton == null)
            {
                return;
            }

            // If there is a button but no event for it then we ensure it is disabled to avoid confusing players
            if (buttonEvent == null)
            {
                panelButton.interactable = false;
                return;
            }

            // If the button has an event set then set the onClick event to trigger the event (if there is an event)
            panelButton.onClick.RemoveAllListeners();

            // Add to the event so that the button disables the menu upon click - could be an optional boolean on the Order if required
            if (disableOnButtonClick)
            {
                buttonEvent += () =>
                {
                    infoPanelActive = false;
                    this.SetActive(false);
                };
            }

            panelButton.onClick.AddListener(buttonEvent);
        }

        /// <summary>
        /// Once a location info panel has been created and the button requires an event to be set, this method should be called to set the event
        /// </summary>
        /// <param name="action"></param>
        public virtual void SetButtonEvent(UnityAction action)
        {
            if (action != null)
            {
                buttonEvent = action;
            }
        }
    }
}

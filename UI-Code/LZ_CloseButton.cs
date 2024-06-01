using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.EventSystems;

// Simply adds a listener to the button object it's attached to that will close the selected window prefab object
// NOTE: I am not 100% certain if this actually properly closes the window in the same way that clicking the plugin button does
// It depends on how the plugin button actually works when it creates the ui window.
// If it creates a parent 'window' gameobject and puts the prefab inside, then this close button is only de-activating the child prefab, whereas the
// plugin button seems like it's pointed at the parent window.
// But since this does work, it might be doing doing the same as the plugin window.

namespace LZ_TrackingDetection
{
    class LZ_CloseButton : MonoBehaviour
    {
        public GameObject windowPrefab;
        private Button closeButton;

        public void Start()
        {
            // Get button!
            closeButton = GetComponent(typeof(Button)) as Button;
            // Add listener to if button is pressed. It will run ButtonPressCheck if it is!
            closeButton.onClick.AddListener(delegate { CloseButtonClicked(); });
        }

        public void CloseButtonClicked()
        {
            this.windowPrefab.SetActive(false);
        }
    }
}

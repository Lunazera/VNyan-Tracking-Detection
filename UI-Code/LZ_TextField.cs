using System;
using UnityEngine;
using UnityEngine.UI;

namespace LZ_TrackingDetection
{
    class LZ_TextField : MonoBehaviour
    {
        // You will want to edit the name. This is the name the VNyanParameter will have in VNyan!
        public string fieldName;
        // If no other value was set we have the field set to a blank string
        public string fieldValue = "";
        private InputField mainField;
        private Button mainButton;

        public void Start()
        {
            // We add the inputfield as the mainfield
            mainField = GetComponent(typeof(InputField)) as InputField;
            // We add a button as confirmation to change the inputted value
            mainButton = GetComponentInChildren(typeof(Button)) as Button;

            // We add a listener that will run ButtonPressCheck if the button is pressed.
            mainButton.onClick.AddListener(delegate { ButtonPressCheck(); });

            // Here we either want to load a existing parameter or add one!
            // A loaded parameter comes from Sja_UICore when it loads the setting Json into the dictionary!
            if (Sja_UICore.VNyanParameters.ContainsKey(fieldName))
            {
                // If the parameter exist, set fieldValue to that value.
                fieldValue = Sja_UICore.VNyanParameters[fieldName];
                // We want to try to show the user the current value. We set the field text to fieldValue.
                mainField.text = fieldValue;
                // Lastly we set the VNyanParameter to this value!
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(fieldName, fieldValue);
            }
            else
            {
                // If it was the first time and there is no parameter we want to add it!
                // field name is the name of the parameter and fieldvalue will be the value.
                Sja_UICore.VNyanParameters.Add(fieldName, fieldValue);
                // We want to try to show the value at all times.
                mainField.text = fieldValue;
                // Set the Vnyan parameter to this name and value!
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(fieldName, fieldValue);
            }
        }

        public void ButtonPressCheck()
        {
            // Set the new value!
            Sja_UICore.VNyanParameters[fieldName] = mainField.text;
            // Set the VNyanparameter!
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(fieldName, mainField.text);
        }
    }
}

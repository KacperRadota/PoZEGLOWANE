using DataStorage;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ChosenBuoyTextController : MonoBehaviour
    {
        private void Start()
        {
            var textField = GetComponent<TextMeshProUGUI>();
            var textToDisplay = "WYBRANA BOJA: ";
            switch (PlayerPrefs.GetString(PPKeys.ChosenBuoyColour))
            {
                case PPValues.RedBuoy:
                    textToDisplay += "CZERWONA";
                    break;
                case PPValues.GreenBuoy:
                    textToDisplay += "ZIELONA";
                    break;
            }

            textField.text = textToDisplay;
        }
    }
}
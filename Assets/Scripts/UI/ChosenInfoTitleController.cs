using DataStorage;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ChosenInfoTitleController : MonoBehaviour
    {
        private static TextMeshProUGUI _title;

        private void Start()
        {
            _title = GetComponent<TextMeshProUGUI>();
            SetTitle();
        }

        private static void SetTitle()
        {
            _title.text = PlayerPrefs.GetString(PPKeys.ChosenInfoContent) switch
            {
                PPValues.SafetyMatters => "Bezpieczeństwo",
                PPValues.Commands => "Komendy",
                PPValues.YachtArchitecture => "Budowa Jachtu",
                PPValues.Nautics => "Locja",
                PPValues.FAQ => "FAQ i Porady",
                PPValues.ScoringRules => "Zasady ŻaGGsowania",
                PPValues.Knots => "Węzły",
                _ => ""
            };
        }
    }
}
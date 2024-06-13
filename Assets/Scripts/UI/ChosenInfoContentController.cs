using DataStorage;
using UnityEngine;

namespace UI
{
    public class ChosenInfoContentController : MonoBehaviour
    {
        private ChosenInfoContentStore chosenInfoContentStore;

        private void Start()
        {
            chosenInfoContentStore = FindObjectOfType<ChosenInfoContentStore>();
            SetContent();
        }

        private void SetContent()
        {
            var contentParent = PlayerPrefs.GetString(PPKeys.ChosenInfoContent) switch
            {
                PPValues.SafetyMatters => chosenInfoContentStore.SafetyMattersContent,
                PPValues.Commands => chosenInfoContentStore.CommandsContent,
                PPValues.YachtArchitecture => chosenInfoContentStore.YachtArchitectureContent,
                PPValues.Nautics => chosenInfoContentStore.NauticsContent,
                PPValues.FAQ => chosenInfoContentStore.FAQContent,
                PPValues.ScoringRules => chosenInfoContentStore.ScoringRulesContent,
                PPValues.Knots => chosenInfoContentStore.KnotsContent,
                _ => null
            };
            if (contentParent is null) return;
            var children = contentParent.GetComponentsInChildren<RectTransform>();
            foreach (var child in children)
            {
                Instantiate(original: child, parent: transform);
            }
        }
    }
}
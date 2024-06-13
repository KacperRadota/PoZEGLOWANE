using UnityEngine;

namespace DataStorage
{
    public class ChosenInfoContentStore : MonoBehaviour
    {
        public GameObject SafetyMattersContent
        {
            get => safetyMattersContent;
            set => safetyMattersContent = value;
        }

        public GameObject CommandsContent
        {
            get => commandsContent;
            set => commandsContent = value;
        }

        public GameObject YachtArchitectureContent
        {
            get => yachtArchitectureContent;
            set => yachtArchitectureContent = value;
        }

        public GameObject NauticsContent
        {
            get => nauticsContent;
            set => nauticsContent = value;
        }

        public GameObject FAQContent
        {
            get => faqContent;
            set => faqContent = value;
        }

        public GameObject ScoringRulesContent
        {
            get => scoringRulesContent;
            set => scoringRulesContent = value;
        }

        public GameObject KnotsContent
        {
            get => knotsContent;
            set => knotsContent = value;
        }

        [SerializeField] private GameObject safetyMattersContent;
        [SerializeField] private GameObject commandsContent;
        [SerializeField] private GameObject yachtArchitectureContent;
        [SerializeField] private GameObject nauticsContent;
        [SerializeField] private GameObject faqContent;
        [SerializeField] private GameObject scoringRulesContent;
        [SerializeField] private GameObject knotsContent;
    }
}
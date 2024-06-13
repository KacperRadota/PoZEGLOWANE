using DataStorage;
using UnityEngine;

namespace Managers
{
    public class ChosenInfoManager : MonoBehaviour
    {
        public void GoToSafetyMatters()
        {
            GoTo(PPValues.SafetyMatters);
        }

        public void GoToCommands()
        {
            GoTo(PPValues.Commands);
        }

        public void GoToYachtArchitecture()
        {
            GoTo(PPValues.YachtArchitecture);
        }

        public void GoToNautics()
        {
            GoTo(PPValues.Nautics);
        }

        public void GoToFAQ()
        {
            GoTo(PPValues.FAQ);
        }

        public void GoToScoringRules()
        {
            GoTo(PPValues.ScoringRules);
        }

        public void GoToKnots()
        {
            GoTo(PPValues.Knots);
        }

        private static void GoTo(string ppValue)
        {
            PlayerPrefs.SetString(PPKeys.ChosenInfoContent, ppValue);
            PlayerPrefs.Save();
            SceneChanger.Instance.ChangeToChosenInfoScene();
        }
    }
}
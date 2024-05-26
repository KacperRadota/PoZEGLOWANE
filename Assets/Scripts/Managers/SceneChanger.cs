using DataStorage;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneChanger : MonoBehaviour
    {
        private static void ChangeSceneTo(string name)
        {
            SceneManager.LoadScene(name);
        }

        private static void ChangeToScorerSceneWith(string buoyPPValue)
        {
            PlayerPrefs.SetString(PPKeys.ChosenBuoyColour, buoyPPValue);
            PlayerPrefs.Save();
            ChangeSceneTo(Scenes.ScorerScene);
        }

        public static void ChangeToMainScene()
        {
            ChangeSceneTo(Scenes.MainScene);
        }

        public static void ChangeToChooseScorerScene()
        {
            ChangeSceneTo(Scenes.ChooseScorerScene);
        }

        public static void ChangeToRedScorerScene()
        {
            ChangeToScorerSceneWith(PPValues.RedBuoy);
        }

        public static void ChangeToGreenScorerScene()
        {
            ChangeToScorerSceneWith(PPValues.GreenBuoy);
        }
    }
}
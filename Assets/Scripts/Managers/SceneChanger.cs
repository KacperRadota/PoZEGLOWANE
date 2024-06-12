using System.Threading.Tasks;
using DataStorage;
using ObjectTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneChanger : MonoBehaviour
    {
        public static SceneChanger Instance;
        private static readonly int StartTrigger = Animator.StringToHash("Start");
        private const int BasicLayer = 0;
        private Animator _transition;

        private void Awake()
        {
            _transition = FindObjectOfType<Crossfade>().GetComponent<Animator>();
            if (Instance) return;
            Instance = this;
        }

        private void ChangeSceneTo(string sceneName)
        {
            LoadLevelWithAnimation(sceneName, _transition);
            return;

            static async void LoadLevelWithAnimation(string sceneName, Animator transition)
            {
                var task = DataController.Instance.RecalculateCurrentScoreOfCurrentBoatIfApplicable();
                transition.SetTrigger(StartTrigger);
                while (transition.IsInTransition(BasicLayer))
                {
                    await Task.Yield();
                }

                await task;

                SceneManager.LoadScene(sceneName);
            }
        }

        private void ChangeToScorerSceneWith(string buoyPPValue)
        {
            PlayerPrefs.SetString(PPKeys.ChosenBuoyColour, buoyPPValue);
            PlayerPrefs.Save();
            ChangeSceneTo(Scenes.ScorerScene);
        }

        public void ChangeToMainScene()
        {
            ChangeSceneTo(Scenes.MainScene);
        }

        public void ChangeToChooseScorerScene()
        {
            ChangeSceneTo(Scenes.ChooseScorerScene);
        }

        public void ChangeToRedScorerScene()
        {
            ChangeToScorerSceneWith(PPValues.RedBuoy);
        }

        public void ChangeToGreenScorerScene()
        {
            ChangeToScorerSceneWith(PPValues.GreenBuoy);
        }

        public void ChangeToInfoScene()
        {
            ChangeSceneTo(Scenes.InfoScene);
        }
    }
}
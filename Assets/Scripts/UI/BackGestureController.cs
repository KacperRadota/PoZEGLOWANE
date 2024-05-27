using System.Collections;
using DataStorage;
using Managers;
using ObjectTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class BackGestureController : MonoBehaviour
    {
        private bool _waitingForSecondBack;
        private float _firstBackTime;
        private const float TimeBetweenBacks = 3f;
        private ScoreManager _scoreManager;
        private ScoreNotSavedPopUp _scoreNotSavedPopUp;

        private void Awake()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (!SceneManager.GetActiveScene().name.Equals(Scenes.ScorerScene)) return;
            _scoreManager = FindObjectOfType<ScoreManager>();
            _scoreNotSavedPopUp = FindObjectOfType<ScoreNotSavedPopUp>(true);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            switch (SceneManager.GetActiveScene().name)
            {
                case Scenes.MainScene:
                    ExitAppHandler();
                    return;
                case Scenes.ChooseScorerScene:
                    SceneChanger.Instance.ChangeToMainScene();
                    return;
                case Scenes.ScorerScene:
                    _scoreManager.GoToPreviousScreenIfApplicable(_scoreNotSavedPopUp.gameObject);
                    return;
                default:
                    SceneChanger.Instance.ChangeToMainScene();
                    return;
            }
        }

        private void ExitAppHandler()
        {
            if (_waitingForSecondBack)
            {
                Application.Quit();
            }
            else
            {
                ToastMaker.ShowToast("Aby wyjść z aplikacji, cofnij jeszcze raz");
                _waitingForSecondBack = true;
                _firstBackTime = Time.time;
                StartCoroutine(WaitForSecondBack());
            }
        }

        private IEnumerator WaitForSecondBack()
        {
            while (_waitingForSecondBack)
            {
                yield return null;
                if (Time.time - _firstBackTime > TimeBetweenBacks)
                {
                    _waitingForSecondBack = false;
                }
            }
        }
    }
}
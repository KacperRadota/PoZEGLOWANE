using DataStorage;
using TMPro;
using UI;
using UnityEngine;
using static DataStorage.DataClasses.Boats.Boat;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI score;
        [Space] [SerializeField] private int redScoreIncrement = 1;
        [SerializeField] private int greenScoreIncrement = 2;

        private int _currentScore;
        private Pulsator _scorePulsator;
        private AudioSource _audioCollectSFX;

        public void GoToPreviousScreenIfApplicable(GameObject questionPopUp)
        {
            if (_currentScore == 0)
            {
                SceneChanger.Instance.ChangeToChooseScorerScene();
            }
            else
            {
                PopUpManager.Instance.OpenPopUp(questionPopUp);
            }
        }

        public void SaveScore()
        {
            var scoringEvent = new ScoringEvent()
            {
                score = _currentScore,
                timeStamp = System.DateTime.Now.ToString()
            };
            DataController.Instance.LoadBoats();
            DataController.Instance.boats.currentlyChosenBoat.scoringEvents.Add(scoringEvent);
            DataController.Instance.SaveBoats();
        }

        public void IncrementScore()
        {
            ChangeScore(1);
        }

        public void DecrementScore()
        {
            ChangeScore(-1);
        }

        private void Start()
        {
            _scorePulsator = score.GetComponentInParent<Pulsator>();
            _audioCollectSFX = GetComponent<AudioSource>();
            HandleScoreChange(true);
        }

        private void ChangeScore(int unitSign)
        {
            switch (PlayerPrefs.GetString(PPKeys.ChosenBuoyColour))
            {
                case PPValues.RedBuoy:
                    _currentScore += unitSign * redScoreIncrement;
                    break;
                case PPValues.GreenBuoy:
                    _currentScore += unitSign * greenScoreIncrement;
                    break;
            }

            _currentScore = Mathf.Clamp(_currentScore, 0, 9999);

            HandleScoreChange();
        }

        private void HandleScoreChange(bool skipEffects = false)
        {
            score.text = $"+{_currentScore}";
            if (skipEffects) return;
            _scorePulsator.PlayPulsatingEffect();
            if (_audioCollectSFX.isPlaying)
            {
                _audioCollectSFX.Stop();
            }

            _audioCollectSFX.Play();
        }
    }
}
using DataStorage;
using TMPro;
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
            DisplayCurrentScore();
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

            DisplayCurrentScore();
        }

        [ContextMenu("Print All Scores")]
        private void PrintAllScores()
        {
            DataController.Instance.LoadBoats();
            var boats = DataController.Instance.boats;
            foreach (var boat in boats.boatsList)
            {
                Debug.Log($"Boat name: {boat.boatName}");
                foreach (var crewMemberName in boat.crewMembersNames)
                {
                    Debug.Log(crewMemberName);
                }

                foreach (var scoringEvent in boat.scoringEvents)
                {
                    Debug.Log($"{scoringEvent.timeStamp} - {scoringEvent.score}");
                }
            }
        }

        private void DisplayCurrentScore()
        {
            score.text = $"+{_currentScore}";
        }
    }
}
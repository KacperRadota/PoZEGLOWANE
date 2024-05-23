using DataStorage;
using TMPro;
using UnityEngine;
using static DataStorage.DataClasses.ScoringEvents;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI score;
        [Space] [SerializeField] private int redScoreIncrement = 1;
        [SerializeField] private int greenScoreIncrement = 2;

        private int _currentScore;


        public void IncrementRed()
        {
            IncrementScore(redScoreIncrement);
        }

        public void IncrementGreen()
        {
            IncrementScore(greenScoreIncrement);
        }

        public void SaveScore()
        {
            var scoringEvent = new ScoringEvent
            {
                Score = _currentScore,
                TimeStamp = System.DateTime.Now.ToString()
            };
            var time = System.Diagnostics.Stopwatch.GetTimestamp();
            DataController.Instance.LoadScoringEvents();
            DataController.Instance.ScoringEvents.Add(scoringEvent);
            DataController.Instance.SaveScoringEvents();
            Debug.Log($"JSON serialisation time: {System.Diagnostics.Stopwatch.GetTimestamp() - time}");
        }

        [ContextMenu("Print All Scores")]
        private void PrintAllScores()
        {
            DataController.Instance.LoadScoringEvents();
            var scoringEvents = DataController.Instance.ScoringEvents;
            foreach (var scoringEvent in scoringEvents)
            {
                Debug.Log(scoringEvent.TimeStamp + " - " + scoringEvent.Score);
            }
        }


        private void Start()
        {
            DisplayCurrentScore();
        }

        private void DisplayCurrentScore()
        {
            score.text = $"+{_currentScore}";
        }

        private void IncrementScore(int value)
        {
            _currentScore += value;
            DisplayCurrentScore();
        }
    }
}
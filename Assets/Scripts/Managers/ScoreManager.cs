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
            var scoringEvent = new ScoringEvent()
            {
                score = _currentScore,
                timeStamp = System.DateTime.Now.ToString()
            };
            DataController.Instance.LoadBoats();
            DataController.Instance.boats.currentlyChosenBoat.scoringEvents.Add(scoringEvent);
            DataController.Instance.SaveBoats();
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
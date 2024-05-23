using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static DataStorage.DataClasses;
using static DataStorage.DataClasses.ScoringEvents;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace DataStorage
{
    public class DataController : MonoBehaviour
    {
        public static DataController Instance { get; private set; }
        public List<ScoringEvent> ScoringEvents;
        private ScoringEvents _scoringEventsObj;

        private const string ScoringEventsFileName = "scoringEvents.json";
        private string _scoringEventsDataPath;

        private void Awake()
        {
            HandleInstance();
            _scoringEventsDataPath = Path.Combine(Application.persistentDataPath, ScoringEventsFileName);
            Application.targetFrameRate = 90;
            LoadAllData();
            return;

            void HandleInstance()
            {
                if (!Instance)
                {
                    Instance = this;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        private void LoadAllData()
        {
            LoadScoringEvents();
        }

        public void SaveScoringEvents()
        {
            List<string> scoringEventsJsonList = new();
            foreach (var scoringEvent in ScoringEvents)
            {
                var json = JsonUtility.ToJson(scoringEvent);
                scoringEventsJsonList.Add(json);
            }

            _scoringEventsObj.ScoringEventsJsonList = scoringEventsJsonList;


            var jsonData = JsonUtility.ToJson(_scoringEventsObj);
            File.WriteAllText(_scoringEventsDataPath, jsonData);
        }

        public void LoadScoringEvents()
        {
            if (File.Exists(_scoringEventsDataPath))
            {
                var jsonData = File.ReadAllText(_scoringEventsDataPath);
                if (_scoringEventsObj is null)
                {
                    _scoringEventsObj = JsonUtility.FromJson<ScoringEvents>(jsonData);
                }
                else
                {
                    JsonUtility.FromJsonOverwrite(jsonData, _scoringEventsObj);
                }

                List<ScoringEvent> scoringEvents = new();
                foreach (var scoringEventJson in _scoringEventsObj.ScoringEventsJsonList)
                {
                    var scoringEvent = JsonUtility.FromJson<ScoringEvent>(scoringEventJson);
                    scoringEvents.Add(scoringEvent);
                }

                ScoringEvents = scoringEvents;
            }
            else
            {
                ScoringEvents = new List<ScoringEvent>();
                _scoringEventsObj = new ScoringEvents
                {
                    ScoringEventsJsonList = new List<string>()
                };
            }

            SaveScoringEvents();
        }
    }
}
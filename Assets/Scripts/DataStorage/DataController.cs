using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using static DataStorage.DataClasses;
using static DataStorage.DataClasses.Boats;
using static DataStorage.DataClasses.Boats.Boat;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace DataStorage
{
    public class DataController : MonoBehaviour
    {
        //TODO: uncomment webGL compiler checks
// #if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern bool IsIOS();

        [DllImport("__Internal")]
        private static extern void SaveToLocalStorage(string key, string value);

        [DllImport("__Internal")]
        private static extern string LoadFromLocalStorage(string key);
// #endif

        public static DataController Instance { get; private set; }
        public Boats boats;

        private const string BoatsFileName = "boats.json";
        private string _boatsDataPath;
        private bool _isWebIOS;

        private void Awake()
        {
            if (!HandleInstance()) return;
            _boatsDataPath = Path.Combine(Application.persistentDataPath, BoatsFileName);
            _isWebIOS = false;
// #if UNITY_WEBGL && !UNITY_EDITOR
            _isWebIOS = IsIOS();
// #endif
            Application.targetFrameRate = 90;
            LoadAllData();
            return;

            bool HandleInstance()
            {
                if (!Instance)
                {
                    Instance = this;
                    transform.SetParent(null);
                    DontDestroyOnLoad(this);
                    return true;
                }

                Destroy(gameObject);
                return false;
            }
        }

        private void Start()
        {
            RecalculateCurrentScoreOfCurrentBoatIfApplicable();
        }

        private void RecalculateCurrentScoreOfCurrentBoatIfApplicable()
        {
            LoadBoats();
            var currentlyChosenBoat = Instance.boats.currentlyChosenBoat;
            if (currentlyChosenBoat.scoringEvents.Count == currentlyChosenBoat.lastCalculatedScoreListCount) return;
            var score = Instance.boats.currentlyChosenBoat.scoringEvents.Sum(scoringEvent =>
                scoringEvent.score);
            currentlyChosenBoat.lastCalculatedScore = score;
            currentlyChosenBoat.lastCalculatedScoreListCount = currentlyChosenBoat.scoringEvents.Count;
            SaveBoats();
        }

        private void LoadAllData()
        {
            LoadBoats();
        }

        private static void SetPopUpShowInfo()
        {
            PlayerPrefs.SetInt(PPKeys.WasDownloadInfoShown, PPValues.False);
            PlayerPrefs.Save();
        }

        public void SaveBoats()
        {
            var isCurrentBoatPresent = false;
            for (var i = 0; i < boats.boatsList.Count; i++)
            {
                if (boats.boatsList[i].id != boats.currentlyChosenBoat.id) continue;
                isCurrentBoatPresent = true;
                boats.boatsList[i] = boats.currentlyChosenBoat;
            }

            if (!isCurrentBoatPresent)
            {
                boats.boatsList.Add(boats.currentlyChosenBoat);
            }

            var jsonData = JsonUtility.ToJson(boats);
            File.WriteAllText(_boatsDataPath, jsonData);
        }

        public void LoadBoats()
        {
            if (_isWebIOS)
            {
                LoadBoatsWithLocalStorage();
            }
            else
            {
                LoadBoatsWithPersistentDataPath();
            }
        }

        private void LoadBoatsWithLocalStorage()
        {
        }

        private void LoadBoatsWithPersistentDataPath()
        {
            if (File.Exists(_boatsDataPath))
            {
                var jsonData = File.ReadAllText(_boatsDataPath);
                if (boats is null)
                {
                    boats = JsonUtility.FromJson<Boats>(jsonData);
                }
                else
                {
                    JsonUtility.FromJsonOverwrite(jsonData, boats);
                }
            }
            else
            {
                boats = new Boats()
                {
                    boatsList = new List<Boat>(),
                    currentlyChosenBoat = new Boat()
                    {
                        id = 0,
                        boatName = "",
                        checkInDay = "",
                        checkInMonth = "",
                        checkInYear = "",
                        checkOutDay = "",
                        checkOutMonth = "",
                        checkOutYear = "",
                        crewMembersNames = new List<string>(),
                        notes = "",
                        scoringEvents = new List<ScoringEvent>(),
                        lastCalculatedScore = 0,
                        lastCalculatedScoreListCount = 0
                    }
                };
                SetPopUpShowInfo();
                SaveBoats();
            }
        }

        public ulong GetNextAvailableID()
        {
            LoadBoats();
            var existingIDs = new SortedSet<ulong>(boats.boatsList.Select(boat => boat.id));
            ulong nextID = 0;
            foreach (var unused in existingIDs.TakeWhile(id => id == nextID))
            {
                nextID++;
            }

            return nextID;
        }
    }
}
// ReSharper disable RedundantUsingDirective

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataStorage.DataClasses;
using static DataStorage.DataClasses.Boats;
using static DataStorage.DataClasses.Boats.Boat;

// ReSharper disable UnusedMember.Local
// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace DataStorage
{
    public class DataController : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsIOS();

        [DllImport("__Internal")]
        private static extern void SaveToLocalStorage(string key, string value);

        [DllImport("__Internal")]
        private static extern string LoadFromLocalStorage(string key);

        [DllImport("__Internal")]
        private static extern bool ExistsInLocalStorage(string key);
#endif

        public static DataController Instance { get; private set; }
        public Boats boats;

        private const string BoatsFileName = "boats.json";
        private const string LocalStorageKey = "BOATS";
        private string _boatsDataPath;
        private bool _isWebIOS;

        private void Awake()
        {
            if (!HandleInstance()) return;
            _boatsDataPath = Path.Combine(Application.persistentDataPath, BoatsFileName);
            Application.targetFrameRate = 90;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SetIsWebIOSAccordingly();
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

            void SetIsWebIOSAccordingly()
            {
                _isWebIOS = false;
#if UNITY_WEBGL && !UNITY_EDITOR
                _isWebIOS = IsIOS();
#endif
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RecalculateCurrentScoreOfCurrentBoatIfApplicable();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            if (_isWebIOS)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                SaveToLocalStorage(LocalStorageKey, jsonData);
#endif
            }
            else
            {
                File.WriteAllText(_boatsDataPath, jsonData);
            }
        }

        public void LoadBoats()
        {
            var alreadyExists = false;
            if (_isWebIOS)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                alreadyExists = ExistsInLocalStorage(LocalStorageKey);
#endif
            }
            else
            {
                alreadyExists = File.Exists(_boatsDataPath);
            }

            if (alreadyExists)
            {
                var jsonData = "";
                if (_isWebIOS)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    jsonData = LoadFromLocalStorage(LocalStorageKey);
#endif
                }
                else
                {
                    jsonData = File.ReadAllText(_boatsDataPath);
                }

                LoadDataFromJson(jsonData);
            }
            else
            {
                CreateNewSaveFile();
            }

            return;

            void LoadDataFromJson(string jsonData)
            {
                if (boats is null)
                {
                    boats = JsonUtility.FromJson<Boats>(jsonData);
                }
                else
                {
                    JsonUtility.FromJsonOverwrite(jsonData, boats);
                }
            }

            void CreateNewSaveFile()
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
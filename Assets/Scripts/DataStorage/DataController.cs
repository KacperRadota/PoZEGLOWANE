// ReSharper disable RedundantUsingDirective

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataStorage.DataClasses;
using static DataStorage.DataClasses.Boats;
using static DataStorage.DataClasses.Boats.Boat;


namespace DataStorage
{
    public class DataController : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsIOS();

        [DllImport("__Internal")]
        private static extern void SaveToIndexedDB(string value);

        [DllImport("__Internal")]
        private static extern string LoadFromIndexedDB(Action<string> action);

        [DllImport("__Internal")]
        private static extern bool ExistsInIndexedDB(Action<int> action);

        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void ExistsInIndexedDB_Callback(int value)
        {
            Instance._isStorageCreated = value != 0;
            Instance._isFirstSetupReady = true;
            Instance.LoadAllData();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        public static void LoadBoats_Callback(string value)
        {
            Instance._loadBoatsCallbackResult = value;
            Instance._isLoadBoatsCallbackCompleted = true;
        }

#endif

        public static DataController Instance { get; private set; }
        public Boats boats;

        private const string BoatsFileName = "boats.json";
        private string _boatsDataPath;
        private bool _isWebIOS;
        private bool _isStorageCreated;
        private bool _isFirstSetupReady;
        private bool _isLoadBoatsCallbackCompleted = true;
        private string _loadBoatsCallbackResult;


        private void Awake()
        {
            if (!HandleInstance()) return;
            _boatsDataPath = Path.Combine(Application.persistentDataPath, BoatsFileName);
            Application.targetFrameRate = 90;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SetIsWebIOSAccordingly();
            CheckIfStorageExists();
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

        private void CheckIfStorageExists()
        {
            if (_isWebIOS)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                ExistsInIndexedDB(ExistsInIndexedDB_Callback);
#endif
            }
            else
            {
                _isStorageCreated = File.Exists(_boatsDataPath);
                _isFirstSetupReady = true;
                LoadAllData();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _ = RecalculateCurrentScoreOfCurrentBoatIfApplicable();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private async void LoadAllData()
        {
            await LoadBoatsAsync();
        }

        private static void SetPopUpShowInfo()
        {
            PlayerPrefs.SetInt(PPKeys.WasDownloadInfoShown, PPValues.False);
            PlayerPrefs.Save();
        }

        public async Task RecalculateCurrentScoreOfCurrentBoatIfApplicable()
        {
            await LoadBoatsAsync();
            var currentlyChosenBoat = Instance.boats.currentlyChosenBoat;
            if (currentlyChosenBoat.scoringEvents.Count == currentlyChosenBoat.lastCalculatedScoreListCount) return;
            var score = Instance.boats.currentlyChosenBoat.scoringEvents.Sum(scoringEvent =>
                scoringEvent.score);
            currentlyChosenBoat.lastCalculatedScore = score;
            currentlyChosenBoat.lastCalculatedScoreListCount = currentlyChosenBoat.scoringEvents.Count;
            SaveBoats();
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
                SaveToIndexedDB(jsonData);
#endif
            }
            else
            {
                File.WriteAllText(_boatsDataPath, jsonData);
            }
        }

        public async Task LoadBoatsAsync()
        {
            if (!_isFirstSetupReady)
            {
                while (!_isFirstSetupReady)
                {
                    // Wait until setup is ready
                    await Task.Yield();
                }
            }

            if (!_isLoadBoatsCallbackCompleted)
            {
                while (!_isLoadBoatsCallbackCompleted)
                {
                    // If other class called LoadBoats(), wait until the result is gained, and return the function
                    await Task.Yield();
                }

                return;
            }

            if (_isStorageCreated)
            {
                var jsonData = _isWebIOS
                    ? await CallExternalLoadFunction()
                    // ReSharper disable once MethodHasAsyncOverload
                    : File.ReadAllText(_boatsDataPath);
                LoadDataFromJson(jsonData);
            }
            else
            {
                CreateNewSaveFile();
            }

            return;

            async Task<string> CallExternalLoadFunction()
            {
                _isLoadBoatsCallbackCompleted = false;
                _loadBoatsCallbackResult = "";
#if UNITY_WEBGL && !UNITY_EDITOR
                LoadFromIndexedDB(LoadBoats_Callback);
#endif
                while (!_isLoadBoatsCallbackCompleted)
                {
                    await Task.Yield();
                }

                return _loadBoatsCallbackResult;
            }

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
                CheckIfStorageExists();
            }
        }


        public ulong GetNextAvailableID()
        {
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
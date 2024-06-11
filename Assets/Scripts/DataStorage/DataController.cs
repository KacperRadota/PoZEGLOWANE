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
// #if UNITY_WEBGL && !UNITY_EDITOR
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
            Debug.Log("'Exists' Callback called");
            Instance._isStorageCreated = value != 0;
            Instance._isFirstSetupReady = true;
            Instance.LoadAllData();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        public static void LoadBoats_Callback(string value)
        {
            Debug.Log("'Load' Callback called");
            Instance._loadBoatsCallbackResult = value;
            Instance._isLoadBoatsCallbackCompleted = true;
        }

// #endif

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
// #if UNITY_WEBGL && !UNITY_EDITOR
                // TODO change later
                _isWebIOS = true;
                Debug.Log($"Is iOS (set from C#): {_isWebIOS}");
                // _isWebIOS = IsIOS();
// #endif
            }
        }

        private void CheckIfStorageExists()
        {
            if (_isWebIOS)
            {
// #if UNITY_WEBGL && !UNITY_EDITOR
                Debug.Log("IsStorageCreated: " + _isStorageCreated);
                Debug.Log("IsFirstSetupReady: " + _isFirstSetupReady);
                Debug.Log("Calling 'Exists' function'");
                ExistsInIndexedDB(ExistsInIndexedDB_Callback);
// #endif
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
// #if UNITY_WEBGL && !UNITY_EDITOR
                SaveToIndexedDB(jsonData);
// #endif
            }
            else
            {
                File.WriteAllText(_boatsDataPath, jsonData);
            }
        }

        public void LoadBoats()
        {
            while (!_isFirstSetupReady)
            {
                if (!_isFirstSetupReady) continue;
                Debug.Log("Waiting for first setup");
                // Wait until setup is ready
                break;
            }

            while (!_isLoadBoatsCallbackCompleted)
            {
                if (!_isLoadBoatsCallbackCompleted) continue;
                Debug.Log("Waiting for LoadBoatsCallback to finish");
                // If other class called LoadBoats(), wait until the result is gained, and return the function
                return;
            }

            Debug.Log("IsStorageCreated: " + _isStorageCreated);
            Debug.Log("IsFirstSetupReady: " + _isFirstSetupReady);
            Debug.LogError("Exists?: " + _isStorageCreated);
            if (_isStorageCreated)
            {
                // TODO lags after first creation somewhere here
                Debug.Log("Calling a function with GetAwaiter()");
                var jsonData = _isWebIOS
                    ? CallExternalLoadFunction().GetAwaiter().GetResult()
                    : File.ReadAllText(_boatsDataPath);

                Debug.LogError(
                    "[L]File content:                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                " +
                    jsonData);

                LoadDataFromJson(jsonData);
            }
            else
            {
                CreateNewSaveFile();
            }

            return;

            Task<string> CallExternalLoadFunction()
            {
                var tcs = new TaskCompletionSource<string>();
                Debug.Log("Starting Coroutine");
                StartCoroutine(CallExternalLoadFunctionCoroutine(tcs));
                Debug.Log("Returning the tcs.Task");
                return tcs.Task;

                IEnumerator CallExternalLoadFunctionCoroutine(TaskCompletionSource<string> taskCompletionSource)
                {
                    _isLoadBoatsCallbackCompleted = false;
                    _loadBoatsCallbackResult = "";
// #if UNITY_WEBGL && !UNITY_EDITOR
                    Debug.Log("Calling Load function from C#");
                    LoadFromIndexedDB(LoadBoats_Callback);
// #endif
                    while (!_isLoadBoatsCallbackCompleted)
                    {
                        Debug.Log("Coroutine waiting for callback...");
                        yield return new WaitForSeconds(0.01f);
                    }

                    Debug.Log("Task completed");
                    taskCompletionSource.SetResult(_loadBoatsCallbackResult);
                }
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

                Debug.LogError("[O]BoatName: " + boats.currentlyChosenBoat.boatName);
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
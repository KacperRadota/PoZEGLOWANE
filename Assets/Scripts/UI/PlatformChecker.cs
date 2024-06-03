// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantUsingDirective

using System.Runtime.InteropServices;
using DataStorage;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

// ReSharper disable InvertIf

namespace UI
{
    public class PlatformChecker : MonoBehaviour
    {
        [SerializeField] private GameObject appDownloadInfoPopUp;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsAndroid();
#endif

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (IsAndroid() && PlayerPrefs.GetInt(PPKeys.WasDownloadInfoShown) == PPValues.False)
            {
                PopUpManager.Instance.OpenPopUp(appDownloadInfoPopUp);
                PlayerPrefs.SetInt(PPKeys.WasDownloadInfoShown, PPValues.True);
                PlayerPrefs.Save();
            }
#endif
        }
    }
}
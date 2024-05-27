using Thirdparty.Toast_UI.Scripts;
using UnityEngine;

namespace UI
{
    public static class ToastMaker
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public static void ShowToast(string message, int length = 1)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    var toastLength = length switch
                    {
                        2 => "LENGTH_LONG",
                        1 => "LENGTH_SHORT",
                        _ => "LENGTH_SHORT"
                    };
                    var toastClass = new AndroidJavaClass("android.widget.Toast");
                    var context =
                        new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>(
                            "currentActivity");
                    var javaString = new AndroidJavaObject("java.lang.String", message);
                    var toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, javaString,
                        toastClass.GetStatic<int>(toastLength));
                    toast.Call("show");
                    break;
                }
                default:
                    var toastDuration = length switch
                    {
                        2 => 3.5f,
                        1 => 2f,
                        _ => 2f
                    };
                    Toast.Show(message, toastDuration);
                    break;
            }
        }
    }
}
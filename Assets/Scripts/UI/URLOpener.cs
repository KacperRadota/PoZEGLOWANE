using UnityEngine;

namespace UI
{
    public class URLOpener : MonoBehaviour
    {
        public static void OpenConstructionEasterEgg()
        {
            const string url = "https://youtu.be/gBJegk71qf4?si=DYfL2WF37_xDHyRi";
            Application.OpenURL(url);
        }

        public static void OpenDownloadLink()
        {
            const string url =
                "https://github.com/KacperRadota/PoZEGLOWANE/raw/main/Builds/Android/Po%C5%BBEGLOWANE.apk";
            Application.OpenURL(url);
        }
    }
}
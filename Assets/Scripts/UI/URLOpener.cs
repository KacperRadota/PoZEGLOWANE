using UnityEngine;

namespace UI
{
    public class URLOpener : MonoBehaviour
    {
        public void OpenConstructionEasterEgg()
        {
            const string url = "https://youtu.be/gBJegk71qf4?si=DYfL2WF37_xDHyRi";
            Application.OpenURL(url);
        }
    }
}
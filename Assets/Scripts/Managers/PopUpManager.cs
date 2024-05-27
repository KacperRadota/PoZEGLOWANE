using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class PopUpManager : MonoBehaviour
    {
        public static PopUpManager Instance;

        [SerializeField] private GameObject background;
        [SerializeField] private float popUpAnimationDuration = 0.1f;

        private static Image _backgroundImage;
        private static float _finalAlphaValue;

        private void Awake()
        {
            _backgroundImage = background.GetComponent<Image>();
            _finalAlphaValue = _backgroundImage.color.a;
            if (Instance) return;
            Instance = this;
        }

        public void OpenPopUp(GameObject popUp)
        {
            var backgroundColor = _backgroundImage.color;
            _backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0);
            var startScale = Vector3.one * 0.01f;
            popUp.transform.localScale = startScale;
            background.SetActive(true);
            popUp.SetActive(true);
            StartCoroutine(ScaleOverTime(popUp, startScale, Vector3.one, 0, _finalAlphaValue));
        }

        public void ClosePopUp(GameObject popUp)
        {
            StartCoroutine(ScaleOverTime(popUp, Vector3.one, Vector3.one * 0.01f, _finalAlphaValue, 0, true));
        }

        private IEnumerator ScaleOverTime(GameObject obj, Vector3 startScale, Vector3 endScale, float startAlpha,
            float endAlpha, bool isClosing = false)
        {
            var elapsedTime = 0f;
            var backgroundColor = _backgroundImage.color;
            while (elapsedTime < popUpAnimationDuration)
            {
                obj.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / popUpAnimationDuration);
                var newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / popUpAnimationDuration);
                _backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            obj.transform.localScale = endScale;
            _backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, endAlpha);

            obj.SetActive(!isClosing);
            background.SetActive(!isClosing);
            if (isClosing)
            {
                _backgroundImage.color =
                    new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, _finalAlphaValue);
            }
        }
    }
}
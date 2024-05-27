using System.Collections;
using UnityEngine;

namespace UI
{
    public class Pulsator : MonoBehaviour
    {
        [SerializeField] private float scaleMultiplier = 1.5f;
        [SerializeField] private float scaleMaxMultiplier = 3.0f;
        [SerializeField] private float scaleUpDuration = 0.1f;
        [SerializeField] private float scaleDownDuration = 0.5f;

        private Vector3 _initialScale;
        private Vector3 _currentScale;
        private Vector3 _currentTargetScale;
        private bool _isCoroutineRunning;
        private bool _isShrinking;

        public void PlayPulsatingEffect()
        {
            if (!_isCoroutineRunning)
            {
                _currentScale = _initialScale;
                _currentTargetScale = _initialScale * scaleMultiplier;
                StartCoroutine(Pulsate());
            }
            else
            {
                _currentScale = transform.localScale;
                if (!_isShrinking)
                {
                    _currentTargetScale =
                        (_initialScale * scaleMaxMultiplier - _currentTargetScale) / 3 + _currentTargetScale;
                }
                else
                {
                    _currentTargetScale *= 1.5f;
                }
            }

            Debug.Log(_currentTargetScale);
        }

        private void Start()
        {
            _initialScale = transform.localScale;
        }

        private IEnumerator Pulsate()
        {
            _isCoroutineRunning = true;
            _isShrinking = false;
            var elapsedTime = 0f;
            while (elapsedTime < scaleUpDuration)
            {
                transform.localScale = Vector3.Lerp(_currentScale, _currentTargetScale, elapsedTime / scaleUpDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = _currentTargetScale;
            elapsedTime = 0f;
            _isShrinking = true;
            while (elapsedTime < scaleDownDuration)
            {
                transform.localScale =
                    Vector3.Lerp(_currentTargetScale, _initialScale, elapsedTime / scaleDownDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = _initialScale;
            _isCoroutineRunning = false;
            _isShrinking = false;
        }
    }
}
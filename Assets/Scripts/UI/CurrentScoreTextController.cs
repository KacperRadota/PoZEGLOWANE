using DataStorage;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CurrentScoreTextController : MonoBehaviour
    {
        private void Start()
        {
            DataController.Instance.LoadBoats();
            GetComponent<TextMeshProUGUI>().text =
                DataController.Instance.boats.currentlyChosenBoat.lastCalculatedScore.ToString();
        }
    }
}
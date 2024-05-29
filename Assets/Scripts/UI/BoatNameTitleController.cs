using DataStorage;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BoatNameTitleController : MonoBehaviour
    {
        private static TextMeshProUGUI _title;

        private void Start()
        {
            _title = GetComponent<TextMeshProUGUI>();
            SetBoatName();
        }

        public static void SetBoatName()
        {
            DataController.Instance.LoadBoats();
            _title.text = DataController.Instance.boats.currentlyChosenBoat.boatName;
        }
    }
}
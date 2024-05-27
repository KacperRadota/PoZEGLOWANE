using DataStorage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI boatNameTitle;
        [SerializeField] private GameObject noFirstBoatPopUp;

        public void OKClicked(InputField inputField)
        {
            var boatName = inputField.text.Trim();
            inputField.text = "";
            DataController.Instance.LoadBoats();
            if (boatName == "")
            {
                if (DataController.Instance.boats.currentlyChosenBoat.boatName != "")
                {
                    return;
                }

                boatName = "Moja Łódka";
            }

            DataController.Instance.boats.currentlyChosenBoat.boatName = boatName;
            DataController.Instance.SaveBoats();
            SetBoatNameTitle();
        }

        private void Start()
        {
            DataController.Instance.LoadBoats();
            SetBoatNameTitle();
            if (DataController.Instance.boats.currentlyChosenBoat.boatName != "") return;
            PopUpManager.Instance.OpenPopUp(noFirstBoatPopUp);
        }

        private void SetBoatNameTitle()
        {
            boatNameTitle.text = DataController.Instance.boats.currentlyChosenBoat.boatName;
        }
    }
}
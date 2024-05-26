using DataStorage;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI boatNameTitle;
        [SerializeField] private GameObject popUpBackground;
        [SerializeField] private GameObject noFirstBoatPopUp;

        public void OKClicked(TMP_InputField inputField)
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
                else
                {
                    boatName = "Moja Łódka";
                }
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
            popUpBackground.SetActive(true);
            noFirstBoatPopUp.SetActive(true);
        }

        private void SetBoatNameTitle()
        {
            boatNameTitle.text = DataController.Instance.boats.currentlyChosenBoat.boatName;
        }
    }
}
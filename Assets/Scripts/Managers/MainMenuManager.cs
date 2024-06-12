using DataStorage;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject noFirstBoatPopUp;

        public void OKClicked(InputField inputField)
        {
            var boatName = inputField.text.Trim();
            inputField.text = "";
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
            BoatNameTitleController.SetBoatName();
        }

        private async void Start()
        {
            await DataController.Instance.LoadBoatsAsync();
            if (DataController.Instance.boats.currentlyChosenBoat.boatName != "") return;
            PopUpManager.Instance.OpenPopUp(noFirstBoatPopUp);
        }
    }
}
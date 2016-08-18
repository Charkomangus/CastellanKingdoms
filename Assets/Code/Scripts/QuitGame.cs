using UnityEngine;

namespace Assets.Code.Scripts
{
    public class QuitGame : MonoBehaviour {

        // Use this for initialization
        public void Quit () {
            Debug.Log ("You have quit the game! Traitor..");
            Application.Quit();
        }
	

    }
}

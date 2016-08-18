using UnityEngine;

namespace Assets.Code.Scripts
{
    public class MusicManager : MonoBehaviour {
	
	
        // Game Data Reference
        private GameObject m_gameManager;
        private GameData m_gameDataRef;

        // State Manager Reference
        private StateManager m_stateManagerRef;
	
        //reference to the Sound
        public GameObject castleandCourt;
        public GameObject conquest;
        public GameObject castleAmbience;
        public GameObject menu;
        public GameObject openingMusic;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {

        }
        // Play
        public void PlayCastleAndCourtMusic () {
            castleandCourt.SetActive (true);
        }

        // Pause
        public void PauseCastleAndCourtMusic () {
            castleandCourt.SetActive (false);
        }

        // Play
        public void PlayCastleAmbience () {
            castleAmbience.SetActive (true);
        }


        // Pause
        public void PauseCastleAmbience () {
            castleAmbience.SetActive (false);
        }

        // Play
        public void PlayConquestMusic () {
            conquest.SetActive (true);
        }
	
        // Pause
        public void PauseConquestMusic () {
            conquest.SetActive (false);
        }

        // Play
        public void PlayMenuMusic () {
            menu.SetActive (true);
        }
	
        // Pause
        public void PauseMenuMusic () {
            menu.SetActive (false);
        }

        // Play
        public void PlayopeningMusic () {
            openingMusic.SetActive (true);
        }
	
        // Pause
        public void PauseopeningMusic () {
            openingMusic.SetActive (false);
        }
    }
}

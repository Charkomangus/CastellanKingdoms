using UnityEngine;

namespace Assets.Code.Scripts
{
    public class OpeningManager : MonoBehaviour {

        // Game Manager Ref
        private GameObject m_gameManager;
        private StateManager m_stateManagerRef;

        // UI 
        public GameObject startButton;

        private int m_buttonTime = 180;
        private int m_buttonTimer = 0;
        private bool m_buttonTiming = true;

        // Use this for pre-initialization
        void Awake () {
            // Assign game manager ref 
            m_gameManager = GameObject.Find ("GameManager");
            // Assign state manager ref 
            m_stateManagerRef = m_gameManager.GetComponent<StateManager> ();
        }

        // Use this for initialization
        void Start () {

        }
	
        // Update is called once per frame
        void Update () {
            if (m_buttonTiming) {
                m_buttonTimer++;
            }
            if ((m_buttonTiming) && (m_buttonTimer >= m_buttonTime)) {
                m_buttonTiming = false;
                m_buttonTime = 0;
                startButton.SetActive(true);
            }
        }

        // Called by startbutton onclick event
        public void StartButtonPressed()
        {
            // Switch to the castle state
            m_stateManagerRef.SwitchToCastle ();
            // start the first turn
            m_stateManagerRef.StartFirstTurn ();
        }


    }
}

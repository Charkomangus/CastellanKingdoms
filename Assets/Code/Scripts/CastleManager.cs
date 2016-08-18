using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class CastleManager : MonoBehaviour {


        // Game Data Reference
        private GameObject m_gameManager;

        //reference to the UI
        //private	 UIManager m_uiManagerRef; 

        // State Manager Reference
        private StateManager m_stateManagerRef;
		
        // UI element references
        public Button[] buildingButtons;
        //Flag References
        public GameObject[] flagKeep;
        public GameObject[] barrackKeep;

        // Called on startup
        void Awake ()
        {

            // Set up reference to Game Data via Game Manager
            m_gameManager = GameObject.Find ("GameManager");

            // Set up state manager ref
            m_stateManagerRef = m_gameManager.GetComponent<StateManager> ();

        }
	
        // Use this for initialization
        void Start () {
            // If the player is human
            if (m_stateManagerRef.PhasingPlayer.IsHuman)
            {
                // Make all the buttons interactable
                for (int i = 0; i < buildingButtons.Length; i++)
                {
                    buildingButtons[i].interactable = true;
                }
                for (int i = 0; i < flagKeep.Length; i++){
                    flagKeep[i].SetActive(false);
                    barrackKeep[i].SetActive(false);
                }
                flagKeep[m_stateManagerRef.PhasingPlayer.MyFaction.FactionIndex].SetActive(true);
                barrackKeep[m_stateManagerRef.PhasingPlayer.MyFaction.FactionIndex].SetActive(true);
                //Update UI
                m_stateManagerRef.uiManagerRef.UpdateUI();
            }
            // If this is the first turn of the game and there is no save file:
            if ((m_stateManagerRef.GameTurn == 1) && (m_stateManagerRef.PhasingPlayerIndex == 0)) {
                if (!File.Exists (Application.persistentDataPath + "/CKSaveData.dat")) 
                    m_stateManagerRef.uiManagerRef.FirstGameText.SetActive(true);
            }
        }
	
        // Update is called once per frame

        void Update () {
	
        }

        public void SwitchToCourt()
        {
            m_stateManagerRef.SwitchToCourt ();
        }
    }
}

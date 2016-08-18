using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class LossManager : MonoBehaviour {
	
	
        // Game Data Reference
        private GameObject m_gameManager;
	
        // State Manager Reference
        private StateManager m_stateManagerRef;
	

        //Game Data Reference
        private GameData m_gameDataRef;

        //Text References
        public Text lossText;

        //Scorring Array & variables
        private int[] scoringArray;
        public int tiedScore;
	
        //End Text References

        private int scoreCount;
        public Player gameWinner;
        // Called on startup
        void Awake ()
        {
		
            // Set up reference to Game Data via Game Manager
            m_gameManager = GameObject.Find ("GameManager");

            // Set up state manager ref
            m_stateManagerRef = m_gameManager.GetComponent<StateManager> ();

            // Get the GameData script.
            m_gameDataRef = m_gameManager.GetComponent<GameData> ();


        }
	
        // Use this for initialization
        void Start () {


            scoringArray = new int[4];
            //		If the game was won by reaching the Time Limit make the player with the most influence the Winner
            if (m_gameDataRef.GameVictoryState == GameData.VictoryState.GameEnd) {
                for (int i = 0; i < m_gameDataRef.activePlayers.Count; i++) {
                    m_gameDataRef.activePlayers [i].PlayerScore ();
                }
			
                for (int j = 0; j < m_gameDataRef.players.Length; j ++) {
                    scoringArray [j] = m_gameDataRef.players [j].MyPlayerScore; 
                }
                for (int a = 0; a < scoringArray.Length; a ++) {
                    if (scoringArray [a] == scoringArray.Max ())
                        tiedScore += 1;
                }
			
			
                if (tiedScore > 1) {
                    lossText.text = "Your Kingdoms were great in power but there was not enough room for more than one. A massive war begun between your powerful Empires which in " +
                                    "time destroyed much of the known world.";
				
				
				
				
                } else {
				
				
                    lossText.text = "Your Kingdoms have been completely crushed leaving a void of power. Chaos erupts and bitter fighting continues" +
                                    "between the Great Empirs, new and old. None will prevail and the struggle will continue for hundreds of years to come.";
                }
				
            }
        }

				

	

	
	
        public void TryAgain(){
            m_stateManagerRef.Restart ();
            m_stateManagerRef.uiManagerRef.RestartUI ();
        }
    }
}


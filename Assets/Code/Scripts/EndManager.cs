using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class EndManager : MonoBehaviour
    {
	
	
        // Game Data Reference
        private GameObject m_gameManager;
	
        // State Manager Reference
        private StateManager m_stateManagerRef;

        //Game Data Reference
        private GameData m_gameDataRef;

        //Scorring Array & variables
        private int[] scoringArray;
        public int tiedScore;

        //End Text References
        public Text influence;
        public Text popularity;
        public Text piety;
        public Text score;
        public Text playerName;
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
        void Start ()
        {
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
                    m_stateManagerRef.SwitchToLoss ();




                } else {


                    for (int k = 0; k < scoringArray.Length; k ++)
                        if (m_gameDataRef.players [k].MyPlayerScore == scoringArray.Max ()) {
                            gameWinner = m_gameDataRef.players [k];
                            if (gameWinner.IsHuman == false)
                                m_stateManagerRef.SwitchToLoss ();
                        }

                }
            }





            //If the game was won by Domination make the last surviving player the Winner
            Debug.Log ("Current State is " + m_gameDataRef.GameVictoryState);
            if (m_gameDataRef.GameVictoryState == GameData.VictoryState.Domination) {
                gameWinner = m_stateManagerRef.PhasingPlayer;
                Debug.Log ("Winner is " + gameWinner + " with " + gameWinner.Influence + " Influence.");
            }
	









            scoreCount = 0;
            //Set the Influence Text
            if (gameWinner.Influence >= 40) {
                scoreCount += 2;
                influence.text = "You have forged an Empire that will last and flourish for thousands of years. " +
                                 "Millenia will pass but almost everyone will remember you.";
            } else if (gameWinner.Influence < 40 && gameWinner.Influence >= 20) {
                scoreCount += 1;
                influence.text = "Your Kingdom became one of the Great powers to rule the Land. " +
                                 "Your name might not be remembered but the things you did have marked all of History.";
            } else {
                scoreCount += 0;
                influence.text = "You have emerged victorious but at a price." +
                                 "Your Kingdom survived but did not flourish and your legasy vanished in mere decades.";
            }
            //Set the Popularity Text
            if (gameWinner.Popularity >= 20) {
                scoreCount += 2;
                popularity.text = "Your compassion though was what was truly legendary. The way you treated your subjects inspired " +
                                  "massive social changes, all to the good of humanity.";
            } else if (gameWinner.Popularity < 20 && gameWinner.Popularity >= 10) {
                scoreCount += 1;
                popularity.text = "You did not forget about your people. You took care of them and in turn they returned the favour." +
                                  " You die, surrounded not only by loyal subjects but friends.";
            } else {
                scoreCount += 0;
                popularity.text = "Your people suffered under your rule. They knew that they were beneath your notice and while they" +
                                  " respected your authority, it was out of fear and not love.";
            }

            if (gameWinner.Piety >= 20) {
                scoreCount += 2;
                piety.text = "Regardless, it was obvious that you were blessed by God himself. You were quickly elected as Pope " +
                             "where you spread the Christian Faith all across the Globe";
            } else if (gameWinner.Piety < 20 && gameWinner.Piety >= 10) {
                scoreCount += 1;
                piety.text = "Regardless, your actions greatly impressed the Pope and you enjoyed Papal support throughout your reign." +
                             " You are remembered as a Saint.";
            } else {
                scoreCount += 0;
                piety.text = "Regardless, after rebuffing any attempt to please the Pope you were branded as an unbeliever and were excommunicated. " +
                             "Even now, you are remembered as a Heretic.";
            }
            if (scoreCount == 6)
                score.text = "A+";
            if (scoreCount == 5)
                score.text = "A";
            if (scoreCount == 4)
                score.text = "B+";
            if (scoreCount == 3)
                score.text = "B";
            if (scoreCount == 2)
                score.text = "C";
            if (scoreCount == 1)
                score.text = "D";
            if (scoreCount == 0)
                score.text = "F";

            playerName.text = gameWinner.name;
        }
	
				                           
			







	
        // Update is called once per frame
	
        void Update ()
        {
		
        }

        public void TryAgain ()
        {
            m_stateManagerRef.Restart ();
            m_stateManagerRef.uiManagerRef.RestartUI ();
        }
    }
}

	
	

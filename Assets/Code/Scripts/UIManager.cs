using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class UIManager : MonoBehaviour
    {
		
        // VARIABLES
        public int year;
        public string day;
        public string[] months;

        //Tutorial Reference
        public GameObject tutorial;

        private int monthNumber;
        public int MonthNumber
        { get { return monthNumber; }}
	
        public Image playerBoardOpen;

        // Image references		
        public Image currentFactionImage;
        public Image[] currentFactionImages;
	
        // GameData		
        private GameObject m_gameManager;
        private GameData m_gameDataRef;
        private StateManager m_stateManagerRef;
        private Player m_previousPlayer;
		
        // Controller References
        private GameObject m_controllerRef;
		
        //UI Elements
        public GameObject uiBar;
        public Text[] uiNumbers;
        public Text[] influenceNumbers;
        public Text[] playerTurnCountries;
        public GameObject[] playerTurn;
        public Text player;
        public GameObject aiPlayingMask;
        public GameObject aiPlayingPanel;
        public Text aiPlayingText;
        public GameObject FirstGameText;

        // CONSTRUCTOR
		
        public void Awake ()
        {
            DontDestroyOnLoad (transform.gameObject);
            m_gameManager = GameObject.Find ("GameManager");
            m_gameDataRef = m_gameManager.GetComponent<GameData> ();
            m_stateManagerRef = m_gameManager.GetComponent<StateManager> ();	
            year = 1300;
        }
		
        /// <summary>
        /// Update the UI.
        /// </summary>
        public void UpdateUI ()
        {
            Debug.Log ("Updating UI");
				
            //Gold Counter & Gold Per turn
            uiNumbers [0].text = m_stateManagerRef.PhasingPlayer.Gold.ToString () + "(+" + m_stateManagerRef.PhasingPlayer.GoldPerTurn + ")";
            Debug.Log ("Player has " + m_stateManagerRef.PhasingPlayer.Gold + " Gold");

            //Influence Counter
            uiNumbers [1].text = m_stateManagerRef.PhasingPlayer.Influence.ToString ();
            Debug.Log ("Player has " + m_stateManagerRef.PhasingPlayer.Influence + " Influence");

            //Piety Counter
            uiNumbers [2].text = m_stateManagerRef.PhasingPlayer.Piety.ToString ();
            Debug.Log ("Player has " + m_stateManagerRef.PhasingPlayer.Piety + " Piety");

            //Popularity Counter
            uiNumbers [3].text = m_stateManagerRef.PhasingPlayer.Popularity.ToString ();
            Debug.Log ("Player has " + m_stateManagerRef.PhasingPlayer.Popularity + " Popularity");

            //The Day
            uiNumbers [4].text = day;			
            Debug.Log ("Today is the " + day);

            //TheMonth
            uiNumbers [5].text = months [monthNumber] + (",");
            Debug.Log ("It is the month " + months [monthNumber]);

            //The Year Of Our Lord
            uiNumbers [6].text = year.ToString ();
            Debug.Log ("It is the year " + year + " of our Lord");
								
            //ActivePlayer
            uiNumbers [7].text = "(" + m_stateManagerRef.PhasingPlayer.name + ")";
            Debug.Log ("It is " + m_stateManagerRef.PhasingPlayer.name + "'s turn.");

            //ActivePlayer's Faction
            uiNumbers [8].text = m_stateManagerRef.PhasingPlayer.MyFaction.name;
            Debug.Log (m_stateManagerRef.PhasingPlayer.MyFaction.name);				

            //Set Phasing Players Flaf
            currentFactionImage.sprite = m_stateManagerRef.PhasingPlayer.MyFaction.MyFlagSprite;
				
            //Set Player on Closed Panel
            player.text = m_stateManagerRef.PhasingPlayer.name;


            //Set number of players on UI

            for (int i = 0; i < m_gameDataRef.activePlayers.Count; i++) {				 
                playerTurn [i].SetActive (true);					
                playerTurnCountries [i].text = m_gameDataRef.activePlayers [i].MyFaction.name;
                influenceNumbers [i].text = m_gameDataRef.activePlayers [i].Influence.ToString ();
                currentFactionImages[i].sprite = m_gameDataRef.activePlayers [i].MyFaction.MyFlagSprite;
            }
            if (m_gameDataRef.activePlayers.Count == 3) {
                playerBoardOpen.rectTransform.sizeDelta = new Vector2 (307, 220);
                playerTurn [3].SetActive (false);	
            }
            if (m_gameDataRef.activePlayers.Count == 2) {
                playerTurn [2].SetActive (false);	
                playerTurn [3].SetActive (false);	
                playerBoardOpen.rectTransform.sizeDelta = new Vector2 (307, 150);
            }
		
        }

        public void WhatDateIsIt ()
        {
            //Sets a random number to be the day with it's proper ending
            int dayNumber = Random.Range (1, 28);
            if (dayNumber == 1)
                day = dayNumber.ToString () + "st";
            if (dayNumber == 2)
                day = dayNumber.ToString () + "nd";
            if (dayNumber == 3)
                day = dayNumber.ToString () + "rd";
            if (dayNumber >= 4 && dayNumber <= 20)
                day = dayNumber.ToString () + "th";
            if (dayNumber == 21)
                day = dayNumber.ToString () + "st";
            if (dayNumber == 22)
                day = dayNumber.ToString () + "nd";
            if (dayNumber == 23)
                day = dayNumber.ToString () + "rd";
            if (dayNumber >= 24 && dayNumber <= 28)
                day = dayNumber.ToString () + "th";
				
            monthNumber = Random.Range (1, 12);
            Debug.Log ("The month number is " + monthNumber);

        }

        public void SetLoadedDate(string loadedDay, int LoadedMonthNumber)
        {
            day = loadedDay;
            monthNumber = LoadedMonthNumber;
        }
		
        public void WhatYearIsthis ()
        {
            year = (m_stateManagerRef.GameTurn * 10) + ((10/m_gameDataRef.activePlayers.Count) * m_stateManagerRef.PhasingPlayerIndex) + 1290;				
            Debug.Log ("The Year is " + year);
        }

        //Restart the UI
        public void RestartUI ()
        {
            WhatDateIsIt ();
            year = 1300;
        }

        // <summary>
        // Activates The tutorial in the apropriate scenes
        // </summary>
        public void ActivateTutorial ()
        {
            tutorial.SetActive (true);
        }

		


    }
}





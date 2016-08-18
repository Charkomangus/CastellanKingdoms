using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class StartMenuManager : MonoBehaviour
    {

        //VARIABLES
        // Game Data Reference
        private GameData m_gameDataRef;
        // Menu references
        public GameObject startMenu;
        public GameObject selectPlayerMenu;
        public Button continueButton;
        public GameObject chooseFactionButton;
        public Toggle[] activeToggles;
        public Toggle[] humanToggles;
        public Toggle[] aiToggles;
        public Toggle[] aiDifficultyToggles;
        public Toggle[] gameLengthToggles;
        public Toggle battleAnimToggle;
        public Button clearSaveGameButton;

        // Text refereneces
        public Text[] chosenFactionText;
        public Text currentFactionText;
        public Text factionDescription;
        public Text tutorialDescription;
        public Text tutorialTitle;
        public Image tutorial;
        public GameObject credits;

        // Image references
        public Image[] chosenFactionImage;
        public Image currentFactionImage;
		
        // Play info
        private bool m_readyToStart;

        public bool readyTostart
        { get { return m_readyToStart; } }

        private Player m_selectedPlayer;
        private Faction m_selectedFaction;

        // Called on startup
        void Awake ()
        {
            // Set reference to Game Data
            m_gameDataRef = GetComponent<GameData> ();
        }

        // Use this for initialization
        void Start ()
        {
            // Player does not begin the game ready to start
            m_readyToStart = false;

            // Set Faction selection for players 1 & 2
            m_selectedPlayer = m_gameDataRef.players [0];
            m_selectedFaction = m_gameDataRef.factions [0];
            m_selectedPlayer.SetFaction (m_selectedFaction, 0);
            SetPlayerFaction ();
            m_selectedPlayer = m_gameDataRef.players [1];
            m_selectedFaction = m_gameDataRef.factions [1];
            m_selectedPlayer.SetFaction (m_selectedFaction, 1);
            SetPlayerFaction ();
        }
	
        // Update is called once per frame
        void Update ()
        {
            // Show selected player in debug
            //if (m_selectedPlayer != null)
            //Debug.Log ("Selected Player: " + m_selectedPlayer.name);
        }
		
        // Activates the start menu
        public void ActivateStartMenu ()
        {
            // Activate start menu
            startMenu.SetActive (true);
            Debug.Log ("Activated Start Menu");
        }

        public void CheckForSavedGame ()
        {
            if (File.Exists (Application.persistentDataPath + "/CKSaveData.dat")) 
                continueButton.interactable = true;
            else
                continueButton.interactable = false;
        }

        public void BattleAnimToggleChanged ()
        {
            if (battleAnimToggle.isOn) {
                m_gameDataRef.SetBattleAnimationsEnabled (true);
            } else {
                m_gameDataRef.SetBattleAnimationsEnabled (false);
            }
        }

        public void DisableContinueButton ()
        {
            continueButton.interactable = false;
        }

	
        // Changes ReadyToStart to the received parameter
        public void ReadyToStart (bool ready)
        {
            // Change ReadyToStart tp the received parameter
            m_readyToStart = ready;
            Debug.Log ("Ready to Start? " + m_readyToStart);
        }
		
        // Deactivates the selection menu
        public void DeactivateSelectionMenu ()
        {
            // Deactivate selection menu
            selectPlayerMenu.SetActive (false);
            Debug.Log ("Deactivated player select menu");
        }

        // Resets the player seleciton menu
        public void ResetPlayerSelectionMenu ()
        {
            // switch on all toggles
            for (int i = 0; i < activeToggles.Length; i++) {
                activeToggles [i].isOn = true;
            }
            // set P2, 3 & 4 to default to AI
            humanToggles [1].isOn = false;
            humanToggles [2].isOn = false;
            humanToggles [3].isOn = false;
            aiToggles [1].isOn = true;
            aiToggles [2].isOn = true;
            aiToggles [3].isOn = true;

            // switch off toggles 3 & 4
            activeToggles [2].isOn = false;
            activeToggles [3].isOn = false;

            // Assign player 1 to England as default
            m_gameDataRef.players [0].SetFaction (m_gameDataRef.factions [0], 0);
            chosenFactionText [0].text = m_gameDataRef.players [0].MyFaction.name;
            chosenFactionImage [0].sprite = m_gameDataRef.players [0].MyFaction.MyFlagSprite;
            // Assign player 2 to France as default
            m_gameDataRef.players [1].SetFaction (m_gameDataRef.factions [1], 1);
            chosenFactionText [1].text = m_gameDataRef.players [1].MyFaction.name;
            chosenFactionImage [1].sprite = m_gameDataRef.players [1].MyFaction.MyFlagSprite;

            // Set player 3 & 4 to have no faction, and to be inactive

            m_gameDataRef.players [2].SetFaction (null, 0);
            m_gameDataRef.players [3].SetFaction (null, 0);

            m_gameDataRef.players [2].SetActiveTo (false);
            m_gameDataRef.players [3].SetActiveTo (false);

        }
	
        // Sets the selected player for the purpose of choosing faction
        public void SelectPlayerAndFaction (string ButtonPressed)
        {
            Debug.Log ("Called Select Player For Faction.");
            switch (ButtonPressed) {
                // If the Pressed Button was for Player 1
                case "P1ChooseFactionButton":
                {
                    // Set selected player to player 1
                    m_selectedPlayer = m_gameDataRef.players [0];
                    break;
                }
                // If the Pressed Button was for Player 2
                case "P2ChooseFactionButton":
                {
                    // Set selected player to player 2
                    m_selectedPlayer = m_gameDataRef.players [1];
                    break;
                }
                // If the Pressed Button was for Player 3
                case "P3ChooseFactionButton":
                {
                    // Set selected player to player 3
                    m_selectedPlayer = m_gameDataRef.players [2];
                    break;
                }
                // If the Pressed Button was for Player 4
                case "P4ChooseFactionButton":
                {
                    // Set selected player to player 4
                    m_selectedPlayer = m_gameDataRef.players [3];
                    break;
                }

            }

            // Set selected faction to selected player's faction
            m_selectedFaction = m_selectedPlayer.MyFaction;
            // Set the current faction (in choose faction menu) text and image to the selected faction's
            currentFactionText.text = m_selectedFaction.name;
            currentFactionImage.sprite = m_selectedFaction.MyFlagSprite;
            factionDescription.text = m_selectedFaction.Description;
            Debug.Log ("Selected player is " + m_selectedPlayer.name + ", faction is " + m_selectedFaction.name);

        }


        // Change the Selected Faction in the Choose Faction Menu
        public void ChangeSelectedFaction (string ButtonPressed)
        {
            // If "Next Faction" was pressed
            if (ButtonPressed == "ButtonFactionNext") {
                Debug.Log ("Next faction button pressed.");
                // If the selected player's faction index is less than the length of the faction array
                if (m_selectedFaction.FactionIndex < (m_gameDataRef.factions.Length - 1)) {
                    // Set Local Variable i
                    int i = m_selectedFaction.FactionIndex + 1;
                    //Set the new description
                    m_selectedFaction.setDescription (m_gameDataRef.factions [i - 1].Description);
                    // Set the new selected faction
                    m_selectedFaction = m_gameDataRef.factions [i];
                }
                else 
                { 
                    // Set the selected faction etc 
                    //m_selectedFaction.setDescription(m_gameDataRef.factions[0].Description);
                    m_selectedFaction = m_gameDataRef.factions[0];
                }
            } else {
                Debug.Log ("Previous faction button pressed.");
                // If the selected player's faction index is more than 0
                if (m_selectedFaction.FactionIndex > 0) {
                    // Set Local Variable i
                    int i = m_selectedFaction.FactionIndex - 1;
                    //Set the new description
                    //m_selectedFaction.setDescription (m_gameDataRef.factions [i + 1].Description);
                    // Set the new selected faction
                    m_selectedFaction = m_gameDataRef.factions [i];
                }
                else {
                    m_selectedFaction = m_gameDataRef.factions[m_gameDataRef.factions.Length - 1];
                }
            }
            // Show the name of the current selected faction
            currentFactionText.text = m_selectedFaction.name;
            // Show the image of the current selected faction
            currentFactionImage.sprite = m_selectedFaction.MyFlagSprite;
            factionDescription.text = m_selectedFaction.Description;

        }

        // This is used to check whether the selected faction is in play under control of another player,
        // If another player is using it the 'choose faction' button is disabled.
        public void CheckFactionInPlay ()
        {
            // If selected faction is in play
            if (m_selectedFaction.InPlay == true) {
                // If selected faction's player index is different from selected player's index
                if (m_selectedFaction.ControllingPlayerIndex != m_selectedPlayer.MyPlayerIndex)
                    // Disable the choose Faction button
                    chooseFactionButton.SetActive (false);
                else
                // Otherwise enable the choose Faction button
                    chooseFactionButton.SetActive (true);
            }
            // Otherwise selected faction is in play, enable the choose faction button
            else
                chooseFactionButton.SetActive (true);
        }

        // Set the Selected Player's Faction to the Selected Faction
        public void SetPlayerFaction ()
        {
            m_selectedPlayer.SetFaction (m_selectedFaction, m_selectedFaction.FactionIndex);
            chosenFactionText [m_selectedPlayer.MyPlayerIndex].text = m_selectedFaction.name;
            chosenFactionImage [m_selectedPlayer.MyPlayerIndex].sprite = m_selectedFaction.MyFlagSprite;
        }

        // Set the player's faction to the next available faction
        // This is called when a new player is added or removed
        public void SetNextAvailableFaction (int index)
        {
            // If the player is active
            if (m_gameDataRef.players [index].IsActive == true) {
                // For all factions
                for (int i = 0; i < m_gameDataRef.factions.Length - 1; i++) {
                    // If the faction's in play value is false
                    if (m_gameDataRef.factions [i].InPlay == false) {
                        // Assign the playert this faction then break from loop
                        m_gameDataRef.players [index].SetFaction (m_gameDataRef.factions [i], i);
                        chosenFactionText [index].text = m_gameDataRef.players [index].MyFaction.name;
                        chosenFactionImage [index].sprite = m_gameDataRef.players [index].MyFaction.MyFlagSprite;
                        break;
                    }
                }

            } 
        }
        //move towards a target at a set speed.
        public void MoveTowardsTarget ()
        {
            //the speed, in units per second, we want to move towards the target
            float speed = 1;
            //move towards the center of the world (or where ever you like)
            Vector3 targetPosition = new Vector3 (0, 0, 0);
		
            Vector3 currentPosition = this.transform.position;
            //first, check to see if we're close enough to the target
            if (Vector3.Distance (currentPosition, targetPosition) > .1f) { 
                Vector3 directionOfTravel = targetPosition - currentPosition;
                //now normalize the direction, since we only want the direction information
                directionOfTravel.Normalize ();
                //scale the movement on each axis by the directionOfTravel vector components
			
                this.transform.Translate (
                    (directionOfTravel.x * speed * Time.deltaTime),
                    (directionOfTravel.y * speed * Time.deltaTime),
                    (directionOfTravel.z * speed * Time.deltaTime),
                    Space.World);
            }
        }

        /// <summary>
        /// Called when one of the AI difficulty toggles is clicked.
        /// This method cycles through the toggles to find which is 'ticked'
        /// It then sets the AI Difficulty via game data to the corresponding difficulty.
        /// It is also called when entering the difficutly menu to ensure that the current difficulty is 
        /// </summary>
        public void AIDifficultyToggleClicked ()
        {
            for (int i = 0; i < aiDifficultyToggles.Length; i++) {
                if (aiDifficultyToggles [i].isOn) {
                    switch (i) {
                        case 0:
                        {
                            if (m_gameDataRef.GameAIDifficulty != GameData.AIDifficulty.Easy)
                                m_gameDataRef.SetAIDifficulty (GameData.AIDifficulty.Easy);
                            break;
                        }
                        case 1:
                        {
                            if (m_gameDataRef.GameAIDifficulty != GameData.AIDifficulty.Standard)

                                m_gameDataRef.SetAIDifficulty (GameData.AIDifficulty.Standard);
                            break;
                        }
                        case 2:
                        {
                            if (m_gameDataRef.GameAIDifficulty != GameData.AIDifficulty.Difficult)

                                m_gameDataRef.SetAIDifficulty (GameData.AIDifficulty.Difficult);
                            break;
                        }
                    }
                    break;
                }
            }

        }

        public void SetTogglesToAIDifficulty ()
        {
            switch (m_gameDataRef.GameAIDifficulty) {
                case GameData.AIDifficulty.Easy:
                {
                    aiDifficultyToggles [0].isOn = true;
                    break;
                }
                case GameData.AIDifficulty.Standard:
                {
                    aiDifficultyToggles [1].isOn = true;

                    break;
                }
                case GameData.AIDifficulty.Difficult:
                {
                    aiDifficultyToggles [2].isOn = true;

                    break;
                }
            }
        }

        public void GameLengthToggleChanged ()
        {
            for (int i = 0; i < gameLengthToggles.Length; i++) {
                if (gameLengthToggles [i].isOn) {
                    switch (i) {
                        case 0:
                        {
                            if (m_gameDataRef.LastGameTurn != m_gameDataRef.ShortGameLength)
                                m_gameDataRef.SetLastGameTurn (m_gameDataRef.ShortGameLength);
                            break;
                        }
                        case 1:
                        {
                            if (m_gameDataRef.LastGameTurn != m_gameDataRef.MediumGameLength)
                                m_gameDataRef.SetLastGameTurn (m_gameDataRef.MediumGameLength);
                            break;
                        }
                        case 2:
                        {
                            if (m_gameDataRef.LastGameTurn != m_gameDataRef.LongGameLength)
                                m_gameDataRef.SetLastGameTurn (m_gameDataRef.LongGameLength);
                            break;
                        }
                    }
                    break;
                }
            }
		
        }

        public void SetTogglesToGameLength ()
        {
            switch (m_gameDataRef.LastGameTurn) {
                case 10:
                {
                    gameLengthToggles [0].isOn = true;
                    break;
                }
                case 20:
                {
                    gameLengthToggles [1].isOn = true;

                    break;
                }
                case 30:
                {
                    gameLengthToggles [2].isOn = true;
                    break;
                }
            }
        }

        //Start The Credits Rolling
        public void StartCredits ()
        {
            credits.SetActive (true);

        }


    }
}

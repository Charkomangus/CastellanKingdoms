using Assets.Code.Interfaces;
using Assets.Code.States;
using UnityEngine;
namespace Assets.Code.Scripts
{
    public class StateManager : MonoBehaviour
    {
        // VARIABLES
        // Active state
        public IStateBase activeState;
        // Reference to the instance of state manager
        public static StateManager instanceRef;
        // Reference to the Game Data script
        public GameData gameDataRef;
        // Reference to the Start Menu Manager Script
        public StartMenuManager startMenuManagerRef;


        //reference to the UI & UI variables
        public UIManager uiManagerRef;
        public GameObject ui;

        //PauseMenu
        public GameObject pauseMenu;

        // Turn Counter Variables
        private int m_gameTurn;

        public int GameTurn
        { get { return m_gameTurn; } }


        private int m_phasingPlayerIndex;

        public int PhasingPlayerIndex
        { get { return m_phasingPlayerIndex; } }

        private Player m_phasingPlayer;

        public Player PhasingPlayer
        { get { return m_phasingPlayer; } }

        private bool m_loadingGame;

        public bool LoadingGame
        { get { return m_loadingGame; } }

        /// <summary>
        /// Awake this instance.
        /// </summary>
        void Awake ()
        {

            // If no instanceRef has been set, set it to this instance and don't destroy game object.
            // Otherwise destroy gameObject.
            if (instanceRef == null) {
                instanceRef = this;
                DontDestroyOnLoad (gameObject);
            } else {
                DestroyImmediate (gameObject);
            }

            // Get the GameData script.
            gameDataRef = GetComponent<GameData> ();
            startMenuManagerRef = GetComponent<StartMenuManager> ();

        }

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start ()
        {
            // Set Active State to IntroState.
            activeState = new Start_MenuState (this);
            uiManagerRef.WhatDateIsIt ();
        }
	
        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update ()
        {
            // If an active state has been assigned, call it's Update.
            if (activeState != null) {
                activeState.StateUpdate ();
            }
            if ((PhasingPlayer != null) && (PhasingPlayer.IsHuman)) {
                if (ui.activeSelf == true)
                    if (Input.GetKeyDown (KeyCode.Escape)) {
                        if (pauseMenu.activeSelf == true)
                            pauseMenu.SetActive (false);
                        else 
                            pauseMenu.SetActive (true);
                    }
            }
        }


        /// <summary>
        /// Raises the GU event.
        /// </summary>
        void OnGUI ()
        {
            // If an active state has been assigned, call it's ShowUI.
            if (activeState != null)
                activeState.ShowUI ();
			
        }

        /// <summary>
        /// Switches the state.  
        /// </summary>
        public void SwitchState (IStateBase newstate)
        {
            Debug.Log ("Switched State");
            // Set active state to the new state 
            activeState = newstate;
        }

        /// <summary>
        /// Switches the state to the court state.  
        /// </summary>
        public void SwitchToCourt ()
        {
            Debug.Log ("Switching State");
            // Set active state to the new state 
            activeState = new Play_CourtState (this);

        }
        /// <summary>
        /// Switches the state to the main menu 
        /// </summary>
        public void SwitchToMenu ()
        {
            Debug.Log ("Switching State");
            // Set active state to the new state 
            activeState = new Start_MenuState (this);
		
        }

        public void SwitchToCastle ()
        {
            Debug.Log ("Switching State");
            // Set active state to the new state 
            activeState = new Play_CastleState (this);
        }

        public void SwitchToConquest()
        {
            activeState = new Play_ConquestState (this);
        }

        /// <summary>
        /// Switches the state to the end state.  
        /// </summary>
        public void SwitchToVictory ()
        {
            Debug.Log ("Switching to End state");
            // Set active state to the new state
            activeState = new EndState (this);
        }

        public void SwitchToLoss ()
        {
            Debug.Log ("Switching to Loss state");
            // Set active state to the new state
            activeState = new LossState (this);
        }

        /// <summary>
        /// Starts a new game at the start menu.
        /// </summary>
        public void NewGame ()
        {
            Debug.Log ("Started new game.");
            // Switch to the Start Menu
            SwitchState (new Start_MenuState (this));

            gameDataRef.SetVictoryState (GameData.VictoryState.None);

            gameDataRef.ResetPlayers ();
            gameDataRef.ResetAreas ();

            m_gameTurn = 0;
            m_phasingPlayer = null;
            m_phasingPlayerIndex = 0;

            // Make the Start Menu Active
            startMenuManagerRef.ActivateStartMenu ();
            // Reset the player selection menu
            startMenuManagerRef.ResetPlayerSelectionMenu ();

            // The player is not ready to start
            startMenuManagerRef.ReadyToStart (false);
            // Reset the UI
            uiManagerRef.RestartUI ();

        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void StartNewGame()
        {
            // Switch to the Start Intro State					
            SwitchState (new Start_OpeningState (this));
            // Deactivate the Player Selection Menu
            startMenuManagerRef.DeactivateSelectionMenu ();
            // Set up the Active Players List
            gameDataRef.SetActivePlayers ();
            // Set up the starting armies
            gameDataRef.SetStartingArmies ();
            // Set up the starting castles
            gameDataRef.SetStartingCastles ();
            // Set up the starting empires
            gameDataRef.SetStartingEmpires ();
            // Set the starting stats
            gameDataRef.SetStartingStats();
            // Set the AI priority
            gameDataRef.SetAIPriority();
        }


        /// <summary>
        /// Starts the first turn with player 1, turn 1.
        /// </summary>
        public void StartFirstTurn ()
        {
            m_phasingPlayerIndex = 0;
            m_phasingPlayer = gameDataRef.activePlayers [m_phasingPlayerIndex];
            m_gameTurn = 1;
        }

        /// <summary>
        /// Starts a new player turn.
        /// </summary>
        public void NewPlayerTurn ()
        {
            // Set the date
            if (!m_loadingGame) {
                uiManagerRef.WhatDateIsIt ();
            }

            // If the phasing player is not the last in the active players list
            if (m_phasingPlayerIndex < gameDataRef.activePlayers.Count - 1) {
                // Make the next player in the list the phasing player
                m_phasingPlayerIndex++;
                m_phasingPlayer = gameDataRef.activePlayers [m_phasingPlayerIndex];
						
                // If the phasing player is the last player in the active players list
            } else {
                // Start a new game turn
                NewGameTurn ();
            }

            //Update UI and add a few years on the counter
            //uiManagerRef.year += 10 / gameDataRef.activePlayers.Count;
            uiManagerRef.WhatYearIsthis ();
            uiManagerRef.UpdateUI ();
				
        }

        /// <summary>
        /// Starts a new game turn.
        /// </summary>
        public void NewGameTurn ()
        {
            //empties the papal selected deck (as long as there is soemthing in it!)
            if (gameDataRef.deckPapal.selectedPapalCard [0] != null) {
                gameDataRef.deckPapal.ResolveSelectedPapalArray ();
            }
            // Save the game
            gameDataRef.SaveLoad.Save ();

            // increase the game turn by one
            m_gameTurn++;
            Debug.Log ("Current Turn is " + m_gameTurn);

            // Sort the active players list
            gameDataRef.SortActivePlayers ();

            // Reset phasing player to the first player in the active player list.
            m_phasingPlayerIndex = 0;
            m_phasingPlayer = gameDataRef.activePlayers [m_phasingPlayerIndex];


        }

        public void StartLoadedGame (int gameTurn, string gameDay, int gameMonthNumber)
        {
            m_loadingGame = true;
            m_gameTurn = gameTurn;
            m_phasingPlayerIndex = gameDataRef.activePlayers.Count - 1;
            m_phasingPlayer = gameDataRef.activePlayers [m_phasingPlayerIndex];
            uiManagerRef.SetLoadedDate (gameDay, gameMonthNumber);
            SwitchState (new Play_CastleState (this));
            m_loadingGame = false;
            Debug.Log ("Current Turn is " + m_gameTurn);
        }

        // Check if this is the game end
        public bool IsLastTurn ()
        {
            // If the game turn is the last turn, and the phasing player is the last active player in the turn sequence
            if ((m_gameTurn == gameDataRef.LastGameTurn) && (PhasingPlayerIndex == gameDataRef.activePlayers.Count - 1)) {
                gameDataRef.SetVictoryState (GameData.VictoryState.GameEnd);
                return true;
            } else
                return false;
        }
		

        // <summary>
        // Activates The UI in the apropriate scenes
        // </summary>
        public void ActivateUI ()
        {
            ui.SetActive (true);
        }
	
        // <summary>
        // Activates The UI in the appropriate scenes
        // </summary>
        public void DeactivateUI ()
        {
            ui.SetActive (false);
        }

        /// <summary>
        /// Restart this instance.
        /// </summary>
        public void Restart ()
        {
            // Destroy the game object & load scene 0
            Destroy (gameObject);
            Application.LoadLevel ("Game");
        }

        public void SetLoadingGame (bool newValue)
        {
            m_loadingGame = newValue;
        }
    }
}

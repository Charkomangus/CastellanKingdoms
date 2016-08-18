using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class ConquestMapManager : MonoBehaviour
    {

        // Game Data Reference
        private GameObject m_gameManager;
        private GameData m_gameDataRef;

        // State Manager Reference
        private StateManager m_stateManagerRef;


        // Map Reference
        public GameObject mapCanvas;
        public GameObject pitchedBattleCanvas;
        public GameObject siegeBattleCanvas;
        public GameObject invadePromptWindow;
        public GameObject[] invadePromptButtons;
        public Text invadePromptWindowText;
        public PitchedBattleManager pitchedBattleManager;
        public SiegeBattleManager siegeBattleManager;
        public GameObject gameVictoryPanel;
        public Image gameVictoryPanelImage;
        public Text gameVictoryText;
        public RectTransform mapImage;
        public Image[] shieldImage;
        public GameObject[] keepInArea;
        public GameObject[] regionOwned;
        public Sprite shieldDefault;
        public Sprite invadableArea;


        // Buttons Reference
        public Button[] AreaButtons;
        public GameObject EndTurnButton;

        // Area Reference
        private Area m_selectedArea;
        public AudioSource audioSource;


        // Called on startup
        void Awake ()
        {
            // Set up reference to Game Data via Game Manager
            m_gameManager = GameObject.Find ("GameManager");
            m_gameDataRef = m_gameManager.GetComponent<GameData> ();

            // Set up state manager ref
            m_stateManagerRef = m_gameManager.GetComponent<StateManager> ();
	
        }

        // Use this for initialization
        void Start ()
        {
            //gameVictoryPanelImage.sprite = m_stateManagerRef.PhasingPlayer.MyFaction.MyFlagSprite;
            // Set the map position to centre on the phaisng player's starting position
            mapImage.anchoredPosition = m_stateManagerRef.PhasingPlayer.MyFaction.StartingAreaMapPosition;

            for (int i = 0; i< m_gameDataRef.AllAreas.Length; i++) {
                m_gameDataRef.AllAreas [i].AssignMapButton (AreaButtons [i]);
            }
            // Assign new images to buttons
            for (int i = 0; i < m_gameDataRef.AllAreas.Length; i++) {
                // For areas which have a controlling player
                if (m_gameDataRef.AllAreas [i].ControllingPlayer != null) {
                    // Assign the controlling player's faction's flag
                    shieldImage [i].sprite = m_gameDataRef.AllAreas [i].ControllingPlayer.MyFaction.MyFlagSprite;
                }
                // For areas that don't have a controlling player
                else {
                    // Assign the default UI background image
                    shieldImage [i].sprite = shieldDefault;

			
                }
            }

            // Make all buttons non-interactable
            for (int i = 0; i < AreaButtons.Length; i++) {
                AreaButtons [i].interactable = false;
            }
            // Make all castle icons inactive
            for (int i = 0; i < keepInArea.Length; i++) {
                keepInArea [i].SetActive (false);
            }
            // Make all region owned icons inactive
            for (int i = 0; i < regionOwned.Length; i++) {
                regionOwned [i].SetActive (false);
            }
				

            // If the phasingplayer is human
            if (m_stateManagerRef.PhasingPlayer.IsHuman == true) {
                // Check all Areas in phasingplayer empire
                for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
                    // Set the buttons for owned areas to not be greyed out
                    ColorBlock colorBlock = ColorBlock.defaultColorBlock;
                    colorBlock.disabledColor = colorBlock.normalColor;
                    m_stateManagerRef.PhasingPlayer.myEmpire [i].MyMapButton.colors = colorBlock;
                    // Check all areas in the empire area's neighbours
                    for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
                        // If the area neighbour is not owned by the phasing player
                        if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer)
                            // Set their buttons to be interactable
                            m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].MyMapButton.interactable = true;
                        // If the button relates to an empty area
                        if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer == null) {
                            // Apply thr invadable area sprite
                            shieldImage [m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].AreaIndex].sprite = invadableArea;
                        }
                    }
                }
            } else {
                SetMuteAudio (true);
            }
            // Assign the pitched & siege battle manager
            pitchedBattleManager = GetComponent<PitchedBattleManager> ();
            siegeBattleManager = GetComponent<SiegeBattleManager> ();

            //Checks Regions & updates Map UI
            for (int i = 0; i < m_gameDataRef.activePlayers.Count; i++)
                m_gameDataRef.activePlayers [i].CheckRegions ();
            for (int i = 0; i < regionOwned.Length; i++) 
                regionOwned [i].SetActive (false);				


            //Activate End Turn for human players
            if (m_stateManagerRef.PhasingPlayer.IsHuman) {
                EndTurnButton.SetActive (true);
            } else {
                EndTurnButton.SetActive (false);
				
            }
            //Updates Map

            CheckAreaRegion ();
            CheckAreaKeep ();

				
				
        }

	
        // Update is called once per frame
        void Update ()
        {
	
        }

        // Mutes the audio
        public void SetMuteAudio (bool newValue)
        {
            audioSource.mute = newValue;
        }
	

        // When an area button is clicked
        public void AreaButtonClicked (int ButtonIndex)
        {
            m_selectedArea = m_gameDataRef.AllAreas [ButtonIndex];
            Debug.Log ("" + m_selectedArea.name + " has been clicked");

            // If the Area is unoccupied:
            if (m_selectedArea.ControllingPlayer == null) {
                // Switch off all of the buttons
                for (int i = 0; i < AreaButtons.Length; i++) {
                    AreaButtons [i].interactable = false;
                }

                // Activate the invade prompt window
                invadePromptWindow.SetActive (true);

                // If player has no army...
                if (m_stateManagerRef.PhasingPlayer.CountSoldiers() == 0) {
                    Debug.Log ("Player has no army.");
                    // Show a prompt to say "You have no army"
                    invadePromptWindowText.text = "You have no army.";
                    // Activate back button
                    invadePromptButtons [2].SetActive (true);
                } 
                // otherwise the player does have an army
                else {


                    // Show a prompt to say "Do you want to invade (area name)?"
                    invadePromptWindowText.text = "Do you want to invade " + m_selectedArea.name + "?";
                    // Activate yes and no buttons
                    invadePromptButtons [0].SetActive (true);
                    invadePromptButtons [1].SetActive (true);
                }
            }

            // If the Area is occupied by an enemy player:
            if ((m_selectedArea.ControllingPlayer != null) && (m_selectedArea.ControllingPlayer != m_stateManagerRef.PhasingPlayer)) {
                // Switch off all of the buttons
                for (int i = 0; i < AreaButtons.Length; i++) {
                    AreaButtons [i].interactable = false;
                }
                // Activate the invade prompt window
                invadePromptWindow.SetActive (true);

                // If player has no army...
                if ((m_stateManagerRef.PhasingPlayer.CountSoldiers() == 0) && (m_selectedArea.occupyingCastle == null)) {
                    Debug.Log ("You have no army.");
                    // Show a prompt to say "You have no army"
                    invadePromptWindowText.text = "You have no army.";
                    // Activate back button
                    invadePromptButtons [2].SetActive (true);
                } 
                // otherwise the player does have an army
                else {
                    // If the area does not contain a castle
                    if (m_selectedArea.occupyingCastle == null) {
                        // Show a prompt to say "Do you want to battle (enemy player) for (area name)?"
                        invadePromptWindowText.text = "Do you want to battle " + m_selectedArea.ControllingPlayer.MyFaction.name + " ( " + m_selectedArea.ControllingPlayer.name + ") " + " for " + m_selectedArea.name + "?";
                        // Activate yes and no buttons
                        invadePromptButtons [0].SetActive (true);
                        invadePromptButtons [1].SetActive (true);
                    } 
                    // If the area does contain a castle
                    else {
                        //if the phasing player has siege engines in their army
                        if (m_stateManagerRef.PhasingPlayer.HasSiegeEngines ()) {
                            // They get the option to besiege
                            invadePromptWindowText.text = "Do you want to besiege " + m_selectedArea.ControllingPlayer.name + "'s castle at " + m_selectedArea.name + "?";
                            // Activate yes and no buttons
                            invadePromptButtons [0].SetActive (true);
                            invadePromptButtons [1].SetActive (true);
                        }
                        //otherwise the phasing player has no siege engines and can't attack the castle
                        else {
                            invadePromptWindowText.text = "You need siege engines to attack a castle.";
                            // Activate back button
                            invadePromptButtons [2].SetActive (true);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Sets the selected area.
        /// This is called when the AI player has identified a target area to attack.
        /// </summary>
        public void SetSelectedArea (Area targetArea)
        {
            m_selectedArea = targetArea;
            Debug.Log ("Set target area to " + m_selectedArea);
        }

        // When no is clicked in the invade prompt window 
        public void NoToInvadePrompt ()
        {
            // Check all Areas in phasingplayer empire
            for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
                // Set the buttons for owned areas to not be greyed out
                ColorBlock colorBlock = ColorBlock.defaultColorBlock;
                colorBlock.disabledColor = colorBlock.normalColor;
                m_stateManagerRef.PhasingPlayer.myEmpire [i].MyMapButton.colors = colorBlock;
                // Check all areas in the empire area's neighbours
                for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
                    // If the area neighbour is not owned by the phasing player
                    if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer)
                        // Set their buttons to be interactable
                        m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].MyMapButton.interactable = true;
                }
            }


            // deselect the selected area
            m_selectedArea = null;
            // deactivate the invade prompt window
            invadePromptWindow.SetActive (false);
            // deactivate all the invade prompt buttons
            for (int i = 0; i < invadePromptButtons.Length; i++) {
                invadePromptButtons [i].SetActive (false);
            }
        }

        // When yes is clicked in the invade prompt window
        public void YesToInvadePrompt ()
        {
            Debug.Log ("Said yes to invade prompt at " + m_selectedArea);
//		//Turn The UI off
            m_stateManagerRef.DeactivateUI ();
            // Make the map inactive
            mapCanvas.SetActive (false);

            // If the occupying area does not contain a castle
            if (m_selectedArea.occupyingCastle == null) {
                // Set up a Pitched Battle:
                pitchedBattleCanvas.SetActive (true);
                // Assign the battle area and the battling players in pitched battle manager
                pitchedBattleManager.AssignBattleArea (m_selectedArea);
                pitchedBattleManager.AssignBattlingPlayers (m_stateManagerRef.PhasingPlayer, m_selectedArea.ControllingPlayer);
            }
            // Otherwise the occupying area does contain a castle
            else {
                // Set up a Siege Battle
                siegeBattleCanvas.SetActive (true);
                // Assign the siege area and the battling players in siege battle manager
                siegeBattleManager.AssignBattleArea (m_selectedArea);
                siegeBattleManager.AssignBattlingPlayers (m_stateManagerRef.PhasingPlayer, m_selectedArea.ControllingPlayer);
                siegeBattleManager.ClearAttackPanel ();
//			siegeBattleManager.MakeAttackButtonsNonInteractable();
                // Activate the choose units prompt
                siegeBattleManager.chooseUnitsPromptPanel.SetActive (true);
                // OK this automatically if both players are AI
                if ((!m_stateManagerRef.PhasingPlayer.IsHuman) && (!m_selectedArea.ControllingPlayer.IsHuman)) {

                    siegeBattleManager.chooseUnitsPromptButton.onClick.Invoke ();

                }
            }
        }


        // On exiting the pitched battle screen
        public void ExitPitchedBattle ()
        {
            Debug.Log ("Exiting the pitched battle at " + m_selectedArea);
            pitchedBattleManager.pitchedAnim.KillAnimation ();
            // deselect the selected area
            m_selectedArea = null;
            // unassign the battle area & battling players
            pitchedBattleManager.AssignBattleArea (null);
            pitchedBattleManager.AssignBattlingPlayers (null, null);
            pitchedBattleManager.ClearBattleArmies ();
            // deactivate the invade prompt window
            invadePromptWindow.SetActive (false);
            // deactivate all the invade prompt buttons
            for (int i = 0; i < invadePromptButtons.Length; i++) {
                invadePromptButtons [i].SetActive (false);
            }
            // Assign new images to buttons
            for (int i = 0; i < m_gameDataRef.AllAreas.Length; i++) {
                // For areas which have a controlling player
                if (m_gameDataRef.AllAreas [i].ControllingPlayer != null) {
                    // Assign the controlling player's faction's flag
                    shieldImage [i].sprite = m_gameDataRef.AllAreas [i].ControllingPlayer.MyFaction.MyFlagSprite;
                }
                // For areas that don't have a controlling player
                else {
                    // Assign the default UI background image
                    shieldImage [i].sprite = shieldDefault;

                } 

            }
            // If the phasingplayer is human
            if (m_stateManagerRef.PhasingPlayer.IsHuman == true) {
                // Check all Areas in phasingplayer empire
                for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
                    // Set the buttons for owned areas to not be greyed out
                    ColorBlock colorBlock = ColorBlock.defaultColorBlock;
                    colorBlock.disabledColor = colorBlock.normalColor;
                    m_stateManagerRef.PhasingPlayer.myEmpire [i].MyMapButton.colors = colorBlock;
                    // Check all areas in the empire area's neighbours
                    for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
                        // If the area neighbour is not owned by the phasing player
                        if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer)
                            // Set their buttons to be interactable
                            m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].MyMapButton.interactable = true;
                        // If the button relates to an empty area
                        if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer == null) {
                            // Apply thr invadable area sprite
                            shieldImage [m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].AreaIndex].sprite = invadableArea;
                        }
                    }
                }
            }  
            // If the phasing player is not human
            else {
                // call the active state's state trigger
                // (this prompts the AI to take their next action)
                m_stateManagerRef.activeState.StateTrigger ();
            }

            //Checks Regions & updates Map UI
            for (int i = 0; i < m_gameDataRef.activePlayers.Count; i++)
                m_gameDataRef.activePlayers [i].CheckRegions ();
            for (int i = 0; i < regionOwned.Length; i++) 
                regionOwned [i].SetActive (false);				
            CheckAreaRegion ();

            //Checks Tax
            m_stateManagerRef.PhasingPlayer.CalculateTax ();

//		//Update & Activate UI for human players
            if (m_stateManagerRef.PhasingPlayer.IsHuman)
                m_stateManagerRef.ActivateUI ();
            else
                m_stateManagerRef.DeactivateUI ();

            // Activate & Update the UI

            m_stateManagerRef.uiManagerRef.UpdateUI ();

        }
	
	
        // On exiting the siege battle screen
        public void ExitSiegeBattle ()
        {

            // deselect the selected area
            m_selectedArea = null;
            // unassign the battle area & battling players
            siegeBattleManager.AssignBattlingPlayers (null, null);
            siegeBattleManager.AssignBattleArea (null);
            siegeBattleManager.ClearBattleArmies ();
            // deactivate the invade prompt window
            invadePromptWindow.SetActive (false);
            // deactivate all the invade prompt buttons
            for (int i = 0; i < invadePromptButtons.Length; i++) {
                invadePromptButtons [i].SetActive (false);
            }

            // Assign new images to buttons
            for (int i = 0; i < m_gameDataRef.AllAreas.Length; i++) {
                // For areas which have a controlling player
                if (m_gameDataRef.AllAreas [i].ControllingPlayer != null) {
                    // Assign the controlling polayer's faction's flag
                    shieldImage [i].sprite = m_gameDataRef.AllAreas [i].ControllingPlayer.MyFaction.MyFlagSprite;
                }
                // For areas that don't have a controlling player
                else {
                    // Assign the default UI background image
                    shieldImage [i].sprite = shieldDefault;
                }

            }

            // If no player has one by domination, and if there are human players left alive
            if ((!CheckForVictoryByDomination ()) && (!CheckForLossByElimination ())) {
                // If the phasingplayer is human
                if (m_stateManagerRef.PhasingPlayer.IsHuman == true) {
                    // Check all Areas in phasingplayer empire
                    for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
                        // Set the buttons for owned areas to not be greyed out
                        ColorBlock colorBlock = ColorBlock.defaultColorBlock;
                        colorBlock.disabledColor = colorBlock.normalColor;
                        m_stateManagerRef.PhasingPlayer.myEmpire [i].MyMapButton.colors = colorBlock;
                        // Check all areas in the empire area's neighbours
                        for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
                            // If the area neighbour is not owned by the phasing player
                            if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer)
                                // Set their buttons to be interactable
                                m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].MyMapButton.interactable = true;
                            // If the button relates to an empty area
                            if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer == null) {
                                // Apply thr invadable area sprite
                                shieldImage [m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].AreaIndex].sprite = invadableArea;
                            }
                        }
                    }
                } 
                // Otherwise assume the phasing player is AI
                else {
                    // Prompt next AI action
                    m_stateManagerRef.activeState.StateTrigger ();
                }
            }
            // If a player has won by domination
            if (CheckForVictoryByDomination ()) {
                SetGameVictoryPanel ();
            }
            // If all human players have been eliminated 
            if (CheckForLossByElimination ()) {
                SetGameVictoryPanel ();
            }
            //Checks Regions & updates Map UI
            for (int i = 0; i < m_gameDataRef.activePlayers.Count; i++)
                m_gameDataRef.activePlayers [i].CheckRegions ();
            for (int i = 0; i < regionOwned.Length; i++) 
                regionOwned [i].SetActive (false);				
            CheckAreaRegion ();

            //Checks Tax
            m_stateManagerRef.PhasingPlayer.CalculateTax ();

            // Activate & update the UI

            if (m_stateManagerRef.PhasingPlayer.IsHuman)
                m_stateManagerRef.ActivateUI ();
            else
                m_stateManagerRef.DeactivateUI ();
            m_stateManagerRef.uiManagerRef.UpdateUI ();
        }
	
        // This checks whether a player has won the game by domination
        bool CheckForVictoryByDomination ()
        {
            // If there is only one active player left, they have won:

            if (m_gameDataRef.activePlayers.Count == 1) {
                if (m_gameDataRef.activePlayers [0].IsHuman)
                    m_gameDataRef.SetVictoryState (GameData.VictoryState.Domination);
                return true;
            } else
                return false;
        }

        // This checks if all human players have been eliminated.
        // It returns true if all human players have been eliminated.
        bool CheckForLossByElimination ()
        {
            // If there are no human players left, the game is a loss
            bool humansDestroyed;
            humansDestroyed = true;
            for (int i = 0; i < m_gameDataRef.activePlayers.Count; i++) {
                if (m_gameDataRef.activePlayers [i].IsHuman) {
                    humansDestroyed = false;
                    break;
                }
            }
            if (humansDestroyed) {
                m_gameDataRef.SetVictoryState (GameData.VictoryState.HumansDestroyed);

                return true;

            } else {
                return false;
            }
        }
	

        // Set the victory panel dependant onhow the game has ended
        public void SetGameVictoryPanel ()
        {
            // Deactivate the AIplaying panel
            m_stateManagerRef.uiManagerRef.aiPlayingPanel.SetActive (false);
            m_stateManagerRef.uiManagerRef.aiPlayingMask.SetActive (false);
		
            // Make all the buttons uninteractable
            for (int i = 0; i < AreaButtons.Length; i++) {
                AreaButtons [i].interactable = false;
            }

            // Show the victory panel
            gameVictoryPanel.SetActive (true);

            switch (m_gameDataRef.GameVictoryState) {
                case GameData.VictoryState.Domination:
                {
                    // Set the panel image to the flag of the wining player
                    gameVictoryPanelImage.sprite = m_stateManagerRef.PhasingPlayer.MyFaction.MyFlagSprite;
                    gameVictoryText.text = "" + m_gameDataRef.activePlayers [0].MyFaction.name + " (" + m_gameDataRef.activePlayers [0].name + ") has won the game!";
                    break;
                }
                case GameData.VictoryState.GameEnd:
                {
                    gameVictoryText.text = "The game is over.";
                    break;
                }
                case GameData.VictoryState.HumansDestroyed:
                {
                    gameVictoryText.text = "All human players have been defeated.";
                    break;
                }
            }
        }

        // Confirms victory of the winning player
        public void OKToGameVictory ()
        {
            switch (m_gameDataRef.GameVictoryState) {
		
                case GameData.VictoryState.Domination:
                {
                    // Switch to the end state
                    m_stateManagerRef.SwitchToVictory ();
                    Debug.Log ("" + m_gameDataRef.activePlayers [0].name + " has won the game by domination");

                    break;
                }
                case GameData.VictoryState.GameEnd:
                {
                    // Swtich to end state
                    m_stateManagerRef.SwitchToVictory ();
                    Debug.Log ("Game ended due to timing out.");
                    break;

                }
                case GameData.VictoryState.HumansDestroyed:
                {
                    // Switch to loss state
                    m_stateManagerRef.SwitchToLoss ();
                    Debug.Log ("Game ended as all human players have been defeated.");			
                    break;
                }
            }


        }

        //Check which areas have a keep and assign the apropriate icon
        private void CheckAreaKeep ()
        {
            for (int i = 0; i < m_gameDataRef.AllAreas.Length; i++) {
                if (m_gameDataRef.AllAreas [i].occupyingCastle != null)
                    keepInArea [i].SetActive (true);
            }
        }


        //Check which areas have a keep and assign the apropriate icon
        private void CheckAreaRegion ()
        {

            for (int a = 0; a < m_gameDataRef.activePlayers.Count; a++) {
                for (int i = 0; i < m_gameDataRef.activePlayers[a].regionsOwned.Count; i++) {
                    for (int j = 0; j < m_gameDataRef.AllAreas.Length; j++) {
                        if (m_gameDataRef.AllAreas [j].tag == m_gameDataRef.activePlayers [a].regionsOwned [i])
                            regionOwned [j].SetActive (true);
//										else
//											regionOwned [j].SetActive (false);
										
                    }
                }
            }

        }

        // Called via onclick event when end turn is pressed
        public void EndTurnPressed ()
        {
            // Switch off all of the buttons
            for (int i = 0; i < AreaButtons.Length; i++) {
                AreaButtons [i].interactable = false;
            }

        }

        // Called when pressed yes to confirm end turn 
        public void EndTurn ()
        {
            //Check to see if this is the last turn
            if (m_stateManagerRef.IsLastTurn ()) {
                // If so switch to the end state of the game
                SetGameVictoryPanel ();
            } else {
                // If not switch to the castle state (i.e. start the next turn as normal)
                m_stateManagerRef.uiManagerRef.aiPlayingMask.SetActive (true);
                m_stateManagerRef.SwitchToCastle ();
            }
        }


    }
}

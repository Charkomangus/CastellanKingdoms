using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class SiegeBattleManager : MonoBehaviour
    {
        // Siege state
        public enum SiegeState
        {
            Prepare,
            Attack
        }

        // Game Data
        private GameObject m_gameManager;
        private GameData m_gameDataRef;
        private StateManager m_stateManager;
        private ConquestMapManager m_conquestMapManager;

        // Siege Battle Info
        private Area m_battleArea;

        public Area BattleArea
        { get { return m_battleArea; } }

        private Player m_attacker;
        public Player Attacker
        { get { return m_attacker; } }

        private Player m_defender;
        public Player Defender
        { get { return m_defender; } }

        private Castle m_besiegedCastle;
        private int m_attackScore;
        private int m_defenceScore;
        private SiegeState m_siegeState;
        private int m_attritionPoints;
        private CardMilitary m_attackingUnit;
        private CardMilitary m_defendingUnit;
        public CardMilitary[] attackingArmy;
        public List<CardMilitary> defendingArmy;

        // UI Elements
        public GameObject chooseUnitsPromptPanel;
        public Button chooseUnitsPromptButton;
        public Text[] chooseUnitsPromptText;
        public GameObject[] attackButtons;
        public Text[] attackButtonsText;
        public GameObject unitMenu;
        public Button[] unitMenuButtons;
        public Text[] unitMenuQuantities;
        public Button readyButton;
        public Button clearButton;
        public GameObject defencePanel;
        public Text defenceText;
        public Text attritionText;
        public GameObject victoryPanel;
        public Text[] victoryText;
        public Button victoryButton;
        public GameObject siegePanel;
        public Text siegePanelText;
        public Button retreatButton;


        // Use for priority initialisation
        void Awake ()
        {
            // Assign game data reference via game manager
            m_gameManager = GameObject.Find ("GameManager");
            m_gameDataRef = m_gameManager.GetComponent<GameData> ();
            m_stateManager = m_gameManager.GetComponent<StateManager> ();
            m_conquestMapManager = GetComponent<ConquestMapManager> ();
        }


        // Use this for initialization
        void Start ()
        {
            attackingArmy = new CardMilitary[10];
        }
	
        // Update is called once per frame
        void Update ()
        {
	
        }

        // Assigns the area in which the battle is taking place
        // Also assigns the castle which is being besieged
        public void AssignBattleArea (Area battleArea)
        {
            // Assign the battle area
            m_battleArea = battleArea;
            if (battleArea != null) {
                //Set the besieged castle to my occupying castle
                m_besiegedCastle = m_battleArea.occupyingCastle;
                // Debug messages
                if (m_battleArea != null)
                    Debug.Log ("" + m_battleArea.name + " is under siege");
                else 
                    Debug.Log ("No one is under siege.");
            }

        }
	
        // Assign the players who are battling
        public void AssignBattlingPlayers (Player Attacker, Player Defender)
        {
            if (Attacker != null && Defender != null) {
                // Assign the attacking and defending players
                m_attacker = Attacker;
                m_defender = Defender;
                // The attacking player is active first
                //m_activePlayerType = ActivePlayerType.Attacker;

                // Form the defending army now
                FormDefendingArmy ();

                // Set the siege state to prepare
                m_siegeState = SiegeState.Prepare;

                // Unmute the sound if the defender is human
                if (m_defender.IsHuman)
                {
                    m_conquestMapManager.SetMuteAudio(false);
                }

                // Set the choose unit prompt text
                SetChooseUnitPromptText ();

                // Debug messages
                if (m_attacker != null)
                    Debug.Log ("" + m_attacker + " is attacking.");
                else 
                    Debug.Log ("" + "No one is attacking");
                if (m_defender != null)
                    Debug.Log ("" + m_defender + "is defending");
                else
                    Debug.Log ("" + "No one is defending");
            }
        }

        public void SetChooseUnitPromptText ()
        {
            chooseUnitsPromptText [0].text = "" + m_defender.MyFaction.name + " (" + m_defender.name + ")'s";
            chooseUnitsPromptText [1].text = "castle at " + m_battleArea.name + " is under siege from";
            chooseUnitsPromptText [2].text = "" + m_attacker.MyFaction.name + " (" + m_attacker.name + ").";

            if (m_attacker.IsHuman) {
                chooseUnitsPromptText [3].text = "" + m_attacker.name + ", choose your attacking units.";
            } else {
                chooseUnitsPromptText [3].text = "Press OK to proceed.";

            }


        }

        // Called when 'OK' is clicked on the 'choose your attacking units' prompt
        public void OKToChooseUnitsPrompt ()
        {
            Debug.Log ("Pressed OK to choose units prompt.");
            if (m_attacker.IsHuman) {
                Debug.Log ("The attacker is human.");
                // Deactivate all attack buttons except the first
                for (int i = 0; i < attackButtons.Length; i++) {
                    if (i == 0) {
                        attackButtons [i].SetActive (true);
                        Button firstButton;
                        firstButton = attackButtons [i].GetComponent<Button> ();
                        firstButton.interactable = true;
                        attackButtonsText [0].text = "Choose Unit";
                        Debug.Log ("The first button is active and interactable");
                    } else
                        attackButtons [i].SetActive (false);
                }
            } else {
                m_stateManager.activeState.StateTrigger ();
            }
        }

        // Called when an Attack Button is clicked 
        // Parameter is a reference to allow identification of which button has been clicked
        // Behaviour is different depending on whether the player is preparing or attacking
        // Clicking in prepare mode allows a new unit to be assigned to the attacking army
        // Clikcing in attack mode allows the unit linked to the button to attack the castle defences
        public void ChooseAttackingUnitClicked (int buttonIndex)
        {
            // Check siege state
            switch (m_siegeState) {
                // In the prepare state
                case SiegeState.Prepare:
                {
                    // Make all buttons except the one which has been clicked non-interactable
                    for (int i = 0; i < attackButtons.Length; i++) {
                        if (i != buttonIndex) {
                            Button attackButtonsButton;
                            attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                            attackButtonsButton.interactable = false;
                        }
                    }
                    // Make the other navigational buttons non-interactable
                    readyButton.interactable = false;
                    clearButton.interactable = false;
                    // Activate the unit menu
                    unitMenu.SetActive (true);
                    // Refresh the unit menu quatities
                    SetUnitMenuQuantities ();
                    break;
                }
                // In the attack state
                case SiegeState.Attack:
                {
                    ResolveCombat ();
                    break;
                }
            }
        }

        // This makes all of the attack buttons interactable
        // It also makes the other navigational buttons interactable
        // It is usually called on exiting the unit selection menu
        public void MakeAttackButtonsInteractable ()
        {
            // Make all active attack buttons interactable via their Button component
            for (int i = 0; i < attackingArmy.Length; i++) {
                Button attackButtonsButton;
                attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                attackButtonsButton.interactable = true;	
            }

            // if siege is in the preparation stage make the clear and ready buttons interactable
            if (m_siegeState == SiegeState.Prepare) {
                clearButton.interactable = true;

                // The player must have at lease one siege engine in their army to begin the siege
                // If the attacking army includes any siege engines, make the 'ready' button interactable
                // Otherwise make it non-interactable
                for (int i = 0; i < attackingArmy.Length; i++) {
                    if (attackingArmy [i] != null) {
                        if ((attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Ram) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Belfry) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Artillery)) {
                            readyButton.interactable = true;
                            break;
                        } else
                            readyButton.interactable = false;
                    }
                }
            }
        }

        // This makes all of the attack buttons (and navigational buttons) non interactable
        // If it usually called on pressing the ready button
        public void MakeAttackButtonsNonInteractable ()
        {
            // Make all attack buttons interactable via their Button component
            for (int i = 0; i < attackButtons.Length; i++) {
                Button attackButtonsButton;
                attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                attackButtonsButton.interactable = false;	
            }
            // Make navigational buttons non-interactable
            readyButton.interactable = false;
            clearButton.interactable = false;

        }

        // Make the button corresponding to the last attacking unit interactable
        // This is called on entering the attack state of the siege
        public void MakeLastButtonActive ()
        {
            // Check through the attacking army
            for (int i = 0; i < attackingArmy.Length; i++) {
                // on finding the first null entry
                if (attackingArmy [i] == null) {
                    // Make the corresponding button non-active
                    // (a blank button is only required during army selection)
                    attackButtons [i].SetActive (false);
                    // If the phasing player is a human player
                    if (m_stateManager.PhasingPlayer.IsHuman) {
                        // Make the previous button (i.e. the last one with a unit assigned to it) interactable via its Button componenent
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i - 1].GetComponent<Button> ();
                        attackButtonsButton.interactable = true;
                    }

                    break;
                }
                // if current index is not empty and this is the last slot
                else if (i == attackingArmy.Length - 1) {
                    // If the phasing player is a human player
                    if (m_stateManager.PhasingPlayer.IsHuman) {
                        // Make this button interactable via its Button componenent
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                        attackButtonsButton.interactable = true;
                    }
                }
            }
        }

        // Sets the quantities next to each unit type in the unit menu
        public void SetUnitMenuQuantities ()
        {
            // Count the attacker's army units
            m_attacker.CountArmyUnits ();
            // Update the UI text in respect of the counted units
            for (int i = 0; i < unitMenuQuantities.Length; i++) {
                unitMenuQuantities [i].text = m_attacker.myArmyUnitTotals [i].ToString ();
                // If the attacker has run out of a unit type
                if (m_attacker.myArmyUnitTotals [i] == 0) {
                    // Turn off the button for that unit type
                    unitMenuButtons [i].interactable = false;
                } else 
                    unitMenuButtons [i].interactable = true;
            }
        }

        // Activates the next attack button 
        // (as long as there is another attack button left to activate)
        void ActivateNextButton (int buttonIndex)
        {
            if (buttonIndex < attackButtons.Length) {
                attackButtons [buttonIndex].SetActive (true);

            }
        }
		
        // Assigns a unit of a certain type to the attacking side.
        // Removes the unit from the attacker's army list
        public void AssignUnitToBattle (string UnitType)
        {
            switch (UnitType) {
                case "Spearmen":
                {
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                        if (attackButtonsButton.interactable == true) {
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            } 
                            // if the button hasn't already been assigned, add a new attack button on assignment
                            else {
                                ActivateNextButton (i + 1);
                            }
						
                            // add a spearman unit to the attacking army
                            // subtract a spearmen from the attacker's army list
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Spearmen) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            if (m_attacker.IsHuman) {
                                // Set the button's text and reset the unit menu 
                                attackButtonsText [i].text = "Spearmen";
                                SetUnitMenuQuantities ();
                            }
                        }
                    }
                    break;
                }

                case "Archers":
                {
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                        if (attackButtonsButton.interactable == true) {
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            } 
                            // if the button hasn't already been assigned, add a new attack button on assignment
                            else {
                                ActivateNextButton (i + 1);
                            }
						
                            // add an archers unit to the attacking army at the selected point
                            // subtract an archers unit from the attacker's army list
						
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Archers) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            if (m_attacker.IsHuman) {
                                // Set the button's text and reset the unit menu 
                                attackButtonsText [i].text = "Archers";
                                SetUnitMenuQuantities ();
                            }
                        }
                    }
                    break;

                }
                case "Knights":
                {
				
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                        if (attackButtonsButton.interactable == true) {
						
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            } 
                            // if the button hasn't already been assigned, add a new attack button on assignment
                            else {
                                ActivateNextButton (i + 1);
                            }
						
                            // add a knights unit to the attacking army at the selected point
                            // subtract a knights unit from the attacker's army list
						
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Knights) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            if (m_attacker.IsHuman) {
                                // Set the button's text and reset the unit menu 
                                attackButtonsText [i].text = "Knights";
                                SetUnitMenuQuantities ();
                            }
						
                        }
                    }
                    break;
                }
                case "Ram":
                {
			
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                        if (attackButtonsButton.interactable == true) {
					
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            } 
                            // if the button hasn't already been assigned, add a new attack button on assignment
                            else {
                                ActivateNextButton (i + 1);
                            }
					
                            // add a ram unit to the attacking army at the selected point
                            // subtract a ram unit from the attacker's army list
					
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Ram) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            if (m_attacker.IsHuman) {
                                // Set the button's text and reset the unit menu 
                                attackButtonsText [i].text = "Ram";
                                SetUnitMenuQuantities ();
                            }
					
                        }
                    }
                    break;
                }
                case "Belfry":
                {
			
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                        if (attackButtonsButton.interactable == true) {
					
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            } 
                            // if the button hasn't already been assigned, add a new attack button on assignment
                            else {
                                ActivateNextButton (i + 1);
                            }
					
                            // add a belfry unit to the attacking army at the selected point
                            // subtract a belfry unit from the attacker's army list
					
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Belfry) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            if (m_attacker.IsHuman) {
                                // Set the button's text and reset the unit menu 
                                attackButtonsText [i].text = "Tower";
                                SetUnitMenuQuantities ();
                            }
					
                        }
                    }
                    break;
                }
                case "Artillery":
                {
			
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        Button attackButtonsButton;
                        attackButtonsButton = attackButtons [i].GetComponent<Button> ();
                        if (attackButtonsButton.interactable == true) {
					
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            } 
                            // if the button hasn't already been assigned, add a new attack button on assignment
                            else {
                                ActivateNextButton (i + 1);
                            }
					
                            // add an artillery unit to the attacking army at the selected point
                            // subtract an artillery unit from the attacker's army list
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Artillery) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            if (m_attacker.IsHuman) {
                                // Set the button's text and reset the unit menu 
                                attackButtonsText [i].text = "Artillery";
                                SetUnitMenuQuantities ();
                            }
                        }
                    }
                    break;
                }
            }
        }

        // Clear the attack panel
        public void ClearAttackPanel ()
        {
            // Check the attacking army
            for (int i = 0; i < attackingArmy.Length; i++) {
                // If any army slot is assigned
                if (attackingArmy [i] != null) {
                    // add the currently assigned one back to the player's army list
                    m_attacker.AddToArmyList (attackingArmy [i]);
                    // remove it from the attacking army
                    attackingArmy [i] = null;
                    // reset the corresponding button text 
                    //attackButtonsText [i].text = "Choose Unit";

                }
            }
            // reset the corresponding button text 
            for (int i = 0; i < attackButtonsText.Length; i++) {
                attackButtonsText [i].text = "Choose Unit";
            }

            // Deactivate all attack buttons excet the first.
            for (int i = 0; i < attackButtons.Length; i++) {
                if (i == 0) {
                    attackButtons [i].SetActive (true);
                } else {
                    attackButtons [i].SetActive (false);
                }
            }

            // reset the menu quantities
            SetUnitMenuQuantities ();

            // once clear, the player is not ready for battle
            readyButton.interactable = false;

        }

        // Auto-set an AI attacking army
        public void SetAIAttack ()
        {
            Debug.Log ("Setting AI attack for " + m_attacker.name + "'s siege of " + m_battleArea);
            // For all attacking army slots
            for (int i = 0; i < attackingArmy.Length; i++) {
                // Check the attacker's army unit totals (from the end of the collection, so that the most powerful units are checked first)
                for (int j = m_attacker.myArmyUnitTotals.Length - 1; j >= 0; j--) {
                    // If the total at j is greater than 0
                    if (m_attacker.myArmyUnitTotals [j] > 0) {
                        // Check through the attacker army
                        for (int k = 0; k < m_attacker.myArmy.Count; k++) {
                            // if j is equal to or greater than 3 (i.e. if it's referring to a siege engine) 
                            if (j >= 3) {
                                // Adjust the reference to correspond to the Siege Engines collection in game data
                                if (m_attacker.myArmy [k].myMilitaryType == m_gameDataRef.SiegeEngines [j - 3].myMilitaryType) {
                                    // Assign this unit to the attacking army, remove it from the attacker's army, and break from this loop.
                                    attackingArmy [i] = m_attacker.myArmy [k];
                                    m_attacker.myArmy.RemoveAt (k);
                                    break;
                                }
                            }
                            // Otherwise j is less than 3 (i.e. referring to a soldiers unit)
                            else {
                                // Compare the reference to the Soldiers collection in game data
                                if (m_attacker.myArmy [k].myMilitaryType == m_gameDataRef.Soldiers [j].myMilitaryType) {
                                    // Assign this unit to the attacking army, remove it from the attacker's army, and break from this loop.
                                    attackingArmy [i] = m_attacker.myArmy [k];
                                    m_attacker.myArmy.RemoveAt (k);
                                    break;
                                }
                            }
                        }
                    }
                    // If a unit has been assigned at this slot, break and check the next one
                    if (attackingArmy [i] != null)
                        break;
                }
            }
        }

        // This is called after the AI Army has been set.
        // It assigns the relevant text to the attack buttons and makes the buttons non-interactable.
        public void SetAIAttackPanel ()
        {
            Debug.Log ("Setting AI attack panel for " + m_attacker.name + "'s siege of " + m_battleArea);
            // For each attacking army slot
            for (int i = 0; i < attackingArmy.Length; i++) {
                // If a unit has been assigned to this slot
                if (attackingArmy [i] != null) {
                    attackButtons [i].SetActive (true);
                    // Set this corresponding button's text to the unit
                    attackButtonsText [i].text = attackingArmy [i].myMilitaryType.ToString();
                    if (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Belfry)
                        attackButtonsText [i].text = "Tower";

                }
                // If no unit has been assigned to this slot
                else {
                    break;
                }
            }
            // Make all buttons non-interactable
            MakeAttackButtonsNonInteractable ();
            // Actvate Defence Panel	
            defencePanel.SetActive (true);
        }

        // Make the Defender's Army
        void FormDefendingArmy ()
        {
            // The Keep in the battle area's castle is the "bottom card of the deck"
            defendingArmy.Add (m_besiegedCastle.castleDefences [0]);

            // Followed by any occupying army units
            // Sort the occupying units
            m_battleArea.SortOccupyingUnits ();
            // Check through occupying units
            for (int i = 0; i < m_battleArea.occupyingUnits.Length; i++) {
                // If there is an army unit occupying this index
                if (m_battleArea.occupyingUnits [i] != null) {
                    // add it to the defence
                    defendingArmy.Add (m_battleArea.occupyingUnits [i]);
                }
            }

            // Followed by any other units in the defender's castle
            // (iteration starts at 1 here since we've already added the keep above)
            for (int i = 1; i < m_besiegedCastle.castleDefences.Count; i++) {
                defendingArmy.Add (m_besiegedCastle.castleDefences [i]);
            }

        }

        // Starts a siege attack
        // This is usually called when "Yes" is clicked in the "Are You Sure" prompt for starting the siege attack
        public void StartSiegeAttack ()
        {
            Debug.Log ("Starting attack in " + m_attacker.name + "'s siege of " + m_battleArea);
            // Set siege state to attack
            m_siegeState = SiegeState.Attack;
            // Set the defence text to the 'top card' of the defending castle deck
            defenceText.text = defendingArmy [defendingArmy.Count - 1].myMilitaryType.ToString();
            // Set the siege Panel Text
            siegePanelText.text = "Click a unit to attack.";	

            // If the Attacker is an AI player
            if (!m_attacker.IsHuman) {
                // Activate the siege panel
                siegePanel.SetActive (true);
                // Set the siege text
                siegePanelText.text = "" + m_attacker.MyFaction.name + " (" + m_attacker.name + ") is attacking...";	
                // Deactivate the retreat button
                retreatButton.interactable = false;
            }
        }

        // This resolves one combat action
        public void ResolveCombat ()
        {
            // Identify attacking unit
            // Find the last unit in the attacking army
            for (int i = 0; i < attackingArmy.Length; i++) {
                // If the current index is empty
                if (attackingArmy [i] == null) {
                    // Attack unit is the last index
                    m_attackingUnit = attackingArmy [i - 1];
                    break;
                }
                // if current index is not empty and this is the last slot
                else if (i == attackingArmy.Length - 1) {
                    // Attacking unit is this index
                    m_attackingUnit = attackingArmy [i];

                }
            }

            // Identify defending unit
            m_defendingUnit = defendingArmy [defendingArmy.Count - 1];

            // Write in debug
            Debug.Log ("attacking unit is: " + m_attackingUnit);
            Debug.Log ("defending unit is: " + m_defendingUnit);

            // Calculate combat values:
            m_attackingUnit.CalculateCombatValue (CardMilitary.BattleType.Siege, m_defendingUnit.myMilitaryType);
            m_defendingUnit.CalculateCombatValue (CardMilitary.BattleType.Siege, m_attackingUnit.myMilitaryType);

            // Assign combat values:
            m_attackScore = m_attackingUnit.combatScore + m_attritionPoints;
            m_defenceScore = m_defendingUnit.combatScore;
            // FOR TESTING ONLY: AUTOLOSE DEFENCE
            //m_defenceScore = 0;

            // Write in debug
            Debug.Log ("attacker's score is: " + m_attackingUnit.combatScore + " + " + m_attritionPoints + " = " + m_attackScore);
            Debug.Log ("defender's score is: " + m_defenceScore);

            // Compare combat values to determine winner:
            // If the attacker's score beats the defender's score...
            if (m_attackScore > m_defenceScore) {
                // the attacker wins the combat
                Debug.Log ("Attacker Wins");
                // Reset attrition points
                m_attritionPoints = 0;
                attritionText.text = "" + m_attritionPoints;
                // Remove the defending unit from the defending army
                defendingArmy.RemoveAt (defendingArmy.Count - 1);
                // If the defending unit was a castle defence, 
                if ((m_defendingUnit.myMilitaryType == CardMilitary.MilitaryType.Keep) || (m_defendingUnit.myMilitaryType == CardMilitary.MilitaryType.Wall) || (m_defendingUnit.myMilitaryType == CardMilitary.MilitaryType.Defence)) {
                    // remove the defending unit from the defending castle
                    m_besiegedCastle.castleDefences.RemoveAt (m_besiegedCastle.castleDefences.Count - 1);
                }
                // Destroy the defending unit
                m_defendingUnit.DestroyUnit ();


                // Check if there are any units left in the defending army:
                // If there are no units left
                if (defendingArmy.Count == 0) {
                    // The castle has been destroyed
                    defenceText.text = "The keep was breached!";
                    // Remove castle from player by checking through all of the defender's castles
                    for (int i = 0; i < m_defender.myCastles.Count; i++) {
                        // and removing the besieged one
                        if (m_defender.myCastles [i] == m_besiegedCastle)
                            m_defender.myCastles.RemoveAt (i);
                    }
                    // Remove castle from area
                    m_battleArea.occupyingCastle = null;

                    // Destroy the castle
                    m_besiegedCastle.DestroyCastle ();

                    // set besieged castle to null
                    m_besiegedCastle = null;

                    // The attacker wins the siege
                    ApplyVictory (m_attacker);
                }
                // If there are units left
                else {
                    // update the defence text
                    defenceText.text = defendingArmy [defendingArmy.Count - 1].myMilitaryType.ToString();
                }
            }
            // On any other result...
            else {
                // the defender wins the combat
                Debug.Log ("Defender Wins");
                // Add the attacker's combat score to the attrition points
                m_attritionPoints += m_attackScore;
                attritionText.text = "" + m_attritionPoints;
                // Remove the attacking unit from the attacking army
                for (int i = 0; i < attackingArmy.Length; i++) {
                    if (attackingArmy [i] == m_attackingUnit) {
                        attackingArmy [i] = null;
                        break;
                    }
                }
                // Destroy the attacking unit 
                m_attackingUnit.DestroyUnit ();

                // Check if there are any units left in the attacking army
                if (attackingArmy [0] != null) {
                    // Resize buttons and panel to allow combat to continue
                    MakeLastButtonActive ();
                }
                // otherwise, all units have been destroyed, so the defender wins the siege
                else 
                    ApplyVictory (m_defender);
            }
        }

        public void AttackerRetreat ()
        {
            ApplyVictory (m_defender);
        }

        // Apply the result of the victory, dependant on which player won
        void ApplyVictory (Player victoriousPlayer)
        {
            // Set attrition to 0
            m_attritionPoints = 0;
            // If the attacker won the siege
            if (victoriousPlayer == m_attacker) {
                Debug.Log ("Attacker wins the siege.");
                // (Defending units have all been destroyed in the siege so no action required there)
                // Remove the area from the defender's empire
                for (int i = 0; i < m_defender.myEmpire.Count; i++) {
                    if (m_defender.myEmpire [i].name == m_battleArea.name) {
                        m_defender.myEmpire.RemoveAt (i);
                        m_defender.LoseInfluence ();
                    }
                }
                // Check if the defending player has been wiped out
                // (i.e. have all of their castles been destroyed?)
                if (m_defender.myCastles.Count == 0) {
                    // If so remove the player as an active player
                    m_gameDataRef.RemoveActivePlayer (m_defender);
                    // Set the relevant text to the name of the defeated player
                    victoryText [3].text = ("" + m_defender.name + " has been defeated.");
                }
                // otherwise make that text line blank
                else
                    victoryText [3].text = "";

                // Deal with adding the winning player to the area:
                // Add the Area to the victor's empire
                victoriousPlayer.AddToEmpire (m_battleArea);
                // Assign remaining units to the conquered area:
                // Check through the attacking army
                for (int i = 0; i < attackingArmy.Length; i++) {
                    // If the current slot contains a unit
                    if (attackingArmy [i] != null) {
                        // clear the attack button text 
                        attackButtonsText[i].text = "Choose Unit";
                        // if the unit is a soldiers unit
                        if ((attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Spearmen) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Archers) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Knights)) {
                            // If the area has space for the unit
                            if (!m_battleArea.AreaFull())
                                // assign it to the area
                                m_battleArea.AssignOccupyingUnit (attackingArmy [i], false);
                            //otherwise
                            else
                                m_attacker.AddToArmyList(attackingArmy[i]);

                        }
                        // otherwise
                        else {
                            // destroy the unit (siege engines are not reusable)
                            attackingArmy [i].DestroyUnit ();
                        }
                        attackingArmy [i] = null;
                    }
                }

                // Give the player a new castle with a keep
                victoriousPlayer.AddNewCastle (m_gameDataRef.castle, m_gameDataRef.Defences [2]);
                // Assign the new castle to the area
                m_battleArea.AssignCastle (victoriousPlayer.myCastles [victoriousPlayer.myCastles.Count - 1]);

                // Update the UI:
                // Deactivate the siege attack buttons
                siegePanel.SetActive (false);
                MakeAttackButtonsNonInteractable ();
                // Show the victory panel
                victoryPanel.SetActive (true);
                victoryText [0].text = "" + m_attacker.MyFaction.name + " (" + m_attacker.name + ")";
                victoryText [1].text = "has captured";
                victoryText [2].text = "" + m_battleArea.name + " Castle";
                // OK automatically if both players were AI
                if ((!m_attacker.IsHuman) && (!m_defender.IsHuman))
                {
                    victoryButton.onClick.Invoke();
                }
            } 
            // Otherwise... 
            else {
                // The Defender has won the siege
                Debug.Log ("Defender wins the siege.");

                // Disband the attacking army (if any units remain)
                // check through the attacking army:
                for (int i = 0; i < attackingArmy.Length; i++) {
                    // If the current slot contains a unit
                    if (attackingArmy [i] != null) {
                        attackButtonsText[i].text = "Choose Unit";
                        // if the unit is a soldiers unit
                        if ((attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Spearmen) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Archers) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Knights)) {
                            // Add this unit back to the attacker's army list
                            m_attacker.AddToArmyList (attackingArmy [i]);
                        }
                        // Otherwise assume the unit is a siege engine
                        else {
                            // Siege engines are one-use only, so destroy it
                            attackingArmy [i].DestroyUnit ();
                        }
                        // Clear the attacking army slot
                        attackingArmy [i] = null;
                    }
                }

                // Clear the defending army
                defendingArmy.Clear ();

                // Update the UI:
                // Deactivate the siege attack buttons
                siegePanel.SetActive (false);
                MakeAttackButtonsNonInteractable ();
                // Show the victory panel
                victoryPanel.SetActive (true);
                victoryText [0].text = "" + m_attacker.MyFaction.name + " (" + m_attacker.name + ")";
                victoryText [1].text = "has failed to capture";
                victoryText [2].text = "" + m_battleArea.name + " Castle";
                victoryText [3].text = "";
                // OK automatically if both players were AI
                if ((!m_attacker.IsHuman) && (!m_defender.IsHuman))
                {
                    victoryButton.onClick.Invoke();
                }
            }
        }

        // Clears both the attacking and defending army collections
        // Used when exiting the battle to ensure these are blank for the next combat
        public void ClearBattleArmies ()
        {
            // Clear the attacking army
            // Clear the attack panel buttons
            ClearAttackPanel ();
            // Clear the defending army
            for (int i = 0; i < defendingArmy.Count; i++) {
                defendingArmy.RemoveAt (i);
            }
            // Set the defence text to default
            defenceText.text = "Defence";

        }

        public void DisableSoundIfAI()
        {
            if (!m_stateManager.PhasingPlayer.IsHuman) {
                m_conquestMapManager.SetMuteAudio(true);
            }
        }


    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class PitchedBattleManager : MonoBehaviour
    {
        public enum ActivePlayerType
        {
            Attacker,
            Defender
        }

        // Game Data
        private GameObject m_gameManager;
        private GameData m_gameDataRef;
        private PitchedBattleAnim m_pitchedAnim; 
        public PitchedBattleAnim pitchedAnim
        { get { return m_pitchedAnim; } }

        // State Manager
        private StateManager m_stateManagerRef;

        // Conquest Manager
        private ConquestMapManager m_conquestMapManagerRef;

        // Area & Player references
        private Area m_battleArea;
        private Player m_attacker;

        public Player Attacker 
        { get { return m_attacker; } }

        private Player m_defender;

        public Player Defender 
        { get { return m_defender; } }

        private ActivePlayerType m_activePlayerType;
        public CardMilitary[] attackingArmy;
        public CardMilitary[] defendingArmy;
        public int[] attackScores;
        public int[] defenceScores;

        // UI element references
        public Button[] defenceButtons;
        public Button[] attackButtons;
        public Button[] defencePanelNavButtons;
        public Button[] attackPanelNavButtons;
        public Text[] attackButtonText;
        public Text[] defenceButtonText;
        public Text[] defencePromptText;
        public GameObject unitMenuPanel;
        public Button[] unitMenuButtons;
        public Text[] unitMenuQuantities;
        public GameObject defencePlaceUnitsPrompt;
        public Button attackPlaceUnitsPromptButton;
        public Text[] victoryPanelText;
        public GameObject victoryPanel;
        public Button victoryOKButton;
        public Text areaText;

        void Awake ()
        {
            m_gameManager = GameObject.Find ("GameManager");
            m_gameDataRef = m_gameManager.GetComponent<GameData> ();
            m_stateManagerRef = m_gameManager.GetComponent<StateManager>();
            m_pitchedAnim = GetComponent<PitchedBattleAnim> (); 
            m_conquestMapManagerRef = GetComponent<ConquestMapManager> ();
        }

        // Use this for initialization
        void Start ()
        {
            // initialise army arrays
            attackingArmy = new CardMilitary[3];
            defendingArmy = new CardMilitary[3];

            attackScores = new int[3];
            defenceScores = new int[3];
        }
	
        // Update is called once per frame
        void Update ()
        {
	
        }

        // Assigns the area in which the battle is taking place
        public void AssignBattleArea (Area battleArea)
        {
            // Assign the battle area
            m_battleArea = battleArea;
            // Debug messages
            if (m_battleArea != null)
                Debug.Log ("" + m_battleArea.name + " is under attack");
            else 
                Debug.Log ("No one is under attack.");
        }

        // Assign the players who are battling
        public void AssignBattlingPlayers (Player Attacker, Player Defender)
        {
            // Assign the attacking and defending players
            m_attacker = Attacker;
            m_defender = Defender;
            // The attacking player is active first
            m_activePlayerType = ActivePlayerType.Attacker;
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

        // Called when the OK button is pressed on the prompt for placing units
        public void OKToPlaceUnitsPrompt ()
        {
            // If the active player is the attacker
            if (m_activePlayerType == ActivePlayerType.Attacker) {
                Debug.Log ("OK'd place attacking units prompt at " + m_battleArea);

                // Make the Defender's Buttons non-interactable
                for (int i = 0; i < defenceButtons.Length; i++) {
                    defenceButtons [i].interactable = false;
                }
                // If the attacker is human, make the Attacker's Buttons interactable
                if (m_attacker.IsHuman == true) {
                    for (int i = 0; i < attackButtons.Length; i++) {
                        attackButtons [i].interactable = true;
                    }
                }
                // Otherwise the attacker is AI, so make the buttons non-interactable
                else {
                    for (int i = 0; i < attackButtons.Length; i++) {
                        attackButtons [i].interactable = false;
                    }
                    for (int i = 0; i < attackPanelNavButtons.Length; i++) {
                        attackPanelNavButtons [i].interactable = false;
                    }
                }
            }
            // If the active player is the defender
            else {
                Debug.Log ("OK'd place defending units prompt at " + m_battleArea);

                // Make the Attacker's Buttons non-interactable
                for (int i = 0; i < attackButtons.Length; i++) {
                    attackButtons [i].interactable = false;
                }
                // If the Defender is human, make the Defender's Buttons interactable
                if (m_defender.IsHuman == true) {
                    for (int i = 0; i < attackButtons.Length; i++) {
                        defenceButtons [i].interactable = true;
                    }
                }
                // Otherwise the defender is AI, so make the buttons non-interactable
                else {
                    for (int i = 0; i < attackButtons.Length; i++) {
                        defenceButtons [i].interactable = false;
                    }
                    for (int i = 0; i < defencePanelNavButtons.Length; i++) {
                        defencePanelNavButtons [i].interactable = false;
                    }
                }
            }
        }



        public void SwitchActivePlayerType ()
        {
            // If the active player is the attacker
            if (m_activePlayerType == ActivePlayerType.Attacker) {

                // change to defender
                m_activePlayerType = ActivePlayerType.Defender;
                // if the defender is human, turn the sund effects back on
                if ((m_defender != null) && (m_defender.IsHuman)) {
                    m_conquestMapManagerRef.SetMuteAudio (false);
                }
            }
            // If the active player is the defender
            else {
                // change to attacker
                m_activePlayerType = ActivePlayerType.Attacker;

            }
            Debug.Log ("Switched active player to " + m_activePlayerType);
        }
	
        // Sets the quantities next to each unit type in the unit menu
        public void SetUnitMenuQuantities ()
        {
            // If the active player is the attacker
            if (m_activePlayerType == ActivePlayerType.Attacker) {

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
            // If the active player is the defender
            else {
                // Count the defender's army units 
                m_battleArea.CountOccupyingUnits ();
                // Update the UI text in respect of the counted units
                // (Using only the first half of the collection since siege engines will never be defending an area)
                for (int i = 0; i < unitMenuQuantities.Length / 2; i++) {
                    unitMenuQuantities [i].text = m_battleArea.myOccupyingUnitTotals [i].ToString ();
                    // If the defender has run out of a unit type
                    if (m_battleArea.myOccupyingUnitTotals [i] == 0) {
                        // Turn off the button for that unit type
                        unitMenuButtons [i].interactable = false;
                    } else 
                        unitMenuButtons [i].interactable = true;
                }
            }
        }

        // Enables the attacker or defender buttons dependent on active player
        public void ExitUnitMenuPanel ()
        {
            // If the active player is the attacker
            if (m_activePlayerType == ActivePlayerType.Attacker) {
                // Make all attack buttons interactable
                for (int i =0; i< attackButtons.Length; i++) {
                    attackButtons [i].interactable = true;
                }
                // If the active player is not the attacker
            } else {
                // Make all the defence buttons interactable
                for (int i =0; i< defenceButtons.Length; i++) {
                    defenceButtons [i].interactable = true;
                }
            }
        }

        // Clear the attacker's panel
        // Allows the player to 'uncommit' all assigned units from the combat
        public void ClearCombatPanel ()
        {
            // If the Attacker is the active player
            if (m_activePlayerType == ActivePlayerType.Attacker) {
                // Check the attacking army
                for (int i = 0; i < attackingArmy.Length; i++) {
                    // If any army slot is assigned
                    if (attackingArmy [i] != null) {
                        // add the currently assigned one back to the player's army list
                        m_attacker.AddToArmyList (attackingArmy [i]);
                        // remove it from the attacking army
                        attackingArmy [i] = null;
                        // reset the corresponding button text 
                        attackButtonText [i].text = "Attacker";
                        // reset the menu quantities
                        SetUnitMenuQuantities ();
                    }
                }
            }
            // Otherwise the Defender is the active player
            else {
                // Check the defending army
                for (int i = 0; i < defendingArmy.Length; i++) {
                    // If any army slot is assigned
                    if (defendingArmy [i] != null) {
                        // add the currently assigned one back to the battle area occupying unit aray
                        m_battleArea.AssignOccupyingUnit (defendingArmy [i], false);
                        // Remove it from the defending army
                        defendingArmy [i] = null;
                        // reset the corresponding button text
                        defenceButtonText [i].text = "Defender";
                        // reset the menu quantities
                        SetUnitMenuQuantities ();
                    }
                }
            }
        }

        // Assigns a unit of a certain type to the attacking or defending side.
        // Removes the unit from the attacker/defender's army list
        public void AssignUnitToBattle (string UnitType)
        {
            switch (UnitType) {
                case "Spearmen":
                {
                    // If the Attacker is the active player
                    if (m_activePlayerType == ActivePlayerType.Attacker) {

                        // Find the active attack button
                        for (int i = 0; i < attackButtons.Length; i++) {
                            if (attackButtons [i].interactable == true) {

                                // if the active attack button has already been assigned
                                if (attackingArmy [i] != null) {
                                    // add the currently assigned one back to the player's army list
                                    m_attacker.AddToArmyList (attackingArmy [i]);
                                    // remove it from the attacking army
                                    attackingArmy [i] = null;
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
                                // Set the button's text and reset the unit menu 
                                attackButtonText [i].text = "Spearmen";

                                SetUnitMenuQuantities ();
                            }
                        }
                    }
                    // Otherwise the Defender is the active player
                    else {
                        // Find the active defence button
                        for (int i = 0; i < defenceButtons.Length; i++) {
                            if (defenceButtons [i].interactable == true) {
						
                                // if the active attack button has already been assigned
                                if (defendingArmy [i] != null) {
                                    // add the currently assigned one back to the battle area's occupying units array
                                    m_battleArea.AssignOccupyingUnit (defendingArmy [i], false);
                                    // remove it from the defending army
                                    defendingArmy [i] = null;
                                }
	
                                // add a spearman unit to the defending army
                                // subtract a spearmen from the battle area's occupying units array
                                for (int j = 0; j < m_battleArea.occupyingUnits.Length; j++) {
                                    if (m_battleArea.occupyingUnits [j] != null) {
                                        if (m_battleArea.occupyingUnits [j].myMilitaryType == CardMilitary.MilitaryType.Spearmen) {
                                            defendingArmy [i] = m_battleArea.occupyingUnits [j];
                                            m_battleArea.occupyingUnits [j] = null;
                                            break;
                                        }
                                    }
                                }
                                // Set the button's text and reset the unit menu 
                                defenceButtonText [i].text = "Spearmen";
                                SetUnitMenuQuantities ();
                            }
                        }
                    }
                    break;
                }
                case "Archers":
                {
                    // If the Attacker is the active player
                    if (m_activePlayerType == ActivePlayerType.Attacker) {

                        // Find the active attack button
                        for (int i = 0; i < attackButtons.Length; i++) {
                            if (attackButtons [i].interactable == true) {

                                // if the active attack button has already been assigned
                                if (attackingArmy [i] != null) {
                                    // add the currently assigned one back to the player's army list
                                    m_attacker.AddToArmyList (attackingArmy [i]);
                                    // remove it from the attacking army
                                    attackingArmy [i] = null;
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
                                // Set the button's text and reset the unit menu 
                                attackButtonText [i].text = "Archers";

                                SetUnitMenuQuantities ();
                            }
                        }
                    }
                    // Otherwise the Defender is the active player
                    else {
                        // Find the active defence button
                        for (int i = 0; i < defenceButtons.Length; i++) {
                            if (defenceButtons [i].interactable == true) {
						
                                // if the active attack button has already been assigned
                                if (defendingArmy [i] != null) {
                                    // add the currently assigned one back to the battle area's occupying units array
                                    m_battleArea.AssignOccupyingUnit (defendingArmy [i], false);
                                    // remove it from the defending army
                                    defendingArmy [i] = null;
                                }
						
                                // add a an archers unit to the defending army
                                // subtract an archers from the battle area's occupying units array
                                for (int j = 0; j < m_battleArea.occupyingUnits.Length; j++) {
                                    if (m_battleArea.occupyingUnits [j] != null) {
                                        if (m_battleArea.occupyingUnits [j].myMilitaryType == CardMilitary.MilitaryType.Archers) {
                                            defendingArmy [i] = m_battleArea.occupyingUnits [j];
                                            m_battleArea.occupyingUnits [j] = null;
                                            break;
                                        }
                                    }
                                }
                                // Set the button's text and reset the unit menu 
                                defenceButtonText [i].text = "Archers";
                                SetUnitMenuQuantities ();
                            }
                        }

                    }

                    break;
                }
                case "Knights":
                {
                    // If the Attacker is the active player
                    if (m_activePlayerType == ActivePlayerType.Attacker) {

                        // Find the active attack button
                        for (int i = 0; i < attackButtons.Length; i++) {
                            if (attackButtons [i].interactable == true) {

                                // if the active attack button has already been assigned
                                if (attackingArmy [i] != null) {
                                    // add the currently assigned one back to the player's army list
                                    m_attacker.AddToArmyList (attackingArmy [i]);
                                    // remove it from the attacking army
                                    attackingArmy [i] = null;
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
                                // Set the button's text and reset the unit menu 

                                attackButtonText [i].text = "Knights";
					
                                SetUnitMenuQuantities ();

                            }
                        }
                    }
                    // Otherwise the Defender is the active player 
                    else {
                        // Find the active defence button
                        for (int i = 0; i < defenceButtons.Length; i++) {
                            if (defenceButtons [i].interactable == true) {
						
                                // if the active attack button has already been assigned
                                if (defendingArmy [i] != null) {
                                    // add the currently assigned one back to the battle area's occupying units array
                                    m_battleArea.AssignOccupyingUnit (defendingArmy [i], false);
                                    // remove it from the defending army
                                    defendingArmy [i] = null;
                                }
						
                                // add a knights unit to the defending army
                                // subtract a knights unit from the battle area's occupying units array
                                for (int j = 0; j < m_battleArea.occupyingUnits.Length; j++) {
                                    if (m_battleArea.occupyingUnits [j] != null) {
                                        if (m_battleArea.occupyingUnits [j].myMilitaryType == CardMilitary.MilitaryType.Knights) {
                                            defendingArmy [i] = m_battleArea.occupyingUnits [j];
                                            m_battleArea.occupyingUnits [j] = null;
                                            break;
                                        }
                                    }
                                }
                                // Set the button's text and reset the unit menu 
                                defenceButtonText [i].text = "Knights";
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
                        if (attackButtons [i].interactable == true) {
						
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            }
						
                            // add a Ram unit to the attacking army at the selected point
                            // subtract a Ram unit from the attacker's army list
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Ram) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            // Set the button's text and reset the unit menu 
						
                            attackButtonText [i].text = "Ram";

                            SetUnitMenuQuantities ();
						
                        }
                    }

                    break;
                }
                case "Belfry":
                {

				
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        if (attackButtons [i].interactable == true) {
						
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            }
						
                            // add a Belfry unit to the attacking army at the selected point
                            // subtract a Belfry unit from the attacker's army list
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Belfry) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            // Set the button's text and reset the unit menu 
                            attackButtonText [i].text = "Tower";

                            SetUnitMenuQuantities ();
						
                        }
                    }

                    break;
                }

                case "Artillery":
                {
                    // Find the active attack button
                    for (int i = 0; i < attackButtons.Length; i++) {
                        if (attackButtons [i].interactable == true) {
						
                            // if the active attack button has already been assigned
                            if (attackingArmy [i] != null) {
                                // add the currently assigned one back to the player's army list
                                m_attacker.AddToArmyList (attackingArmy [i]);
                                // remove it from the attacking army
                                attackingArmy [i] = null;
                            }
						
                            // add a artillery unit to the attacking army at the selected point
                            // subtract a artillery unit from the attacker's army list
                            for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                                if (m_attacker.myArmy [j].myMilitaryType == CardMilitary.MilitaryType.Artillery) {
                                    attackingArmy [i] = m_attacker.myArmy [j];
                                    m_attacker.myArmy.RemoveAt (j);
                                    break;
                                }
                            }
                            // Set the button's text and reset the unit menu 
						
                            attackButtonText [i].text = "Artillery";
				
                            SetUnitMenuQuantities ();
                        }
                    }
                    break;
                }


            }
        }

        // Called after selecting a unit from the unit menu.
        // Makes the Attacker/Defender's buttons interactable
        public void MakeButtonsInteractable ()
        {
            // If the active player is the attacker
            if (m_activePlayerType == ActivePlayerType.Attacker) {
                // Make all the attack buttons interactable
                for (int i = 0; i < attackButtons.Length; i ++) {
                    attackButtons [i].interactable = true;
                }
                // Make the clear panel navigation button interactable
                attackPanelNavButtons [0].interactable = true;
                // If the attacking army is not empty
                if ((attackingArmy [0] != null) || (attackingArmy [1] != null) || (attackingArmy [2] != null))
                    // Make the ready button interactable
                    attackPanelNavButtons [1].interactable = true;
                // Otherwise it should not be interactable
                else
                    attackPanelNavButtons [1].interactable = false;
            }
            // If the active player is the defender
            else {
                // Make all the defence buttons interactable
                for (int i = 0; i < defenceButtons.Length; i++) {
                    defenceButtons [i].interactable = true;
                }
                // Make the defence clear panel button active
                defencePanelNavButtons [0].interactable = true;
                // If there are no units left in the battle area make the Ready for battle buttons active
                if ((m_battleArea.occupyingUnits [0] == null) && (m_battleArea.occupyingUnits [1] == null) && (m_battleArea.occupyingUnits [2] == null)) {
                    defencePanelNavButtons [1].interactable = true;
                }
            }
        }

        // Make the Atacker's cards show the default card text
        public void AnonymiseCards ()
        {
            // Make all the Attacker's cards read "Attacker"
            for (int i = 0; i < attackButtonText.Length; i++) {
                attackButtonText [i].text = "Attacker";
            }
            // Make all the Defender's cards read "Defender"
            for (int i = 0; i < defenceButtonText.Length; i++) {
                defenceButtonText [i].text = "Defender";
            }
        }

        // Check to see if the attacker is unopposed
        public void CheckUnopposed ()
        {

            // If there is no defender and the area is empty
            if ((m_defender == null) && (!m_battleArea.AreaContainsUnits())) {
                Debug.Log ("No Defender at " + m_battleArea);
                // The attacker wins
                ApplyVictory (m_attacker);

                // otherwise there is a defender
            } else {
                // If the defender is an AI player, or no defender
                if ((m_defender == null) || (!m_defender.IsHuman)) {
                    if ((m_defender != null) && (!m_defender.IsHuman))
                        Debug.Log ("Defender at " + m_battleArea + " is AI.");
                    else
                    {Debug.Log ("No defender but there are units here.");}
                    // Set AI defence
                    SetAIDefence ();
                }
                // otherwise the defender is human
                else {
                    Debug.Log ("Defender at " + m_battleArea + " is human.");
                    // Show the Defender's place units prompt
                    DefencePromptSetup ();
                }
            }
        }

        // Set up the prompt to place defending units
        void DefencePromptSetup ()
        {
            defencePlaceUnitsPrompt.SetActive (true);
            defencePromptText [0].text = "" + m_defender.MyFaction.name + " (" + m_defender.name + "),";
            defencePromptText [2].text = "" + m_attacker.MyFaction.name + " (" + m_attacker.name + ")";
            defencePromptText [3].text = "in " + m_battleArea.name + ".";
        }
	
        // Auto-set an attacking army
        public void SetAIAttack ()
        {
            Debug.Log ("Setting AI attack army for " + m_attacker.name + " at " + m_battleArea);
            // If fighting an empty area
            // Send out a minimal weak army
            if (m_battleArea.ControllingPlayer == null) {
                SetUnitMenuQuantities ();
                // Check soldier unit totals
                // (Performing the check in this way ensures that the weakest units are placed first)
                for (int i = 0; i < (m_attacker.myArmyUnitTotals.Length / 2); i++)
                    // Pick the first type that has a non-zero total
                    if (m_attacker.myArmyUnitTotals [i] > 0) {
                        // find the first unit of that type in the attacker's army
                        for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                            if (m_attacker.myArmy [j].myMilitaryType == m_gameDataRef.Soldiers [i].myMilitaryType) {
                                // Assign the unit to a random attack slot, and remove it from the attacker's army
                                attackingArmy [Random.Range (0, 2)] = m_attacker.myArmy [j];
                                m_attacker.myArmy.RemoveAt (j);
                                break;
                            }
                        }
                        break;
                    }
            } else {
                // Set a standard AI Deployment array
                // (AI Deployment is the order in which the units are placed) 
                int[] AIDeployment = {0, 1, 2};
                // Shuffle the deployment
                ShuffleInt (AIDeployment);

                SetUnitMenuQuantities ();

                for (int i = 0; i < AIDeployment.Length; i++) {
                    for (int j = 0; j < m_attacker.myArmy.Count; j++) {
                        if (m_attacker.myArmy [j].myMilitaryType == m_gameDataRef.Soldiers [2 - i].myMilitaryType) {
                            attackingArmy [AIDeployment [i]] = m_attacker.myArmy [j];
                            m_attacker.myArmy.RemoveAt (j);
                            break;
                        }
                        if (j == m_attacker.myArmy.Count - 1) {
                            if ((m_attacker.myArmy [j].myMilitaryType != CardMilitary.MilitaryType.Ram) && (m_attacker.myArmy [j].myMilitaryType != CardMilitary.MilitaryType.Belfry) &&  (m_attacker.myArmy [j].myMilitaryType != CardMilitary.MilitaryType.Artillery))
                                attackingArmy [AIDeployment [i]] = m_attacker.myArmy [j];
                            m_attacker.myArmy.RemoveAt (j);
                            break;
                        }
                    }
                }
            }
        }


        // Auto-set the defending army
        public void SetAIDefence ()
        {
            // Set a standard AI Deployment array
            // (AI Deployment is the order in which the units are placed) 
            int[] AIDeployment = {0, 1, 2};
            // Shuffle the deployment
            ShuffleInt (AIDeployment);

            // Check the units which are assigned to the area in which the battle is taking place
            for (int i = 0; i < m_battleArea.occupyingUnits.Length; i++) {
                // If there is a unit defending the area at this index
                if (m_battleArea.occupyingUnits != null) {
                    // Assign it to the randomised defending army deployment index
                    defendingArmy [AIDeployment [i]] = m_battleArea.occupyingUnits [i];
                    m_battleArea.occupyingUnits [i] = null;
                }
            }
            // Resolve combat
            ResolveCombat ();
        }

        // Generic method for shuffling an array of integers
        // (Uses the Fisher-Yates algortihm)
        static void ShuffleInt (int[] array)
        {
            // Set the array length
            int n = array.Length;
            // Check each element in the array
            for (int i = 0; i < n; i++) {
                // Set r to a random number between 0 and array length
                int r = Random.Range (0, n);
                // Set t to the value of the array element r
                int t = array [r];
                // Set the array element r to the iterator number
                array [r] = array [i];
                // Set the current array element to t
                array [i] = t;
            }
        }

        // Calculate the result of the combat
        public void ResolveCombat ()
        {
            for (int i = 0; i < attackScores.Length; i++) {
                // If there is an attacking army in this slot
                if (attackingArmy [i] != null) {

                    if (defendingArmy [i] != null) {
                        // Calculate & set attack score
                        attackingArmy [i].CalculateCombatValue (CardMilitary.BattleType.Pitched, defendingArmy [i].myMilitaryType);
                        attackScores [i] = attackingArmy [i].combatScore;
                        // Calculate & set defence score
                        defendingArmy [i].CalculateCombatValue (CardMilitary.BattleType.Pitched, attackingArmy [i].myMilitaryType);
                        defenceScores [i] = defendingArmy [i].combatScore;
                    }

                    // If there is no opposing defending army
                    else {
                        // Calculate & set attack score versus "no" opponent
                        // (for the purposes of determining combat advantage the unit is treated as if being in combat with a unit of its own type)
                        attackingArmy [i].CalculateCombatValue (CardMilitary.BattleType.Pitched, attackingArmy [i].myMilitaryType);
                        attackScores [i] = attackingArmy [i].combatScore;
                        // Defence score is 0
                        defenceScores [i] = 0;
                    }
                }
                // If there is no attacking army
                else {
                    // attack score is 0
                    attackScores [i] = 0;
                    // If there is an opposing defending army
                    if (defendingArmy [i] != null) {
                        // Calculate & set defence score versus "no" opponent
                        // (for the purposes of determining combat advantage the unit is treated as if being in combat with a unit of its own type)
                        defendingArmy [i].CalculateCombatValue (CardMilitary.BattleType.Pitched, defendingArmy [i].myMilitaryType);
                        defenceScores [i] = defendingArmy [i].combatScore;
                    }
                    // If there is no defending army
                    else {
                        defenceScores [i] = 0;
                    }

                }
            }

            // Check if animations are turned on
            if (m_gameDataRef.BattleAnimationsEnabled) {
                // If animations are to be shown for this battle
                if (ShowAnimations ()) {
                    // Set up the animations & start animating
                    SetAnimations ();
                    m_pitchedAnim.StartAnimation();
                }
                // Otherwise move onto comparing the combat results
                else
                    CompareCombatResults ();
            } 
            // Otherwise move on to comparing the combat results
            else
                CompareCombatResults ();
        }


        public void CompareCombatResults()
        {
            // Local variables to count attack & defence victories
            int attackerVictories = 0;
            int defenderVictories = 0;

            // Compare all attack scores
            for (int i = 0; i < attackScores.Length; i++) {
                // Show combatants in debug
                //Debug.Log ("At" + i + ": " + attackingArmy [i].myMilitaryType + "(" + attackScores [i] + ") vs. " + defendingArmy [i].myMilitaryType + "(" + defenceScores [i] + ")");
                // If an attack score is greater than the corresponding defence score
                if (attackScores [i] > defenceScores [i]) {
				
				
                    // The defender is destroyed, the attacker wins
                    if (defendingArmy [i] != null) {
                        if (ShowAnimations()){
                            m_pitchedAnim.SetDefeatAnim("defender", i);
                        }
                        defendingArmy [i].DestroyUnit ();
                        defendingArmy [i] = null;
                    }
                    attackerVictories++;
                    // Show winner in debug
                    Debug.Log ("Attacker wins at " + i);
                } else {
                    // On any other result the attacker is destroyed, the defender wins
                    if (attackingArmy [i] != null) {
                        if (ShowAnimations()){
                            m_pitchedAnim.SetDefeatAnim("attacker", i);
                        }
                        attackingArmy [i].DestroyUnit ();
                        attackingArmy [i] = null;
                    }
                    defenderVictories++;
                    // Show winner in debug
                    Debug.Log ("Defender wins at " + i);
                }
            }
		
            // If the attacker has scored more victories than the defender
            Debug.Log ("Attack: " + attackerVictories + " versus Defence: " + defenderVictories);
            if (attackerVictories > defenderVictories) {
                // Apply the victory to the attacker
                ApplyVictory (m_attacker);
                Debug.Log ("Attacker wins the battle of " + m_battleArea.name);
            } else {
                // Otherwise victory goes to the defence
                ApplyVictory (m_defender);
                Debug.Log ("Defender wins the battle of " + m_battleArea.name);
            }
            attackerVictories = 0;
            defenderVictories = 0;

        }

        /// <summary>
        /// Checks if this is a battle for which to show animations.
        /// Returns true if animation is to be shown.
        /// </summary>
        /// <returns><c>true</c>, if animations was shown, <c>false</c> otherwise.</returns>
        bool ShowAnimations()
        {
            bool showAnim;
            showAnim = false;
            // If there is a defender
            if (m_defender != null) {
                // If either the defender or the attacker is human
                if ((m_attacker.IsHuman) || (m_defender.IsHuman)) {
                    showAnim = true;
                }
            } else {
                // if there is no defender, but the attacker is human and the defending area contains units
                if (((defendingArmy [0] != null) || (defendingArmy [1] != null) || (defendingArmy [2] != null)) && (m_attacker.IsHuman)) {
                    showAnim = true;
                }
            }

            if (showAnim) {
                Debug.Log ("Animations should show");
		
                return true;
            }
            else{
                Debug.Log("Animations should not show");
                return false;}
        }

        /// <summary>
        /// Sets the battle animations and their positions.
        /// </summary>
        public void SetAnimations()
        {
            //this assigns the animations to the right cards
            m_pitchedAnim.AssignAttackAnimations (); 
            //gives position to animations
            m_pitchedAnim.AssignAttackPositions (); 
            // Assign the defence animations & their positions
            m_pitchedAnim.AssignDefenceAnimations();
            m_pitchedAnim.AssignDefencePositions();

        }

        // Apply the result of the victory to both players
        public void ApplyVictory (Player victoriousPlayer)
        {
            // Deal with removing the losing player from the area:
            // If the attacker was victorious
            if (victoriousPlayer == m_attacker) {
                // If there was a defender
                if (m_defender != null) {
                    // Remove the area from the defender's empire
                    for (int i = 0; i < m_defender.myEmpire.Count; i++) {
                        if (m_defender.myEmpire [i].name == m_battleArea.name) {
                            m_defender.myEmpire.RemoveAt (i);
                            m_defender.LoseInfluence ();
                        }
                    }
                    // Return any defender's armies to their hand
                    for (int i = 0; i < defendingArmy.Length; i++) {
                        if (defendingArmy [i] != null) {
                            m_defender.AddToArmy (defendingArmy [i], false);
                            defendingArmy [i] = null;
                        }
                    }
                    // If the defending player was AI, let them know who defeated them
                    if (!m_defender.IsHuman) {
                        // Call method to set lastattackedby
                        m_defender.SetLastAttackedBy (m_attacker);
                    }
                }
                // If there was no defender,
                else {
                    //  destroy any remaining defending armies
                    for (int i = 0; i < defendingArmy.Length; i++) {
                        if (defendingArmy [i] != null) {
                            defendingArmy [i].DestroyUnit();
                            defendingArmy [i] = null;
                        }
                    }
                }

                // Set the victory panel
                victoryPanel.SetActive (true);
                victoryPanelText [0].text = "" + m_attacker.MyFaction.name + " (" + m_attacker.name + ")";
                victoryPanelText [1].text = "Has conquered";
                victoryPanelText [2].text = "" + m_battleArea.name;
            }
            // Otherwise assume the Defender was victorious
            else {
                // Return any surviving attacker's armies to their hand
                for (int i = 0; i < attackingArmy.Length; i++) {
                    if (attackingArmy [i] != null) {
                        // If the current unit's type is spearmen, archers or knights, 
                        if ((attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Spearmen) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Archers) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Knights))
                            // return it to the player
                            m_attacker.AddToArmyList (attackingArmy [i]);
                        // Otherwise destroy it (siege weapons are one-use only)
                        else {
                            attackingArmy [i].DestroyUnit ();
                        }
                        // 
                        attackingArmy [i] = null;

                    }
                }

                // Return the surviving defending armies to the area
                for (int i = 0; i < defendingArmy.Length; i++) {
                    if (defendingArmy [i] != null) {
                        for (int j = 0; j < m_battleArea.occupyingUnits.Length; j++) {
                            if (m_battleArea.occupyingUnits [j] == null) {
                                m_battleArea.occupyingUnits [j] = defendingArmy [i];
                                break;
                            }
                        }
                        defendingArmy [i] = null;
                    }
                }
                victoryPanel.SetActive (true);
                victoryPanelText [0].text = "" + m_attacker.MyFaction.name + " (" + m_attacker.name + ")";
                victoryPanelText [1].text = "Has failed to conquer";
                victoryPanelText [2].text = "" + m_battleArea.name;
            }


            // Deal with adding the winning player to the area:
            // If the attacker was victorious
            if (victoriousPlayer == m_attacker) {
                // Add the Area to the victor's empire
                victoriousPlayer.AddToEmpire (m_battleArea);
                // Assign any remaining units in the attacking army to the area
                for (int i = 0; i < attackingArmy.Length; i++) {
                    // Assign remaining units to the conquered area
                    if (attackingArmy [i] != null) {
                        // If the current unit's type is spearmen, archers or knights, 
                        if ((attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Spearmen) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Archers) || (attackingArmy [i].myMilitaryType == CardMilitary.MilitaryType.Knights))
                            // assign it to the area				
                            m_battleArea.AssignOccupyingUnit (attackingArmy [i], false);
                        // Otherwise destroy the unit
                        else 
                            attackingArmy [i].DestroyUnit ();
                        // Set the army slot to null
                        attackingArmy [i] = null;
                    }
                }
            } 
	
        }

        public void DisableSoundIfAI()
        {
            if (!m_stateManagerRef.PhasingPlayer.IsHuman) {
                m_conquestMapManagerRef.SetMuteAudio(true);
            }
        }

        // Clears both the attacking and defending army collections
        // Used when exiting the pitched battle to ensure these are blank for the next combat
        public void ClearBattleArmies ()
        {
            for (int i = 0; i < attackingArmy.Length; i++) {
                attackingArmy [i] = null;
            }

            for (int i = 0; i < defendingArmy.Length; i++) {
                defendingArmy [i] = null;
            }
        }


    }
}

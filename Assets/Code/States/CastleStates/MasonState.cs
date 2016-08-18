using Assets.Code.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.States.CastleStates
{
    public class MasonState : MonoBehaviour
    {
	
        // Game Data Reference
        private GameObject m_gameManager;
        private GameData m_gameDataRef;
		
        //Castle State & Military Cards
        private Castle m_castle;
        private CardMilitary m_CardMilitary;

        // State Manager Reference
        private StateManager m_stateManagerRef;
	
        // Map Reference
        public RectTransform mapImage;

        // Player Controlled Area Bool
        public bool playerControlled;
	
        // Buttons & Text Reference
        public Button[] areaButtons;
        public Text areaNameDefence, areaNameKeep;
        public GameObject[] purchasePanels;
        public Button keepButton;
        public Button[] defencesButtons;
        public Text[] defencesNumber;
        public Text[] defenceCost;
		
        // Area Reference
        private Area m_selectedArea;

        //Shield Referance
        public Image[] shieldImage;
        public GameObject[] keepInArea;
        public GameObject[] regionOwned;
        public Sprite shieldDefault;


        //Defence Counters
        int wallNumber;
        int towerNumber, oilNumber, ditchNumber;
	
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
            // Make all icons inactive
            for (int i = 0; i < keepInArea.Length; i++) {
                keepInArea [i].SetActive(false);
            }
            for (int i = 0; i < regionOwned.Length; i++) {
                regionOwned [i].SetActive(false);
            }
        }
		

		
        public void CheckMap ()
        {
            // Assign buttons to their areas
            for (int i = 0; i < m_gameDataRef.AllAreas.Length; i++) {
                m_gameDataRef.AllAreas [i].AssignMapButton (areaButtons [i]);
                // If Areas are controlled, set the button image to the controlling faction flag
                if (m_gameDataRef.AllAreas [i].ControllingPlayer != null) {
                    shieldImage[i].sprite = m_gameDataRef.AllAreas [i].ControllingPlayer.MyFaction.MyFlagSprite;
                }
            }
			
				
		
            // Make all buttons non-interactable
            for (int i = 0; i < areaButtons.Length; i++) {
                areaButtons [i].interactable = false;
						
            }
            CheckAreaKeep ();
				
				
            // Set the buttons for all areas in my empire (and their neighbours) to interactable if they contain a castle
            // Check all Areas in phasingplayer empire
            for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
                // if the area contains a castle
                if (m_stateManagerRef.PhasingPlayer.myEmpire [i].occupyingCastle != null) {
                    // Set the area's button to be interactable
                    m_stateManagerRef.PhasingPlayer.myEmpire [i].MyMapButton.interactable = true;
                }
			
			
                // Now check areas which area eligible for castle building and set their buttons to be interactable
                // Castles can only be built in an area in which the controlling player controls all the area's neighbours
                // and which is not adjacent to another castle.

                // Set player controlled to true
                // (i.e. the player controls all neighbours of this area)
                playerControlled = true;
                // Check all neighbour areas of empire areas
                for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
                    // if the checked area's neighbour is not controlled by the phaisng player
                    if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer) {
                        {
                            // player controlled is false
                            playerControlled = false;
                        }
                    }
                }
                // If the player controls all neighbours and the area is not adjacent to another castle
                if ((playerControlled) && (!AdjacentToAnyCastle (m_stateManagerRef.PhasingPlayer.myEmpire [i]))) {
                    // Set this button to be interactable
                    m_stateManagerRef.PhasingPlayer.myEmpire [i].MyMapButton.interactable = true;
                }
            }
        }

        bool AdjacentToAnyCastle (Area areaCheck)
        {
            // Declaring local variable, default is "false"
            bool adjacentToACastle;
            adjacentToACastle = false;
            // Check all areas which are neighbours of the checked area
            for (int i = 0; i < areaCheck.myNeighbours.Length; i++) {
                // If the checked neighbour contains a castle, and this player is not the phasing player
                if ((areaCheck.myNeighbours [i].occupyingCastle != null)) {
                    // The area is adjacent to a castle, break from the loop
                    adjacentToACastle = true;
                    break;
                }
            }
		
            if (adjacentToACastle)
                return true;
            else
                return false;
        }
	
        // When an area button is clicked
        public void AreaButtonClicked (int ButtonIndex)
        {
            m_selectedArea = m_gameDataRef.AllAreas [ButtonIndex];
            Debug.Log ("Build mode" + m_selectedArea.name + " has been clicked");

            if (m_selectedArea.occupyingCastle != null) {
                purchasePanels [2].SetActive (false);
                purchasePanels [1].SetActive (true);
                purchasePanels [0].SetActive (false);						
                areaNameKeep.text = m_selectedArea.name;
                areaNameDefence.text = m_selectedArea.name;
                CheckAreaDefences ();
					


            } else if (m_selectedArea.occupyingCastle == null) {
                purchasePanels [2].SetActive (false);
                purchasePanels [1].SetActive (false);
                purchasePanels [0].SetActive (true);
                areaNameDefence.text = m_selectedArea.name;
                areaNameKeep.text = m_selectedArea.name;

            }
            CheckMap ();
				
        }
	
        //Add Keep and remove corresponding cost from Player
        public void PurchaseKeep ()
        {
            m_stateManagerRef.PhasingPlayer.AddNewCastle (m_gameDataRef.castle, m_gameDataRef.Defences [2]);
            m_selectedArea.AssignCastle (m_stateManagerRef.PhasingPlayer.myCastles [m_stateManagerRef.PhasingPlayer.myCastles.Count - 1]);
            m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Defences [2].Cost);

            //Update UI
            m_stateManagerRef.uiManagerRef.UpdateUI ();
            // Check the map to update interactable buttons
            CheckMap ();
            //Check Costs
            CheckKeepCost ();
            CheckDefenceCost ();

            //Decide which Panel to show to player
            if (m_selectedArea.occupyingCastle != null) {
                purchasePanels [2].SetActive (false);
                purchasePanels [1].SetActive (true);
                purchasePanels [0].SetActive (false);						
                areaNameKeep.text = m_selectedArea.name;
                areaNameDefence.text = m_selectedArea.name;
                CheckKeepCost ();
                CheckDefenceCost ();
            } else if (m_selectedArea.occupyingCastle == null) {
                purchasePanels [2].SetActive (false);
                purchasePanels [1].SetActive (false);
                purchasePanels [0].SetActive (true);
                areaNameDefence.text = m_selectedArea.name;
                areaNameKeep.text = m_selectedArea.name;				
                CheckKeepCost ();
                CheckDefenceCost ();
            } else
                purchasePanels [2].SetActive (true);
            Debug.Log ("You have " + m_stateManagerRef.PhasingPlayer.Gold + " Gold.");
				
        }

        //For use with the AI player (as the area needs to be specified)
        //Add Keep and remove corresponding cost from Player
        public void AIPurchaseKeep (Area keepArea)
        {
            // Add a new castle with keep to the phasing player
            m_stateManagerRef.PhasingPlayer.AddNewCastle (m_gameDataRef.castle, m_gameDataRef.Defences [2]);
            // Assign the new castle the specified area
            keepArea.AssignCastle (m_stateManagerRef.PhasingPlayer.myCastles [m_stateManagerRef.PhasingPlayer.myCastles.Count - 1]);
            // Remove th ecost of the keep from the phasing player.
            m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Defences [2].Cost);
            Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " has a new castle keep at " + keepArea.name);
            Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " has " + m_stateManagerRef.PhasingPlayer.Gold + " Gold.");
        }

        //For use with the AI player (as the area needs to be specified)
        //Add selected Defence and remove corresponding cost from Player
        public void AIPurchaseDefence (Area DefenceArea, int DefenceNumber)
        {
            // Build the defence at the specified area
            DefenceArea.occupyingCastle.AddDefence (m_gameDataRef.Defences [DefenceNumber]);
            // Remove the cost from the player's gold
            m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Defences [DefenceNumber].m_cost);
            Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " purchased a " + m_gameDataRef.Defences [DefenceNumber] + " at " + DefenceArea.name);
            Debug.Log ("You have " + m_stateManagerRef.PhasingPlayer.Gold + " Gold.");
        }

        //Add selected Defence and remove corresponding cost from Player
        public void PurchaseWall (int DefenceNumber)
        {
			
            m_selectedArea.occupyingCastle.AddDefence (m_gameDataRef.Defences [DefenceNumber]);
            m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Defences [DefenceNumber].m_cost);
            m_selectedArea.occupyingCastle.castleDefences [m_selectedArea.occupyingCastle.castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.Wall);
            //Update UI
            m_stateManagerRef.uiManagerRef.UpdateUI ();

            //Check Costs & update Defence number
            CheckKeepCost ();
            CheckDefenceCost ();
            CheckAreaDefences ();

            Debug.Log ("You have purchased a " + m_gameDataRef.Defences [DefenceNumber] + " and it costed " + m_gameDataRef.Defences [DefenceNumber].m_cost + " Gold.");
            Debug.Log ("You have " + m_stateManagerRef.PhasingPlayer.Gold + " Gold.");
        }
		
        //Add selected Defence and remove corresponding cost from Player
        public void PurchaseDefence (int defenceType)
        {
            if (defenceType == 0) {
                m_selectedArea.occupyingCastle.AddDefence (m_gameDataRef.Defences [0]);
                m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Defences [0].m_cost);
                m_selectedArea.occupyingCastle.castleDefences [m_selectedArea.occupyingCastle.castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.WatchTower);
                Debug.Log ("You have bought a Watch Tower");
            }
            if (defenceType == 1) {
                m_selectedArea.occupyingCastle.AddDefence (m_gameDataRef.Defences [0]);
                m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Defences [0].m_cost);
                m_selectedArea.occupyingCastle.castleDefences [m_selectedArea.occupyingCastle.castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.Ditch);
                Debug.Log ("You have bought a Ditch");
            }
            if (defenceType == 2) {
                m_selectedArea.occupyingCastle.AddDefence (m_gameDataRef.Defences [0]);
                m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Defences [0].m_cost);
                m_selectedArea.occupyingCastle.castleDefences [m_selectedArea.occupyingCastle.castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.BoilingOil);
                Debug.Log ("You have bought a Boiling Oil");
            }
            //Update UI
            m_stateManagerRef.uiManagerRef.UpdateUI ();
			
            //Check Costs & update Defence number
            CheckKeepCost ();
            CheckDefenceCost ();
            CheckAreaDefences ();
			

            Debug.Log ("You have " + m_stateManagerRef.PhasingPlayer.Gold + " Gold.");
        }
			

        //Check if the player can afford the Keep. If not it disables the button
        public void CheckKeepCost ()
        {
				

            if (m_gameDataRef.Defences [2].Cost > m_stateManagerRef.PhasingPlayer.Gold) {
                keepButton.interactable = false;
                Debug.Log ("You cannot afford " + m_gameDataRef.Defences [2].name);
            } else
                keepButton.interactable = true;
				
        }

        //Check if the player can afford the defences. If not it disables the buttona
        public void CheckDefenceCost ()
        {
				
            for (int i = 0; i < defencesButtons.Length; i++) {
                {
                    if (m_stateManagerRef.PhasingPlayer.Gold < m_gameDataRef.Defences [1].Cost) {
                        defencesButtons [0].interactable = false;
                        Debug.Log ("You cannot afford " + m_gameDataRef.Defences [1].name);
                    } else
                        defencesButtons [0].interactable = true;
                    if (m_stateManagerRef.PhasingPlayer.Gold < m_gameDataRef.Defences [0].Cost) {
                        defencesButtons [i].interactable = false;
                        Debug.Log ("You cannot afford " + m_gameDataRef.Defences [0].name);
                    } else
                        defencesButtons [i].interactable = true;
                }
            }
			
        }
        //Shows the number of defences to a player
        public void CheckAreaDefences ()
        {
            wallNumber = 0;
            towerNumber = 0;
            ditchNumber = 0;
            oilNumber = 0;
				
            for (int i = 0; i < m_selectedArea.occupyingCastle.castleDefences.Count; i++) {
                if (m_selectedArea.occupyingCastle.castleDefences [i].myMilitaryType == m_gameDataRef.Defences [1].myMilitaryType) {

                    wallNumber += 1;
                }
                if ((m_selectedArea.occupyingCastle.castleDefences [i].myDefenceType == CardMilitary.DefenceType.WatchTower)) {
                    towerNumber += 1;			
                }
                if ((m_selectedArea.occupyingCastle.castleDefences [i].myDefenceType == CardMilitary.DefenceType.Ditch)) {
                    ditchNumber += 1;			
                }
                if ((m_selectedArea.occupyingCastle.castleDefences [i].myDefenceType == CardMilitary.DefenceType.BoilingOil)) {
                    oilNumber += 1;			
                }

            }
            defencesNumber [0].text = wallNumber.ToString ();
            defencesNumber [1].text = towerNumber.ToString ();
            defencesNumber [2].text = ditchNumber.ToString ();
            defencesNumber [3].text = oilNumber.ToString ();
            Debug.Log (m_selectedArea + " has " + wallNumber + " Walls.");
            Debug.Log (m_selectedArea + " has " + towerNumber + " Watch Towers.");
            Debug.Log (m_selectedArea + " has " + ditchNumber + " ditches.");
            Debug.Log (m_selectedArea + " has " + oilNumber + " Boiling Oil instalations.");
        }
        //Check which areas have a keep and assign the apropriate icon
        private void CheckAreaKeep(){
            for (int i = 0; i < m_gameDataRef.AllAreas.Length; i++) {
                if (m_gameDataRef.AllAreas[i].occupyingCastle != null)
                    keepInArea[i].SetActive(true);
            }
        }


    }
}

	
	

	


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Assets.Code.Interfaces;
using Assets.Code.Scripts;
using Assets.Code.States.CastleStates;

namespace Assets.Code.States
{
	// This inherits from IStateBase
	public class Play_CastleState : IStateBase
	{

		// Game Data Reference
		private GameObject m_gameManager;

		// VARIABLES
		// State m_stateManagerRef
		private StateManager m_stateManagerRef;

		// Castle Scene References 
		private GameObject m_castleManager;
		private CastleManager m_castleManagerRef;
		private TreasuryState m_treasuryStateRef;
		private BarracksState m_barracksStateRef;
		private MasonState m_masonStateRef;

		// AI References
		private bool m_AIPlaying;
		private int m_siegeEngineMax = 6;
		private int m_aiSavedGoldForCourt = 3;
		//private int m_soldierMax = 10;
		private Area m_castleThreatenedArea = null;
		private List<Area> m_adjacentAreas;

		//Music Manager Reference
		private MusicManager m_musicManagerRef;

		// Timers
		private int m_loadCountDown;

		// CONSTRUCTOR
		public Play_CastleState (StateManager managerRef)
		{
			// Set up reference to Game Manager
			m_gameManager = GameObject.Find ("GameManager");
			// Set the State m_stateManagerRef
			m_stateManagerRef = managerRef;
			//Set up Music Manager
			m_musicManagerRef = m_gameManager.GetComponent<MusicManager> ();
			// If the current scene is not scene 1, load scene 1
			if (Application.loadedLevelName != "Castle") {
				Application.LoadLevel ("Castle");
				m_loadCountDown = 10;
				Debug.Log ("Switched to Castle Scene");
			}


			// If the game has not started, start the first turn
			if (m_stateManagerRef.GameTurn == 0) {
				m_stateManagerRef.StartFirstTurn ();
			} 
			// Otherwise begin a new player turn
			else {
				m_stateManagerRef.NewPlayerTurn ();
			}
			 



			if (m_stateManagerRef.PhasingPlayer.IsHuman) {
				m_stateManagerRef.uiManagerRef.aiPlayingPanel.SetActive (false);

			} else {
				m_stateManagerRef.uiManagerRef.aiPlayingPanel.SetActive (true);
				m_stateManagerRef.uiManagerRef.aiPlayingText.text = "" + m_stateManagerRef.PhasingPlayer.MyFaction.name + 
					" (" + m_stateManagerRef.PhasingPlayer.name + ")";
			}
			m_stateManagerRef.ActivateUI ();
			//Activate UI for human players
			if (m_stateManagerRef.PhasingPlayer.IsHuman){
				
				m_stateManagerRef.uiManagerRef.uiBar.SetActive (true);

			}
			else{
				m_stateManagerRef.uiManagerRef.uiBar.SetActive (false);

			}
			//Check if Regions are correct
			m_stateManagerRef.PhasingPlayer.CheckRegions ();
			m_adjacentAreas = new List<Area> ();

			if (m_stateManagerRef.PhasingPlayer.IsHuman == true) {
						m_musicManagerRef.PauseopeningMusic ();
						m_musicManagerRef.PauseConquestMusic();
						m_musicManagerRef.PlayCastleAndCourtMusic ();
						m_musicManagerRef.PlayCastleAmbience ();
				} else {
						m_musicManagerRef.PauseCastleAndCourtMusic ();
						m_musicManagerRef.PauseCastleAmbience ();
				}
		}

		/// <summary>
		/// Update this state.
		/// </summary>
		public void StateUpdate ()
		{
			// Delay assignment of castle m_stateManagerRef to allow scene to load
			if (m_loadCountDown > 0) {
				m_loadCountDown--;
			}
			// Once the loadcountdown reaches 0, if castle m_stateManagerRef has not yet been assigned
			if ((m_loadCountDown == 0) && (m_castleManager == null)) {
				// Assign castle m_stateManagerRef
				m_castleManager = GameObject.Find ("CastleManager");
				Debug.Log ("Castle m_stateManagerRef is " + m_castleManager.name);
				// Assign castle m_stateManagerRef ref via castle m_stateManagerRef
				m_castleManagerRef = m_castleManager.GetComponent<CastleManager> ();
				Debug.Log ("Castle m_stateManagerRef Ref is " + m_castleManagerRef.name);
				// Assign treasury ref via castle m_stateManagerRef
				m_treasuryStateRef = m_castleManager.GetComponent<TreasuryState> ();
				Debug.Log ("Treasury ref is: " + m_treasuryStateRef.name);
				// Assign barracks ref via castle m_stateManagerRef
				m_barracksStateRef = m_castleManager.GetComponent<BarracksState> ();
				Debug.Log ("Barracks ref is: " + m_barracksStateRef.name);
				// Assign masons ref via castle m_stateManagerRef
				m_masonStateRef = m_castleManager.GetComponent<MasonState> ();
				Debug.Log ("Masons ref is: " + m_masonStateRef.name);

				// Deactivate the AIplayingMask if the phasing player is human
				if (m_stateManagerRef.PhasingPlayer.IsHuman)
					m_stateManagerRef.uiManagerRef.aiPlayingMask.SetActive (false);

			}


			//Debug.Log("Screen mask is active? "+m_stateManagerRef.uiManagerRef.AIPlayingMask.activeInHierarchy);

			if (m_castleManager != null) {
				// If the phasing player is AI
				if (!m_stateManagerRef.PhasingPlayer.IsHuman) {
					// If the AI is not currently playing
					if (!m_AIPlaying) {
						// Set it playing
						m_AIPlaying = true;
						// Collect Taxes (if they have not yet been collected)
//						if (!m_treasuryStateRef.taxButtonPressed) {
//							m_treasuryStateRef.CollectTax ();
//							m_treasuryStateRef.taxButtonPressed = true;
//						}
						// Check what to buy
						AICheckWhatToBuy ();
					}
				}
			}
		}

		/// <summary>
		/// Performs a series of checks that allows the AI to decide what to buy.
		/// </summary>
		public void AICheckWhatToBuy ()
		{
			// The cheapest unit is spearmen.
			// If the AI player can't afford spearmen, end the phase 
			if(m_stateManagerRef.PhasingPlayer.aiPriority == Player.AIPriority.Military)
			{
				if (!AIHasEnoughGold ("Spearmen")) {
					Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " can't afford any more units");
					EndAIPhase ();
					return;
				}
			}
			if ((m_stateManagerRef.PhasingPlayer.aiPriority == Player.AIPriority.Religion) || (m_stateManagerRef.PhasingPlayer.aiPriority == Player.AIPriority.Popularity)) 
			{
				if (m_stateManagerRef.PhasingPlayer.Gold <= m_aiSavedGoldForCourt)
				{
					EndAIPhase();
					Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " is saving " +m_stateManagerRef.PhasingPlayer.Gold +" for the court phase.");
					return;
				}
			}


			// Dedicated expansion:
			// If the player is surrounded by only empty areas, buy only spearmen
			if (SurroundedByEmptyAreas ()) {
				// buy spearmen
				Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + "is buying spearmen (surrounded by empty areas)");
				m_barracksStateRef.PurchaseUnits ("Spearmen");
				// set playing to false to force another check
				m_AIPlaying = false;
				return;

			}

			// Regular expansion:
			// If the player is adjacent to some empty areas, buy enough spearmen to counquer them
			if (CountAdjacentEmptyAreas () > m_stateManagerRef.PhasingPlayer.myArmyUnitTotals [0]) {
				// buy spearmen
				Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + "is buying spearmen (adjacent to empty areas)");
				m_barracksStateRef.PurchaseUnits ("Spearmen");
				// set playing to false to force another check
				m_AIPlaying = false;
				return;
			}

			// Defence - Castles:
			// If the AI can build a castle, it should build a castle.
			Debug.Log ("Checking to see if " + m_stateManagerRef.PhasingPlayer.name + " can buy any castles.");
			if (AIHasEnoughGold ("Keep")) {
				for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
					// if the area is surrounded by friendly areas
					if (SurroundedbyFriendlyAreas (m_stateManagerRef.PhasingPlayer.myEmpire [i])) {
						// if the area does not contain a castle
						if (m_stateManagerRef.PhasingPlayer.myEmpire [i].occupyingCastle == null) {
							// if there is no adjacent castle
							if (!AdjacentToAnyCastle (m_stateManagerRef.PhasingPlayer.myEmpire [i])) {
								Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " is buying a new castle at " + m_stateManagerRef.PhasingPlayer.myEmpire [i].name);
								// Buy a new keep in the the current area
								m_masonStateRef.AIPurchaseKeep (m_stateManagerRef.PhasingPlayer.myEmpire [i]);
								break;
							}
						} 
					}
					Debug.Log ("No eligible area for" + m_stateManagerRef.PhasingPlayer.name + " to build castle");
				}	
			} else {
				Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " can't afford a new castle");
			}

			// Defence - upgrades:
			// If the player's defences are threatened, build defences at the area where they are threatened.
			if (CastleThreatened ()) {
				if (AIHasEnoughGold ("Wall")) {
					m_masonStateRef.AIPurchaseDefence (m_castleThreatenedArea, 1);
					m_AIPlaying = false;
				}
				if (AIHasEnoughGold ("Defences")) {
					m_masonStateRef.AIPurchaseDefence (m_castleThreatenedArea, 0);
					m_AIPlaying = false;
				}
			
			}

			// Castle Attack:
			// If the player is in a position to attack an enemy castle, buy siege engines
			// (Starting with the most expensive)
			// If the player has an area adjacent to an enemy castle
			if (AdjacentToEnemyCastle ()) {
				// If the player has less than the specified maximum of siege engines
				if (m_stateManagerRef.PhasingPlayer.CountSiegeEngines () < m_siegeEngineMax)
					// Buy the most expensive siege engine player can afford,
					// Then set playing to false to force another check
				if (AIHasEnoughGold ("Artillery")) {
					m_barracksStateRef.PurchaseUnits ("Artillery");
					m_AIPlaying = false;
				}
				if (AIHasEnoughGold ("Belfry")) {
					m_barracksStateRef.PurchaseUnits ("Belfry");
					m_AIPlaying = false;
				}
				if (AIHasEnoughGold ("Ram")) {
					m_barracksStateRef.PurchaseUnits ("Ram");
					m_AIPlaying = false;
				}
			}

			// Area Attack:
			// Buy army units with any gold that is left
			if (AIHasEnoughGold ("Knights")) {
				m_barracksStateRef.PurchaseUnits ("Knights");
				m_AIPlaying = false;
			}
			if (AIHasEnoughGold ("Archers")) {
				m_barracksStateRef.PurchaseUnits ("Archers");
				m_AIPlaying = false;
			}
			if (AIHasEnoughGold ("Spearmen")) {
				m_barracksStateRef.PurchaseUnits ("Spearmen");
				m_AIPlaying = false;
			}
			m_AIPlaying = false;
		}

		bool AIHasEnoughGold (string unitType)
		{
			bool hasEnoughGold;
			hasEnoughGold = false;
			switch (unitType) {
			case "Spearmen":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.Soldiers [0].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Archers":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.Soldiers [1].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Knights":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.Soldiers [2].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Ram": 
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.SiegeEngines [0].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Belfry":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.SiegeEngines [1].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Artillery":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.SiegeEngines [2].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Defence":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.Defences [0].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Wall":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.Defences [1].Cost)
						hasEnoughGold = true;
					break;
				}
			case "Keep":
				{
					if (m_stateManagerRef.PhasingPlayer.Gold >= m_stateManagerRef.gameDataRef.Defences [2].Cost)
						hasEnoughGold = true;
					break;
				}
			}
			if (hasEnoughGold)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns true if the phasing player's empire borders only empty areas.
		/// Otherwise returns false.
		/// </summary>
		bool SurroundedByEmptyAreas ()
		{
			Debug.Log ("Called Surrounded by empty areas check.");

			// Declare local variable, default is true
			bool surroundedByEmptyAreas;
			surroundedByEmptyAreas = true;

			// check each area in the phasing player's empire
			for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
				// check each area's neighbour's
				for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
					// If the checked neighbour is occupied y any player who is not the phasing player
					if ((m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != null) && (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer)) {
						// set local variable to false and break from the for loop
						surroundedByEmptyAreas = false;
						break;
					}
				}
				// if local variable is false break from the for loop 
				if (!surroundedByEmptyAreas)
					break;
			}
			// If temp variable is true return true, otherwise return false.
			if (surroundedByEmptyAreas) {
				Debug.Log (m_stateManagerRef.PhasingPlayer + " is surrounded by empty areas");
				return true;
			} else {
				Debug.Log (m_stateManagerRef.PhasingPlayer + " is not surrounded by empty areas");
				return false;
			}
		}

		/// <summary>
		/// Return
		/// </summary>
		/// <returns><c>true</c>, if by empty areas was surroundeded, <c>false</c> otherwise.</returns>
		int CountAdjacentEmptyAreas ()
		{
			Debug.Log ("Counting adjacent empty areas.");
			
			// Clear adjacent areas list
			m_adjacentAreas.Clear ();
			// check each area in the phasing player's empire
			for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
				// check each of the area's neighbour's
				for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
					// if the checked area is empty
					if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer == null) {
						// Declare local bool to check if the area is already in the list, default is false
						bool areaInList;
						areaInList = false;
						// Check the areas already added to adjacent areas
						for (int k = 0; k < m_adjacentAreas.Count; k++) {
							// if the checked area is already on the list, the area is in the list
							if (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j] == m_adjacentAreas [k])
								areaInList = true;
						}
						// If the checked area is not already in the list
						if (!areaInList)
						// Add checked area to adjacent areas list
							m_adjacentAreas.Add (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j]);
					}
				}
			}
			// Return the number of adjacent areas
			Debug.Log ("Number of adjacent empty areas is " + m_adjacentAreas.Count);
			return m_adjacentAreas.Count;
		}

		/// <summary>
		/// Returns true if all of the checked area's neighbours are controlled by the phasing player
		/// </summary>
		bool SurroundedbyFriendlyAreas (Area areaCheck)
		{
			//Debug.Log ("Called Surrounded by friendly areas check.");
			// Declaring local variable, default is "true"
			bool surroundedbyFriendlyAreas;
			surroundedbyFriendlyAreas = true;

			// Check all areas which are neighbours of the checked area
			for (int i = 0; i < areaCheck.myNeighbours.Length; i++) {
				// If the checked neighbour has a controllingf player, and this player is not the phasing player
				if ((areaCheck.myNeighbours [i].ControllingPlayer != null) && (areaCheck.myNeighbours [i].ControllingPlayer != m_stateManagerRef.PhasingPlayer)) {
					// Set local variable to false and break from the for loop
					surroundedbyFriendlyAreas = false;
					break;
				}
			}
			// If the local variable is true, return true, otherwise return false
			if (surroundedbyFriendlyAreas) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Returns true if any of the checked area's neighbours contain castles
		/// </summary>
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

		/// <summary>
		/// Returns true if phasing player controls an area which is adjacent to an enemy castle.
		/// Otherwise returns false.
		/// </summary>
		bool AdjacentToEnemyCastle ()
		{
			Debug.Log ("Called Adjacent to Enemy Castles check.");

			// declaring local variable, default is "false"
			bool adjacentToEnemyCastle;
			adjacentToEnemyCastle = false;

			// check all areas in phasing player's empire
			for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
				// check their neighbours
				for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
					// if the checked neighbour area contains a castle, which is not the phasing player
					if ((m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].occupyingCastle != null) && (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer)) {
						// Set temp variable to true and break from the for loop
						adjacentToEnemyCastle = true;
						break;
					}
				}
				// If temp variable is true break from for loop 
				if (adjacentToEnemyCastle) 
					break;
			}
			// If temp varibale is true, return true, otherwsie return false
			if (adjacentToEnemyCastle) {
				Debug.Log (m_stateManagerRef.PhasingPlayer.name + " is adjacent to an enemy castle");
				return true;
			} else {
				Debug.Log (m_stateManagerRef.PhasingPlayer.name + " is not adjacent to an enemy castle");
				return false;
			}

		}

		/// <summary>
		/// Returns true if the phasing player's castle is threatened.
		/// (The castle is threatened if any of it's neighbouring areas are adjacent to an enemy controlled area)
		/// Otherwise return false
		/// </summary>
		bool CastleThreatened ()
		{
			Debug.Log ("Checking if " + m_stateManagerRef.PhasingPlayer.name + "'s castle is threatened.");

			// declaring local variable, default is "false"
			bool castleThreatened;
			castleThreatened = false;

			// check player's empire
			for (int i = 0; i < m_stateManagerRef.PhasingPlayer.myEmpire.Count; i++) {
				// if the checked area contains a castle
				if (m_stateManagerRef.PhasingPlayer.myEmpire [i].occupyingCastle != null) {
					// Check that area's neighbours
					for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire[i].myNeighbours.Length; j++) {
						// If any neighbour is adjacent to an enemy controlled area
						if ((m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != null) && (m_stateManagerRef.PhasingPlayer.myEmpire [i].myNeighbours [j].ControllingPlayer != m_stateManagerRef.PhasingPlayer)) {
							// Castle is threatened. Set the area and break from this for loop
							m_castleThreatenedArea = m_stateManagerRef.PhasingPlayer.myEmpire [i];
							castleThreatened = true;
							break;
						}
					}
					// If the castle is threatened, break from the for loop
					if (castleThreatened)
						break;
				}
			}
			// If the player's castle is threatened, return true
			if (castleThreatened) {
				Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + "'s castle is threatened at " + m_castleThreatenedArea.name + " by " + m_castleThreatenedArea.ControllingPlayer);
				return true;
			}
			// otherwise return false
			else {
				Debug.Log ("" + m_stateManagerRef.PhasingPlayer + "'s castle is not under threat.");
				return false;
			}
		}

		/// <summary>
		/// Show UI 
		/// </summary>
		public void ShowUI ()
		{
			// Show state in debug
			//Debug.Log ("In Play_CastleState");
//
//			// Show the button to switch state to the Court Play State
//			if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 70, 120, 30), "To Court")) {
//				m_stateManagerRef.SwitchState (new Play_CourtState (m_stateManagerRef));
//			}
//			// Show the button to switch state to the Conquest Play State
//			if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 110, 120, 30), "To Conquest")) {
//				m_stateManagerRef.SwitchState (new Play_ConquestState (m_stateManagerRef));
//			}
						
		}

		public void StateTrigger ()
		{
		}

		// Ends the AI phase
		void EndAIPhase ()
		{
			Debug.Log ("Ending " + m_stateManagerRef.PhasingPlayer.name + "'s castle phase");
			m_stateManagerRef.SwitchState (new Play_CourtState (m_stateManagerRef));
		}
	}
}





using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Linq;
using System.Collections;
using Assets.Code.Interfaces;
using Assets.Code.Scripts;

namespace Assets.Code.States
{
	// This inherits from IStateBase
	public class Play_ConquestState : IStateBase
	{
		// VARIABLES
		// State m_stateManagerRef
		private StateManager m_stateManagerRef;

		//Music Manager Reference
		private MusicManager m_musicManagerRef;

		// Game Data Reference
		private GameObject m_gameManager;

		// Conquest Scene References
		private GameObject m_conquestManager;
		private ConquestMapManager m_conquestMapManagerRef;
		private PitchedBattleManager m_pitchedBattleManagerRef;
		private SiegeBattleManager m_siegeBattleManagerRef;


		// AI references
		private int m_AIArmyCountMin = 3;
		private int m_AISiegeEngineMin = 3;
		private Area m_AItargetArea;
		private bool m_AIplaying = false;

		// Timers
		private int m_loadCountDown;
		private int m_delayTime = 30;
		private int m_delayTimer = 0;
		private bool m_countingDelay;
		private bool m_siegeAttackStarted = false;
		private int m_siegeAttackDelayTime = 20;
		private int m_siegeAttackDelayTimer = 0;
		private bool m_countingSiegeAttackDelay;

		// CONSTRUCTOR
		public Play_ConquestState (StateManager managerRef)
		{
			// Set the State m_stateManagerRef
			m_stateManagerRef = managerRef;
			// Set up reference to Game Manager
			m_gameManager = GameObject.Find ("GameManager");
			//Set up Music Manager
			m_musicManagerRef = m_gameManager.GetComponent<MusicManager> ();

			
			// If the current scene is not scene 3, load scene 3
			if (Application.loadedLevelName != "Conquest") {
				Application.LoadLevel ("Conquest");
				m_loadCountDown = 10;
				Debug.Log ("Switched to Conquest Scene");

				// Activate the UI

				//m_stateManagerRef.uiManagerRef.AIPlayingMask.SetActive(false);


				if (m_stateManagerRef.PhasingPlayer.IsHuman) {
					m_stateManagerRef.uiManagerRef.aiPlayingPanel.SetActive (false);
					
				} else {
					m_stateManagerRef.uiManagerRef.aiPlayingPanel.SetActive (true);
//					m_stateManagerRef.uiManagerRef.AIPlayingText.text = "" +m_stateManagerRef.PhasingPlayer.MyFaction.name + 
//						" (" +m_stateManagerRef.PhasingPlayer.name +")";
				}


				//Activate UI for human players
				m_stateManagerRef.ActivateUI ();
				if (m_stateManagerRef.PhasingPlayer.IsHuman){
				
					m_stateManagerRef.uiManagerRef.uiBar.SetActive (true);

				}
				else{
					m_stateManagerRef.uiManagerRef.uiBar.SetActive (false);
				}
				//Check if Regions are correct
				m_stateManagerRef.PhasingPlayer.CheckRegions ();
				m_musicManagerRef.PauseCastleAndCourtMusic ();
				m_musicManagerRef.PauseCastleAmbience ();
				m_musicManagerRef.PauseMenuMusic();

				m_musicManagerRef.PlayConquestMusic ();
			}
		}
		
		/// <summary>
		/// Update this state.
		/// </summary>
		public void StateUpdate ()
		{
			//Debug.Log ("In Play_ConquestState");

			// Delay assignment of conquest m_stateManagerRef to allow scene to load
			if (m_loadCountDown > 0) {
				m_loadCountDown--;
			}
			if ((m_loadCountDown == 0) && (m_conquestManager == null)) {
				m_conquestManager = GameObject.Find ("ConquestManager");
				//Debug.Log ("Conquest m_stateManagerRef is " + m_conquestManager.name);

				m_conquestMapManagerRef = m_conquestManager.GetComponent<ConquestMapManager> ();
				//Debug.Log ("Map ref is: " + m_conquestMapManagerRef.name);

				m_pitchedBattleManagerRef = m_conquestManager.GetComponent<PitchedBattleManager> ();
				//Debug.Log ("Pitched battle ref is: " + m_pitchedBattleManagerRef.name);

				m_siegeBattleManagerRef = m_conquestManager.GetComponent<SiegeBattleManager> ();
				//Debug.Log ("Siege battle ref is: " + m_siegeBattleManagerRef.name);

				m_stateManagerRef.uiManagerRef.aiPlayingMask.SetActive (false);


			}




			// if the conquest m_stateManagerRef reference has been assigned
			if (m_conquestManager != null) {
				// If the phasing player is not human
				if (m_stateManagerRef.PhasingPlayer.IsHuman != true) {
					// AI Logic goes here
					//Debug.Log ("AI is in control");

					// If the AI is not currently playing
					if (m_AIplaying != true) {
						// Increase the delay timer
						m_delayTimer++;
						// If the delay timer reaches the delay time
						if (m_delayTimer >= m_delayTime) {
							// Check the army strength, set playing to true & reset the timer
							m_AIplaying = true;
							AICheckArmyStrength ();
							m_delayTimer = 0;

						}
					}
				}
			}

			// If we are counting the Siege Attack Delay
			if (m_countingSiegeAttackDelay) {
				// Increase the delay timer as long as it is below the time limit
				if (m_siegeAttackDelayTimer < m_siegeAttackDelayTime)
					m_siegeAttackDelayTimer++;
				// Once it reaches the time limit, call the delayed siege attack method
				else
					AIDelayedSiegeAttack ();
			}

		}

		/// <summary>
		/// Show UI 
		/// </summary>
		public void ShowUI ()
		{
			// Show state in debug
			//Debug.Log ("In Play_ConquestState");

//			for (int i = 0; i < m_stateManagerRef.gameDataRef.activePlayers.Count; i++) {
//				Debug.Log ("" + m_stateManagerRef.gameDataRef.activePlayers [i].name + " has " + m_stateManagerRef.gameDataRef.activePlayers [i].Influence + " influence");
//			}

			// Show the button to switch state to the Castle Play State
//			if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 70, 120, 60), "End Turn")) {
//				//Check to see if this is the last turn
//				if (m_stateManagerRef.IsLastTurn()) {
//					// If so switch to the end state of the game
//					m_conquestMapManagerRef.SetGameVictoryPanel();
//				} else {
//					// If not switch to the castle state (i.e. start the next turn as normal)
//					m_stateManagerRef.uiManagerRef.aiPlayingMask.SetActive (true);
//					m_stateManagerRef.SwitchState (new Play_CastleState (m_stateManagerRef));
//				}
//			}
//			// Show the button to switch state to the End State
//			if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 140, 120, 60), "End Game")) {
//				m_stateManagerRef.SwitchState (new EndState (m_stateManagerRef));
//			}
//
//			// Show the button to switch state to the Loss State
//			if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 210, 120, 60), "Win")) {
//				m_stateManagerRef.SwitchState (new EndState (m_stateManagerRef));
//			}
		}

		/// <summary>
		/// StateTrigger is called when the AI player has finished an invasion action.
		/// By setting isPlaying to false it forces a check for the next action via the update method. 
		/// </summary>
		public void StateTrigger ()
		{
			Debug.Log ("Called state trigger");

			// If there is not currently a siege battle in progress 
			if (m_siegeBattleManagerRef.BattleArea == null) {
				Debug.Log ("Siege battle area is null");
				// set AI playing to false
				m_AIplaying = false;
				m_siegeAttackStarted = false;
				// Otherwise assume a siege battle is in progress
			} else {
				Debug.Log ("Siege battle area is not null");
				// If the attack has started
				if (m_siegeAttackStarted)
					// Set AI playing to false
					m_AIplaying = false;
				// If the attack has not started
				else
					// Start the siege attack
					StartSiegeAttack ();
			}
		}

		/// <summary>
		/// Ends the AI turn.
		/// </summary>
		void EndAITurn ()
		{
			m_stateManagerRef.uiManagerRef.aiPlayingMask.SetActive (true);
			m_stateManagerRef.PhasingPlayer.SetLastAttackedBy (null);
			if (m_stateManagerRef.IsLastTurn ()) {
				m_conquestMapManagerRef.SetGameVictoryPanel();
				//m_stateManagerRef.SwitchState (new EndState (m_stateManagerRef));
			} else {
				m_stateManagerRef.activeState = null;
				m_stateManagerRef.SwitchState (new Play_CastleState (m_stateManagerRef));
			}

		}

		/// <summary>
		/// The AI player checks whether they have enough military units to warrant an attack.
		/// </summary>
		void AICheckArmyStrength ()
		{
			if (!m_stateManagerRef.PhasingPlayer.IsHuman) {
				Debug.Log ("Called Check Army Strength");
				m_siegeAttackStarted = false;

				// If the phasing player has less than the minimum for an atack
				if (m_stateManagerRef.PhasingPlayer.CountSoldiers() < m_AIArmyCountMin) {
					// End the turn
					EndAITurn ();
				} 
			//if the AI player does have enough units for an attack
			else {
					// Set a target area
					AISetTargetArea ();
				}
			}
		}

		/// <summary>
		/// This is called when the needs to decide which area to target.
		/// An areaCheck array stores an int value for each area.
		/// A series of checks assessing each area's desirability and increases it's areaCheck int acorrdingly.
		/// The first area found with the highest score is then assigned.
		/// </summary>
		void AISetTargetArea ()
		{
			Debug.Log ("Called Set Target Area");
			m_AItargetArea = null;
			// Create a blank integer array to check all areas
			int[] areaCheck = new int[m_stateManagerRef.gameDataRef.AllAreas.Length];
			// For all areas...
			for (int i = 0; i < m_stateManagerRef.gameDataRef.AllAreas.Length; i++) {
				// Compare each against the areas in the player's empire
				for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire.Count; j++) {
					// And check if they are a neighbour to that area
					for (int k = 0; k < m_stateManagerRef.PhasingPlayer.myEmpire[j].myNeighbours.Length; k++) {
						// If they are a neighbour
						if (m_stateManagerRef.gameDataRef.AllAreas [i] == m_stateManagerRef.PhasingPlayer.myEmpire [j].myNeighbours [k]) {
							// Which this player does not already control
							if (m_stateManagerRef.gameDataRef.AllAreas [i].ControllingPlayer != m_stateManagerRef.PhasingPlayer) {
								// Set areaCheck to 1
								areaCheck [i] = 1;
							}
						}
					}
				}
			}

			// Now that all potentially conquerable area have been identified (by setting their areaCheck int to 1)
			// Check through all of these and increase the area check dependent on the following criteria:
			for (int i = 0; i < m_stateManagerRef.gameDataRef.AllAreas.Length; i++) {
				if (areaCheck [i] == 1) {
					// Increase it again if the area is empty
					if (m_stateManagerRef.gameDataRef.AllAreas [i].ControllingPlayer == null) 
						areaCheck [i]++;
					// Increase it again if the area is empty and the player has spearmen
					if ((m_stateManagerRef.gameDataRef.AllAreas [i].ControllingPlayer == null) && (m_stateManagerRef.PhasingPlayer.myArmyUnitTotals [0] > 0))
						areaCheck [i]++;
					// Increase if the area is part of the player's home region
					if (m_stateManagerRef.gameDataRef.CheckIfHomeRegion (m_stateManagerRef.PhasingPlayer, m_stateManagerRef.gameDataRef.AllAreas [i], m_stateManagerRef.gameDataRef.AllAreas [i].tag))
						areaCheck [i] ++;
					// Increase it again if the area is part of a region in which the player already has presence
					if (m_stateManagerRef.gameDataRef.CheckRegionPresence (m_stateManagerRef.PhasingPlayer, m_stateManagerRef.gameDataRef.AllAreas [i].tag))
						areaCheck [i]++;
					// If player was attacked last turn
					if (m_stateManagerRef.PhasingPlayer.LastAttackedByPlayer != null) {
						// If the area belongs to the player who attacked this player last turn
						if (m_stateManagerRef.gameDataRef.AllAreas [i].ControllingPlayer == m_stateManagerRef.PhasingPlayer.LastAttackedByPlayer)
							areaCheck [i]++;
					}
					// If the area contains a castle
					if (m_stateManagerRef.gameDataRef.AllAreas [i].occupyingCastle != null) {
						// If the phasing player has no siege engines
						if (!m_stateManagerRef.PhasingPlayer.HasSiegeEngines ())
							// Set area check to 0, as player can't attack
							areaCheck [i] = 0;
						// If the phasing player has at least as many siege engines as the minimum
						if (m_stateManagerRef.PhasingPlayer.CountSiegeEngines () >= m_AISiegeEngineMin)
							Debug.Log ("" + m_stateManagerRef.PhasingPlayer.name + " has " + m_stateManagerRef.PhasingPlayer.CountSiegeEngines () + " Siege Engines");
						// Increase the value
						areaCheck [i]++;

					}

				}
			}

			// Find the first area in the list with the highest number in the array
			for (int i = 0; i < areaCheck.Length; i++) {
				// If this area ref got the highest score
				if (areaCheck [i] == areaCheck.Max ()) {
					// Compare each against the areas in the player's empire
					for (int j = 0; j < m_stateManagerRef.PhasingPlayer.myEmpire.Count; j++) {
						// And check if they are a neighbour to that area
						for (int k = 0; k < m_stateManagerRef.PhasingPlayer.myEmpire[j].myNeighbours.Length; k++) {
							// If they are a neighbour
							if (m_stateManagerRef.gameDataRef.AllAreas [i] == m_stateManagerRef.PhasingPlayer.myEmpire [j].myNeighbours [k]) {
								// Which this player does not already control
								if (m_stateManagerRef.gameDataRef.AllAreas [i].ControllingPlayer != m_stateManagerRef.PhasingPlayer) {
									// Failsafe check:
									// If this area contains a castle and the player has no siege engines
									if ((m_stateManagerRef.gameDataRef.AllAreas [i].occupyingCastle != null) && (!m_stateManagerRef.PhasingPlayer.HasSiegeEngines ())) {
										// Exit this loop to check next area
										break;
									} else {
										// Set this as the target area
										m_AItargetArea = m_stateManagerRef.gameDataRef.AllAreas [i];
										// Attack the area
										Debug.Log ("AI target area is: " + m_AItargetArea.name);
										AIAttack ();
										break;
									}
								}
							}
						}
						// Break if a target area has been assigned
						if (m_AItargetArea != null)
							break;
					}
				}
				// Break if a target area has been assigned
				if (m_AItargetArea != null)
					break;
			}

			// If m_target area is still null
			if (m_AItargetArea == null) {
				// End the turn
				EndAITurn ();
			}
		}


		/// <summary>
		/// This is called when the AI is instructed to attack its target area
		/// </summary>
		void AIAttack ()
		{


			// Select the target area in the conquest map m_stateManagerRef
			m_conquestMapManagerRef.SetSelectedArea (m_AItargetArea);
			// Treat as if a human player had agreed to invade this area
			m_conquestMapManagerRef.YesToInvadePrompt ();


			// If the target area doesn't contain a castle...
			if (m_AItargetArea.occupyingCastle == null) {
				// Treat as if a human player ok'd the place units prompt
				m_pitchedBattleManagerRef.attackPlaceUnitsPromptButton.onClick.Invoke ();
				// Make an AI Attacking Army
				m_pitchedBattleManagerRef.SetAIAttack ();
				// Switch active player to defence
				m_pitchedBattleManagerRef.SwitchActivePlayerType ();
				// check if the attack is opposed
				m_pitchedBattleManagerRef.CheckUnopposed ();
				// If AI is playing against no defender
				if (m_pitchedBattleManagerRef.Defender == null) {
					// OK the victory panel button
					m_pitchedBattleManagerRef.victoryOKButton.onClick.Invoke ();
				} 
			// If AI is playing against a non-human defender
			else if (!m_pitchedBattleManagerRef.Defender.IsHuman) {
					// OK the victory panel button
					m_pitchedBattleManagerRef.victoryOKButton.onClick.Invoke ();
				}
			}

		}

		// Starts a siege attack
		void StartSiegeAttack ()
		{
			// Set siege attack started to true
			m_siegeAttackStarted = true;
			// Make an AI attacking army
			m_siegeBattleManagerRef.SetAIAttack ();
			// Set the Attack Panel
			m_siegeBattleManagerRef.SetAIAttackPanel ();
			// Start the siege battle
			m_siegeBattleManagerRef.StartSiegeAttack ();

			// If neither the Attacker nor the Defender is human
			if ((!m_siegeBattleManagerRef.Attacker.IsHuman) && (!m_siegeBattleManagerRef.Defender.IsHuman)) {
				// Do an immediate AI attack
				AIImmediateSiegeAttack ();
			} else {
				// Otherwise, start the delayed attack sequence
				AIDelayedSiegeAttack ();
			}
		}

		// The immediate siege attack is used for AI vs AI battles
		void AIImmediateSiegeAttack ()
		{
			// Resolve the combat
			m_siegeBattleManagerRef.ResolveCombat ();
			// Repeat as long as either side has any units left
			if ((m_siegeBattleManagerRef.attackingArmy.Count () > 0) && (m_siegeBattleManagerRef.defendingArmy.Count () > 0)) {
				AIImmediateSiegeAttack ();
			}
		}

		// The delayed siege attack sets off a timer-driven loop to allow the player to see what is going on
		void AIDelayedSiegeAttack ()
		{
			// If the siege attack delay timer is 0
			if (m_siegeAttackDelayTimer == 0) {
				// Start counting
				m_countingSiegeAttackDelay = true;
				// Otherwise
			} else {
				// stop counting, set the timer to 0 and resolve a combat
				m_countingSiegeAttackDelay = false;
				m_siegeAttackDelayTimer = 0;
				m_siegeBattleManagerRef.ResolveCombat ();
				// If there are units remaining in the attacking army
				if (m_siegeBattleManagerRef.attackingArmy [0] != null) {
					// Start another delayed siege attack sequence
					AIDelayedSiegeAttack ();
				}
			}
		}
	}
}

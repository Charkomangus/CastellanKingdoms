using UnityEngine;
using System.Collections;
using Assets.Code.Interfaces;
using Assets.Code.Scripts;

namespace Assets.Code.States
{
	// This inherits from IStateBase
	public class Start_MenuState : IStateBase
	{
		// VARIABLES
		// State m_stateManagerRef
		private StateManager m_stateManagerRef;
		//Music Manager Reference
		private MusicManager musicManagerRef;
		
		// Game Data Reference
		private GameObject m_gameManager;

		
		// CONSTRUCTOR
		public Start_MenuState (StateManager managerRef)
		{
			// Set the State m_stateManagerRef
			m_stateManagerRef = managerRef;
			// Set up reference to Game Manager
			m_gameManager = GameObject.Find ("GameManager");
			//Set up Music Manager
			musicManagerRef = m_gameManager.GetComponent<MusicManager> ();

			// If the current scene is not Game scene, load Game scene
			if (Application.loadedLevelName != "Game") {
				Application.LoadLevel ("Game");
				Debug.Log ("Switched to Game Scene");

				//Deactivate UI
				m_stateManagerRef.DeactivateUI ();
			}

			m_stateManagerRef.startMenuManagerRef.CheckForSavedGame ();
			musicManagerRef.PlayMenuMusic ();
//			Cursor.visible = true;
		}

		/// <summary>
		/// Update this state.
		/// </summary>
		public void StateUpdate ()
		{


		}


		/// <summary>
		/// Show UI 
		/// </summary>
		public void ShowUI ()
		{
			// Show state in debug
			//Debug.Log ("In Start_MenuState");

//			// If the player is ready to start the game
//			if (m_stateManagerRef.startMenuManagerRef.readyTostart != false) {
//				// Show the button to switch state to the starting intro
//				if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 70, 120, 60), "Play Intro")) {
//					// Switch to the Start Intro State					
//					m_stateManagerRef.SwitchState (new Start_OpeningState (m_stateManagerRef));
//					// Deactivate the Player Selection Menu
//					m_stateManagerRef.startMenuManagerRef.DeactivateSelectionMenu ();
//					// Set up the Active Players List
//					m_stateManagerRef.gameDataRef.SetActivePlayers ();
//					// Set up the starting armies
//					m_stateManagerRef.gameDataRef.SetStartingArmies ();
//					// Set up the starting castles
//					m_stateManagerRef.gameDataRef.SetStartingCastles ();
//					// Set up the starting empires
//					m_stateManagerRef.gameDataRef.SetStartingEmpires ();
//					// Set the starting stats
//					m_stateManagerRef.gameDataRef.SetStartingStats();
//					// Set the AI priority
//					m_stateManagerRef.gameDataRef.SetAIPriority();
//	}
//			}

		}

		public void StateTrigger ()
		{
		}
	}
	
}

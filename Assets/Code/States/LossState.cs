using UnityEngine;
using System.Collections;
using Assets.Code.Interfaces;
using Assets.Code.Scripts;

namespace Assets.Code.States
{
		// This inherits from IStateBase
		public class LossState : IStateBase
		{
				// VARIABLES
				// State m_stateManagerRef
				private StateManager m_stateManagerRef;
				//Music Manager Reference
				private MusicManager m_musicManagerRef;
				
				// Game Data Reference
				private GameObject m_gameManager;

		
				// CONSTRUCTOR
				public LossState (StateManager managerRef)
				{
						// Set the State m_stateManagerRef
						m_stateManagerRef = managerRef;
						// If the current scene is not scene 5, load scene 5
						if (Application.loadedLevelName != "Loss") {
								Application.LoadLevel ("Loss");
								Debug.Log ("Switched to Loss Scene");
						}
					// Set up reference to Game Manager
					m_gameManager = GameObject.Find ("GameManager");
					//Set up Music Manager
					m_musicManagerRef = m_gameManager.GetComponent<MusicManager> ();
					//Pause Music
					m_musicManagerRef.PauseConquestMusic ();
					m_musicManagerRef.PauseCastleAmbience ();
					m_musicManagerRef.PauseCastleAndCourtMusic ();
					m_musicManagerRef.PauseMenuMusic ();

					//Deactivate UI
					m_stateManagerRef.DeactivateUI();
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
						Debug.Log ("In LossState");
			
//						// Show the button to switch state to the menu
//						if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 70, 120, 60), "Menu")) {
//								m_stateManagerRef.NewGame();
				}

		public void StateTrigger()
		{
		}
	}
	
}

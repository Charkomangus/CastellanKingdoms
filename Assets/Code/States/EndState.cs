using UnityEngine;
using System.Collections;
using Assets.Code.Interfaces;
using Assets.Code.Scripts;

namespace Assets.Code.States
{
		// This inherits from IStateBase
		public class EndState : IStateBase
		{
				// VARIABLES
				// State m_stateManagerRef
				private StateManager m_stateManagerRef;
				//Music Manager Reference
				private MusicManager m_musicManagerRef;
				
				// Game Data Reference
				private GameObject m_gameManager;

		
				// CONSTRUCTOR
				public EndState (StateManager managerRef)
				{
						// Set the State m_stateManagerRef
						m_stateManagerRef = managerRef;
						// If the current scene is not scene 4, load scene 4
						if (Application.loadedLevelName != "End") {
								Application.LoadLevel ("End");
								Debug.Log ("Switched to End Scene");
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
						Debug.Log ("In EndState");
//			
//						// Show the button to switch state to the menu
//						if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 70, 120, 60), "Menu")) {
//								m_stateManagerRef.NewGame();
//						}
				}

		public void StateTrigger()
		{}
		}

}

	

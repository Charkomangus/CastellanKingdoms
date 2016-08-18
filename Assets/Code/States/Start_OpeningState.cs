using UnityEngine;
using System.Collections;
using Assets.Code.Interfaces;
using Assets.Code.Scripts;

namespace Assets.Code.States
{
		// This inherits from IStateBase
	public class Start_OpeningState : IStateBase
		{

				// VARIABLES
				// State m_stateManagerRef
				private StateManager m_stateManagerRef;

				//Music Manager Reference
				private MusicManager m_musicManagerRef;
				
				// Game Data Reference
				private GameObject m_gameManager;
				
				// CONSTRUCTOR
				public Start_OpeningState (StateManager managerRef)
				{
						// Set the State m_stateManagerRef
						m_stateManagerRef = managerRef;
						// Set up reference to Game Manager
						m_gameManager = GameObject.Find ("GameManager");
						//Set up Music Manager
						m_musicManagerRef = m_gameManager.GetComponent<MusicManager> ();
						// If the current scene is not game, load game
						if (Application.loadedLevelName != "Opening") {
								Application.LoadLevel ("Opening");
								Debug.Log ("Switched to Opening State");
								//Deactivate UI
								m_stateManagerRef.DeactivateUI();
						}
						m_musicManagerRef.PauseMenuMusic ();
						m_musicManagerRef.PlayopeningMusic ();
			Cursor.visible = false;
				}

				/// <summary>
				/// Update this state.
				/// </summary>
				public void StateUpdate ()
				{
						
								//Application.LoadLevel ("Castle");
			
				}

				
				/// <summary>
				/// Show UI 
				/// </summary>
				public void ShowUI ()
				{
						// Show state in debug
						//Debug.Log ("In Start_IntroState");
			
						// Show the button to switch state to the Castle Play State
//						if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 70, 120, 60), "Start")) {
//								m_stateManagerRef.SwitchState (new Play_CastleState (m_stateManagerRef));
//								m_stateManagerRef.StartFirstTurn ();
//						}
				}

		public void StateTrigger()
		{


		}
	}
}

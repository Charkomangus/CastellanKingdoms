using UnityEngine;
using System.Collections;
using Assets.Code.Interfaces;
using Assets.Code.Scripts;

namespace Assets.Code.States
{
	// This inherits from IStateBase
	public class Play_CourtState : IStateBase
	{
		// VARIABLES
		// Game Data Reference
		private GameObject m_gameManager;

		// Game Data Ref
		private GameData m_gameDataRef;

		// State m_stateManagerRef
		private StateManager m_stateManagerRef;

		// Controller References
		private GameObject m_controllerRef;
				
		//Music Manager Reference
		private MusicManager m_musicManagerRef;
				


		// Court Manager Reference
		private GameObject m_courtManager;
		private CourtManager m_courtManagerRef;

		// loading counter
		private int m_loadCountDown;

		// AI references
		private bool m_AIPlaying;
		private bool m_AIDoneWithReligiousAdvisor;
		private bool m_AIDoneWithPoliticalAdvisor;
				
		// CONSTRUCTOR
		public Play_CourtState (StateManager managerRef)
		{
			// Set the State m_stateManagerRef
			m_stateManagerRef = managerRef;
			// Set up reference to Game Manager
			m_gameManager = GameObject.Find ("GameManager");
			// Set up game data ref
			m_gameDataRef = m_gameManager.GetComponent<GameData>();
			//Set up Music Manager ref
			m_musicManagerRef = m_gameManager.GetComponent<MusicManager> ();

			// If the current scene is not scene 2, load scene 2
			if (Application.loadedLevelName != "Court") {
				Application.LoadLevel ("Court");
				m_loadCountDown = 10;
				Debug.Log ("Switched to Court Scene");
			}

			m_musicManagerRef.PauseCastleAmbience ();

			//Activate UI for human players
			m_stateManagerRef.ActivateUI ();
			if (m_stateManagerRef.PhasingPlayer.IsHuman){
				
				m_stateManagerRef.uiManagerRef.uiBar.SetActive (true);

			}
			else{
				m_stateManagerRef.uiManagerRef.uiBar.SetActive (false);

			}

			m_AIPlaying = false;

						
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
			if ((m_loadCountDown == 0) && (m_courtManager == null)) {
				m_courtManager = GameObject.Find ("CourtManager");
				m_courtManagerRef = m_courtManager.GetComponent<CourtManager> ();
			}

			// If court manager has been assigned
			if (m_courtManager != null) {
				// If the phasing player is AI
				if (!m_stateManagerRef.PhasingPlayer.IsHuman) {
					// If the AI is not currently playing
					if (!m_AIPlaying) {
						// Set it playing
						m_AIPlaying = true;
						AICheckNextAction ();
					}
				}
			}

		}

		/// <summary>
		/// Show UI 
		/// </summary>
		public void ShowUI ()
		{
			// Show the button to switch state to the Conquest Play State
//			if (GUI.Button (new Rect (Screen.width - 130, Screen.height - 70, 120, 60), "To Conquest")) {
//				m_stateManagerRef.SwitchState (new Play_ConquestState (m_stateManagerRef));
//			}

		}

		public void StateTrigger ()
		{
		}

		/// <summary>
		/// Checks what the AI player's next action should be.
		/// </summary>
		public void AICheckNextAction()
		{
			switch (m_stateManagerRef.PhasingPlayer.aiPriority) {
			case Player.AIPriority.Popularity:
			{
				// If he has not yet spoken to the political advisor
				if (!m_AIDoneWithPoliticalAdvisor)
				// Speak to the political advisor
					AIDealWithPoliticalAdvisor();
				// otherwise if he has not spoken to the religious advisor
				else if (!m_AIDoneWithReligiousAdvisor)
				// Speak to the religious advisor
					AIDealWithReligiousAdvisor();
				// otherwise end the turn
				else 
					AIEndPhase();
				break;
			}
			case Player.AIPriority.Religion:
			{
				// If he has not yet spoken to the religious advisor
				if (!m_AIDoneWithReligiousAdvisor)
				// Speak to the religious advisor
					AIDealWithReligiousAdvisor();
				// otherwise if he has not spoken to the political advisor
				else if (!m_AIDoneWithPoliticalAdvisor)
				// Speak to the political advisor
					AIDealWithPoliticalAdvisor();
				// otherwise end the turn
				else
					AIEndPhase();
				break;
			}
			case Player.AIPriority.Military:
			{
				// If he has not yet spoken to either the religious or political advisor
				if ((!m_AIDoneWithPoliticalAdvisor) && (!m_AIDoneWithReligiousAdvisor))
				{
					// Get a random value between 0 and 1
					float rand;
					rand = Random.value;
					// If random value is less than or equal to 0.5 speak to the religious advisor
					if (rand <= 0.5)
					{					
						AIDealWithReligiousAdvisor();
					}
					// Otherwise speak to the political advisor
					else 
					{
						AIDealWithPoliticalAdvisor();
					}
				}
				// Otherwise if he hasn't spoken to the political advisor
				else if (!m_AIDoneWithPoliticalAdvisor)	
				// speak to the political advisor
					AIDealWithPoliticalAdvisor();
				// Otherwise if he hasn't spoken to the religious advisor
				else if (!m_AIDoneWithReligiousAdvisor)
					// speak to the religious advisor
					AIDealWithReligiousAdvisor();
				// Otherwise end the turn
				else 
					AIEndPhase();
				break;
			}

			}

		}

		/// <summary>
		/// The AI deals with religious advisor.
		/// </summary>
		public void AIDealWithReligiousAdvisor()
		{
			Debug.Log (""+m_stateManagerRef.PhasingPlayer +" is dealing with the religious advisor");

			// Find the first available papal bull to deal with
			// Check all cards in the selected papal cards array
			for (int i = 0; i < m_courtManagerRef.deckPapalRef.selectedPapalCard.Length; i++) {
				// if the current card has not been played
				if (!m_courtManagerRef.deckPapalRef.selectedPapalCard[i].Played)
				{
					// Set the selected slot (so as to use the same terminology as in court manager)
					string selectedSlot;
					selectedSlot = "Slot "+(i+1);
					m_courtManagerRef.DealPapalValue(selectedSlot);
					break;
				}
			}
			// AI is finished with this action
			m_AIDoneWithReligiousAdvisor = true;
			m_AIPlaying = false;
		}

		/// <summary>
		/// The AI deals with political advisor.
		/// </summary>
		public void AIDealWithPoliticalAdvisor()
		{
			Debug.Log (""+m_stateManagerRef.PhasingPlayer +" is dealing with the political advisor");
			// Draw a political card
			m_courtManagerRef.DrawPoliticalCard ();
			// local string for the option parameter, default Option1
			string option;
			option = "Option1";
			// local float for a random number
			float rand;
			rand = Random.value;
			// if the random value is greater than 0.5, change option to Option2
			if (rand > 0.5)
				option = "Option2";
			// Give outcome for the selected option
			Debug.Log ("For political card "  +m_courtManagerRef.polCard.title +", " +m_stateManagerRef.PhasingPlayer.name +" picked " +option);
			m_courtManagerRef.GiveOutcome(option);
			// AI is finished ith this action
			m_AIDoneWithPoliticalAdvisor = true;
			m_AIPlaying = false;
		}

		/// <summary>
		/// Ends the AI player's court phase. 
		/// </summary>
		public void AIEndPhase()
		{
			Debug.Log(""+m_stateManagerRef.PhasingPlayer +" is ending their court phase");
			// Switch to the conquest phase
			m_stateManagerRef.SwitchState (new Play_ConquestState (m_stateManagerRef));
		}


	}
}



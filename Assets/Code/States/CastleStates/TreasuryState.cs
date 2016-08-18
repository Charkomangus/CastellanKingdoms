using Assets.Code.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.States.CastleStates
{
    public class TreasuryState : MonoBehaviour
    {


        // Game Data
        private GameObject m_gameManager;
        private StateManager m_StateManager;

        // Gold Variables
        public GameObject[] goldPiles;
        public Button taxButton;

        public bool overtaxButtonPressed;

        void Awake ()
        {
            m_gameManager = GameObject.Find ("GameManager");
            m_StateManager = m_gameManager.GetComponent<StateManager> ();
        }
	

        // Use this for initialization
        void Start ()
        {
            CollectTax ();
            if (m_StateManager.PhasingPlayer.Popularity < 2)
                taxButton.interactable = false;
        }
	


        // Checks the Active Player's gold and
        // Removes the units cost from the active players reserve 
        public void ShowGold ()
        {
            if (m_StateManager.PhasingPlayer.Gold >= 1)
                goldPiles [0].SetActive (true);
            else
                goldPiles [0].SetActive (false);
						
            if (m_StateManager.PhasingPlayer.Gold >= 5)
                goldPiles [1].SetActive (true);
            else
                goldPiles [1].SetActive (false);

            if (m_StateManager.PhasingPlayer.Gold >= 10)
                goldPiles [2].SetActive (true);
            else
                goldPiles [2].SetActive (false);

            if (m_StateManager.PhasingPlayer.Gold >= 15)
                goldPiles [3].SetActive (true);
            else
                goldPiles [3].SetActive (false);

            if (m_StateManager.PhasingPlayer.Gold >= 20)
                goldPiles [4].SetActive (true);
            else
                goldPiles [4].SetActive (false);

            if (m_StateManager.PhasingPlayer.Gold >= 25)
                goldPiles [5].SetActive (true);
            else
                goldPiles [5].SetActive (false);

            if (m_StateManager.PhasingPlayer.Gold >= 30)
                goldPiles [6].SetActive (true);
            else
                goldPiles [6].SetActive (false);

            if (m_StateManager.PhasingPlayer.Gold >= 35)
                goldPiles [7].SetActive (true);
            else
                goldPiles [7].SetActive (false);

            if (m_StateManager.PhasingPlayer.Gold >= 40)
                goldPiles [8].SetActive (true);
            else
                goldPiles [8].SetActive (false);
        }

        //Collect Gold eqqual to the players Castles and Areas
        public void CollectTax ()
        {
            m_StateManager.PhasingPlayer.CalculateTax ();
            m_StateManager.PhasingPlayer.AddToGold (m_StateManager.PhasingPlayer.GoldPerTurn);
            Debug.Log ("" + m_StateManager.PhasingPlayer.name + " has collected " + m_StateManager.PhasingPlayer.GoldPerTurn +" gold");
            Debug.Log ("" + m_StateManager.PhasingPlayer.name + " has " + m_StateManager.PhasingPlayer.Gold + " gold");
            //Update UI
            m_StateManager.uiManagerRef.UpdateUI ();
            ShowGold ();
        }



        public void CollectOverTax ()
        {
            m_StateManager.PhasingPlayer.CalculateTax ();
            m_StateManager.PhasingPlayer.AddToGold (m_StateManager.PhasingPlayer.GoldPerTurn/2);
            m_StateManager.PhasingPlayer.AddToPopularity (-2);
            Debug.Log ("" + m_StateManager.PhasingPlayer.name + " has (from the blood of his people) collected " + m_StateManager.PhasingPlayer.GoldPerTurn +" gold");
            Debug.Log ("" + m_StateManager.PhasingPlayer.name + " has " + m_StateManager.PhasingPlayer.Gold + " gold");
            //Update UI
            m_StateManager.uiManagerRef.UpdateUI ();
            ShowGold ();
            if (overtaxButtonPressed == false) {
                taxButton.interactable = false;
                overtaxButtonPressed = true;
            }

        }	

    }
}

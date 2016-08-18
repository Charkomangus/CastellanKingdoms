using Assets.Code.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.States.CastleStates
{
    public class BarracksState : MonoBehaviour {

        public Text[] unitNumbers;

        // Game Data
        private GameObject m_gameManagerRef;
        private GameData m_gameDataRef;
        private StateManager m_stateManagerRef;

        public Button[] shopArmyArray;
        public Button[] shopSiegeArray;

        void Awake ()
        {
            m_gameManagerRef = GameObject.Find ("GameManager");
            m_gameDataRef = m_gameManagerRef.GetComponent<GameData> ();
            m_stateManagerRef = m_gameManagerRef.GetComponent<StateManager> ();
        }


        // Use this for initialization
        void Start () {

        }
	
        // Update is called once per frame
        void Update () {
	
        }
        //Set My Army Textbox to be accurate and updates the UI
        public void SetUnitNumbers()
        {
            m_stateManagerRef.PhasingPlayer.CountArmyUnits ();
            for (int i =0; i < unitNumbers.Length; i++) {
                unitNumbers[i].text = m_stateManagerRef.PhasingPlayer.myArmyUnitTotals[i].ToString();	
            }
            m_stateManagerRef.uiManagerRef.UpdateUI ();
        }

        //Check if Player's Gold and disable the Military Unit Buttons they cannot afford
        public void CheckSoldierCost()
        {
            for (int i = 0; i < shopArmyArray.Length; i++)
                if (m_stateManagerRef.PhasingPlayer.Gold < m_gameDataRef.Soldiers [i].Cost) {
                    shopArmyArray [i].interactable = false;
                    Debug.Log ("You cannot afford " + m_gameDataRef.Soldiers [i].name);
                }
                else
                    shopArmyArray[i].interactable = true;

        }

        //Check if Player's Gold and disable the Siege Unit Buttons they cannot afford
        public void CheckSiegeCost()
        {
            for (int j = 0; j < shopSiegeArray.Length; j++)				
                if (m_stateManagerRef.PhasingPlayer.Gold < m_gameDataRef.SiegeEngines [j].Cost) {
                    shopSiegeArray [j].interactable = false;
                    Debug.Log ("You cannot afford " + m_gameDataRef.SiegeEngines [j].name);
                }
                else
                    shopSiegeArray[j].interactable = true;
						
        }


        // Assigns a unit of a certain type to your army
        // Removes the units cost from the active players reserve 
        public void PurchaseUnits (string UnitType)
        {

            switch (UnitType) {
                case "Spearmen":
                {
                    Debug.Log ("Button Clicked - Spearmen");
                    m_stateManagerRef.PhasingPlayer.AddToArmy (m_gameDataRef.Soldiers [0], true);
                    m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Soldiers [0].Cost);
                    Debug.Log ("Gold Remaining:" + m_stateManagerRef.PhasingPlayer.Gold);
                    SetUnitNumbers ();
                    break;
                }

                case "Archers":
                {
                    Debug.Log ("Button Clicked - Archers");	
                    m_stateManagerRef.PhasingPlayer.AddToArmy (m_gameDataRef.Soldiers [1], true);
                    m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Soldiers [1].Cost);
                    Debug.Log ("Gold Remaining:" + m_stateManagerRef.PhasingPlayer.Gold);
                    SetUnitNumbers ();
                    break;
                }	
		
                case "Knights":
                {
                    Debug.Log ("Button Clicked - Knights");	
                    m_stateManagerRef.PhasingPlayer.AddToArmy (m_gameDataRef.Soldiers [2], true);
                    m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.Soldiers [2].Cost);
                    Debug.Log ("Gold Remaining:" + m_stateManagerRef.PhasingPlayer.Gold);
                    SetUnitNumbers ();	
                    break;
                }

                case "Ram":
                {
                    Debug.Log ("Button Clicked - Ram");	
                    m_stateManagerRef.PhasingPlayer.AddToArmy (m_gameDataRef.SiegeEngines [0], true);
                    m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.SiegeEngines [0].Cost);
                    Debug.Log ("Gold Remaining:" + m_stateManagerRef.PhasingPlayer.Gold);
                    SetUnitNumbers ();	
                    break;
                }
		

	
                case "Belfry":
                {
                    Debug.Log ("Button Clicked - Belfry");	
                    m_stateManagerRef.PhasingPlayer.AddToArmy (m_gameDataRef.SiegeEngines [1], true);
                    m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.SiegeEngines [1].Cost);
                    Debug.Log ("Gold Remaining:" + m_stateManagerRef.PhasingPlayer.Gold);
                    SetUnitNumbers ();	
                    break;
                }
		

                case "Artillery":
                {
                    Debug.Log ("Button Clicked - Artillery");	
                    m_stateManagerRef.PhasingPlayer.AddToArmy (m_gameDataRef.SiegeEngines [2], true);
                    m_stateManagerRef.PhasingPlayer.AddToGold (-m_gameDataRef.SiegeEngines [2].Cost);
                    Debug.Log ("Gold Remaining:" + m_stateManagerRef.PhasingPlayer.Gold);
                    SetUnitNumbers ();
                    break;
                }
            }
            m_stateManagerRef.PhasingPlayer.CountArmyUnits ();
        }
    }
}
			
			

	
					

			
			
		
		
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Scripts
{
    /// <summary>
    /// This is the main player class.
    /// </summary>
    /// 
    public class Player : MonoBehaviour
    {
        public enum AIPriority
        {
            NotAI,
            Military,
            Popularity,
            Religion
        }

        // VARIABLES
        private GameObject m_gameManager;
        private GameData m_gameDataRef;
        private bool m_isActive;

        public bool IsActive
        { get { return m_isActive; } }

        private bool m_isHuman;

        public bool IsHuman
        { get { return m_isHuman; } }

        private AIPriority m_aiPriority;

        public AIPriority aiPriority
        { get { return m_aiPriority; } }
	
        private int m_gold;

        public int Gold
        { get { return m_gold; } }

        private int m_goldPerTurn;
	
        public int GoldPerTurn
        { get { return m_goldPerTurn; } }

        private int m_influence;

        public int Influence
        { get { return m_influence; } }

        private int m_piety;

        public int Piety
        { get { return m_piety; } }

        private int m_popularity;

        public int Popularity
        { get { return m_popularity; } }

        private int m_startingStatValue = 5;
        public int StartingStatValue
        { get { return m_startingStatValue; } }

        public Faction m_myFaction;

        public Faction MyFaction
        { get { return m_myFaction; } }

        private int m_myFactionIndex;

        public int MyFactionIndex
        { get { return m_myFactionIndex; } }

        private int m_myPlayerIndex;

        public int MyPlayerIndex
        { get { return m_myPlayerIndex; } }

        private int m_myPlayerScore;

        public int MyPlayerScore
        { get { return m_myPlayerScore; } }

        public List<string> regionsOwned;
        public List<Castle> myCastles;
        public List<CardMilitary> myArmy;
        public int[] myArmyUnitTotals;
        private int m_spearmenCount;
        private int m_archersCount;
        private int m_knightsCount;
        private int m_ramsCount;
        private int m_belfriesCount;
        private int m_artilleryCount;
        public List<Area> myEmpire;
        private Player m_lastAttackedByPlayer;

        public Player LastAttackedByPlayer
        { get { return m_lastAttackedByPlayer; } }


        // On Startup
        void Awake ()
        {
            m_gameManager = GameObject.Find ("GameManager");
            m_gameDataRef = m_gameManager.GetComponent<GameData> (); 

        }

        // Use this for initialization
        void Start ()
        {	
        }
	
        // Update is called once per frame
        void Update ()
        {
            // Show debug messages
            //ShowDebug ();
	
        }

        // Use this for Debug logs
        void ShowDebug ()
        {
            Debug.Log ("" + this.name + " is human? " + this.m_isHuman);
            //Debug.Log ("" + this.name + "'s faction is: " + this.m_myFaction.name);

            if (m_isActive == true)
                Debug.Log ("" + this.name + " is active.");
        }
		
        // This sets the player's index number
        public void SetPlayerIndex (int index)
        {
            m_myPlayerIndex = index;
        }

        // This sets the default active / inactive values
        public void SetActiveDefaults ()
        {
            if ((this.name == "Player1") || (this.name == "Player2")) {
                m_isActive = true;
            } else {
                m_isActive = false;
            }
        }

        // This sets the isActive bool to the new value
        public void SetActiveTo (bool newValue)
        {
            m_isActive = newValue;
            Debug.Log ("Set " + this.name + "'s isactive value to" + m_isActive);
        }

        // This sets the isActive bool to the new value
        public void SetIsHumanTo (bool newValue)
        {
            m_isHuman = newValue;
            Debug.Log ("Set " + this.name + "'s isHuman value to" + m_isActive);
        }

        // This sets the default controller
        public void SetControlDefaults ()
        {
            // all players except player one default to being non-human (AI)
            if (this.name != "Player1") {
                m_isHuman = false;
            } else {
                m_isHuman = true;
            }
        }

        // This sets the player's faction
        public void SetFaction (Faction faction, int index)
        {
            if (faction != null) {
                // If the player has no faction
                if (m_myFaction == null) {
                    // Set faction and faction index
                    m_myFaction = faction;
                    m_myFactionIndex = index;
                    // If the player is active
                    if (m_isActive == true) {
                        // The faction is in play
                        m_myFaction.SetInPlay (true);
                        // The faction is being controlled by this player
                        m_myFaction.SetControllingPlayerIndex (m_myPlayerIndex);
                    }
                    // If the player already has a faction
                } else {
                    // The current faction is no longer in play
                    m_myFaction.SetInPlay (false);
                    // The current faction's controlling player index is 0
                    m_myFaction.SetControllingPlayerIndex (0);
                    // Set new faction and faction index
                    m_myFaction = faction;
                    m_myFactionIndex = index;
                    // The new faction is in play
                    m_myFaction.SetInPlay (true);
                    // The new faction is being controlled by this player
                    m_myFaction.SetControllingPlayerIndex (m_myPlayerIndex);
                }
            } else
                m_myFaction = faction;
        }


        // This handles the effect of changing the active toggle in player selection
        public void activeToggleChanged ()
        {
            // If the player is active, set to inactive.
            if (m_isActive == true) {
                m_isActive = false;
                m_myFaction.SetInPlay (false);
                m_myFaction = null;
                m_myFactionIndex = 0;
            }

            // If the player is inactive, make active. 
            else {
                m_isActive = true;
            }
        }

        // This handles the effect of changing the human toggle in player selection
        public void humanToggleChanged ()
        {
            // If the player is human, set to AI.
            // If the player is not human, make human. 
            if (m_isHuman == true) {
                m_isHuman = false;
            } else {
                m_isHuman = true;
            }
        }

        // Assigns player's starting stats
        public void AssignStartStats()
        {
            m_piety = m_startingStatValue;
            m_popularity = m_startingStatValue;
        }

        // Assigns a random priority
        public void AssignAIPriority ()
        {
            if (IsHuman)
                m_aiPriority = AIPriority.NotAI;
            else {
                int rand = 0;
                rand = Random.Range (1, 4);
                switch (rand) {
                    case 1:
                    {
                        m_aiPriority = AIPriority.Military;
                        break;
                    }
                    case 2:
                    {
                        m_aiPriority = AIPriority.Popularity;

                        break;
                    }
                    case 3:
                    {
                        m_aiPriority = AIPriority.Religion;
                        break;
                    }
                }
            }
            Debug.Log (""+this.name +"'s AI Priority is " +m_aiPriority);
        }

        public void SetAIPriority(AIPriority newAIpriority)
        {
            m_aiPriority = newAIpriority;
            Debug.Log (""+this.name+"'s AI priority is "+m_aiPriority);
        }

        // This adds a new card to the player's army
        public void AddToArmy (CardMilitary newCard, bool isNewUnit)
        {
            // If this is a new unit
            if (isNewUnit) {
                // Add a duplicate of the new card prefab to the player's army
                //Debug.Log ("Adding a " + newCard.myMilitaryType + " to " + this.name);
                myArmy.Add (Instantiate (newCard) as CardMilitary);
                //Debug.Log ("myArmy[" + (myArmy.Count - 1) + "] is a " + myArmy [myArmy.Count - 1].myMilitaryType);
                // Make the added card a child of this player's army game object
                myArmy [myArmy.Count - 1].transform.parent = GameObject.Find ("" + this.name + "Army").transform;
            } else { 
                // If this unit already exists
                // Add to the player's army
                myArmy.Add (newCard);
                // Make it a child of the player's army game object
                newCard.transform.parent = GameObject.Find ("" + this.name + "Army").transform;
            }
            CountArmyUnits ();

        }


        // This removes a card of the specified type from the player's army list 
        // It does not remove the card object from the player
        // This is used in combat when armies are committed to battle but have not yet fought
//	public void RemoveFromArmyList (CardMilitary.MilitaryType cardType)
//	{
//		for (int i = 0; i < myArmy.Count; i++) {
//			if (myArmy [i].myMilitaryType == cardType) {
//				myArmy.RemoveAt (i);
//				break;
//			}
//		}
//	}

        // This adds a card of the specified type to the player's army list
        // It does not create a new card object
        // This is used in combat when armies are committed to battle but have not yet fought
        public void AddToArmyList (CardMilitary cardRef)
        {
            myArmy.Add (cardRef);
        }

        // This is called whenever a new total of the number of units of each type the player has
        public void CountArmyUnits ()
        {
            // reset all the counts
            m_spearmenCount = 0;
            m_archersCount = 0;
            m_knightsCount = 0;
            m_ramsCount = 0;
            m_belfriesCount = 0;
            m_artilleryCount = 0;

            // For every unit of the specified type in the army, increase the count for that type by one
            for (int i = 0; i < myArmy.Count; i++) {
                //Debug.Log ("Type of " + i + " is: " + myArmy [i].myMilitaryType);
                if (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Spearmen)
                    m_spearmenCount++;
                if (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Archers)
                    m_archersCount++;
                if (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Knights)
                    m_knightsCount++;
                if (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Ram)
                    m_ramsCount++;
                if (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Belfry)
                    m_belfriesCount++;
                if (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Artillery)
                    m_artilleryCount++;
            }

            // Set the new totals based on the new counts
            myArmyUnitTotals = new int[6] {
                m_spearmenCount,
                m_archersCount,
                m_knightsCount,
                m_ramsCount,
                m_belfriesCount,
                m_artilleryCount
            };

        }

        public int CountSiegeEngines ()
        {
            // Refresh the army unit count
            CountArmyUnits ();
            return m_ramsCount + m_belfriesCount + m_artilleryCount;
        }

        public int CountSoldiers ()
        {
            // Refresh the army unit count
            CountArmyUnits ();
            return m_spearmenCount + m_archersCount + m_knightsCount;
        }
		
        // This adds a new castle to the player's castle list
        public void AddNewCastle (Castle newCastle, CardMilitary newCard)
        {
            // Add a duplicate of the castle prefab to the player's castle list
            myCastles.Add (Instantiate (newCastle) as Castle);
            // Make the new castle a child of the player's castles game object
            myCastles [myCastles.Count - 1].transform.parent = GameObject.Find ("" + this.name + "Castles").transform;
            // All castles start with a keep, which is added to the castle
            myCastles [myCastles.Count - 1].AddDefence (newCard);
        }
		
        // Add the given area to my empire
        public void AddToEmpire (Area newArea)
        {
            Debug.Log ("adding to empire");
            myEmpire.Add (newArea);
            newArea.SetControllingPlayer (this);
            m_influence++;
        }

        // Add the given area to my empire
        public void AddToEmpireLoad (Area newArea)
        {
            Debug.Log ("adding to empire from load");
            myEmpire.Add (newArea);
            newArea.SetControllingPlayer (this);
        }

        // Adds popularity to this player
        public void AddToGold (int newGold)
        {
            m_gold += newGold;
        }

        // Adds popularity  to this player
        public void AddToPopularity (int newPopularity)
        {
            m_popularity += newPopularity;
        }

        // Adds faith to this player
        public void AddToPiety (int newPiety)
        {
            m_piety += newPiety;
        }

        // Adds influence to this player
        public void AddToInfluence (int newInfluence)
        {
            m_influence += newInfluence;
        }
	
        // Usually called when the player loses control of a area
        public void LoseInfluence ()
        {
            if (m_influence > 0)
                m_influence--;
        }

        // Returns true if the player has any siege engines in their army
        // Usually this is used to check whether they are eligible to initiate a siege
        public bool HasSiegeEngines ()
        {
            for (int i = 0; i < myArmy.Count; i++) {
                if ((myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Artillery) || (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Belfry) || (myArmy [i].myMilitaryType == CardMilitary.MilitaryType.Ram))
                    return true;
            }
            return false;

        }

        // Set Last Attacked By 
        public void SetLastAttackedBy (Player attackedByPlayer)
        {
            m_lastAttackedByPlayer = attackedByPlayer;
        }

        /// <summary>
        /// Check if Player owns a region
        /// </summary>
        public void CheckRegions ()
        {
            Debug.Log ("Checking regions");
            for (int i = 0; i < myEmpire.Count; i++) {	
                m_gameDataRef.CheckRegionControl (this, myEmpire [i].tag);
                if (m_gameDataRef.hasControl == true) {										
                    if (!regionsOwned.Contains (myEmpire [i].tag)) {
                        Debug.Log (this.name + " has control of " + myEmpire [i].tag + "!");	
                        regionsOwned.Add (myEmpire [i].tag);

                    }
                } else if (m_gameDataRef.hasControl == false) {
                    if (regionsOwned.Contains (myEmpire [i].tag)) {
                        Debug.Log (this.name + " has lost control of " + myEmpire [i].tag + "!");	
                        regionsOwned.Remove (myEmpire [i].tag);
                    }
                }
            }
        }
	
        // Calculates the tax to be collected
        public void CalculateTax ()
        {
            m_goldPerTurn = 0;
            // Set the amount to be collected to the player's influence
            m_goldPerTurn = Influence;
            // Add 5 gold per castle the player has
            m_goldPerTurn += (5 * myCastles.Count);
            // Add 5 gold for each complete region controlled by player
            m_goldPerTurn += regionsOwned.Count * 5;
            // If the player is not human
            if (!IsHuman) {
                switch (m_gameDataRef.GameAIDifficulty) {
                    case GameData.AIDifficulty.Easy:
                    {
                        m_goldPerTurn -= m_gameDataRef.EasyAIGoldModifier;
                        break;
                    }
                    case GameData.AIDifficulty.Standard:
                    {
                        m_goldPerTurn -= m_gameDataRef.StandardAIGoldModifier;
                        break;
                    }
                    case GameData.AIDifficulty.Difficult:
                    {
                        m_goldPerTurn -= m_gameDataRef.DifficultAIGoldModifier;
                        break;
                    }
                }
            }
        }

        // Set all player stats to 0
        public void WipePlayerStats ()
        {
            m_gold = 0;
            m_influence = 0;
            m_piety = 0;
            m_popularity = 0;
            m_myFaction = null;
            m_myFactionIndex = 0;
        }

        // Total stats to get player score
        public void PlayerScore ()
        {
            m_myPlayerScore = Influence + Piety + Popularity;

        }



	 
    }
}

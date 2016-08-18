using System;
using System.Collections.Generic;
using UnityEngine;

//makes the class serializable 
namespace Assets.Code.Scripts
{
    public class GameData : MonoBehaviour
    {
        public enum VictoryState
        {
            None,
            Domination,
            GameEnd,
            HumansDestroyed
        }

        public enum AIDifficulty
        {
            Easy,
            Standard,
            Difficult
        }

        private VictoryState m_gameVictoryState;

        public VictoryState GameVictoryState
        { get { return m_gameVictoryState; } }

        private AIDifficulty m_gameAIDifficulty;

        public AIDifficulty GameAIDifficulty
        { get { return m_gameAIDifficulty; } }

        private int m_easyAIGoldModifier = 3;

        public int EasyAIGoldModifier
        { get { return m_easyAIGoldModifier; } }
	
        private int m_standardAIGoldModifier = 1;

        public int StandardAIGoldModifier
        { get { return m_standardAIGoldModifier; } }
	
        private int m_difficultGoldModifier = 0;

        public int DifficultAIGoldModifier
        { get { return m_difficultGoldModifier; } }

        private int m_lastGameTurn;
	
        public int LastGameTurn
        { get { return m_lastGameTurn; } }

        private int m_shortGameLength = 10;

        public int ShortGameLength
        { get { return m_shortGameLength; } }
	
        private int m_mediumGameLength = 20;

        public int MediumGameLength
        { get { return m_mediumGameLength; } }
	
        private int m_longGameLength = 30;

        public int LongGameLength
        { get { return m_longGameLength; } }
	
        private SaveLoad m_saveLoad;

        public SaveLoad SaveLoad
        { get { return m_saveLoad; } }

        // Players
        private int m_noHumanPlayers;
        private int m_noComputerPlayers;
        public Player[] players;
        public List<Player> activePlayers;
		
        // Factions
        public Faction[] factions;
        public Area[] startingAreas;
        private Vector2[] startingAreaPositions;

        // Flag sprites
        public Sprite[] flagSprites;

        // UI sprites
        public Sprite UIBackground;

        // Animation
        private bool m_battleAnimationsEnabled;

        public bool BattleAnimationsEnabled
        { get { return m_battleAnimationsEnabled; } }

        // Prefab References
        public CardMilitary militaryCard;
        public CardPapal papalCard;
        public CardPolitics politicsCard;
        public Castle castle;
        public DeckPolitics deckPolitics;
        public DeckPolitics discardDeckPolitics;
        public DeckPapal deckPapal;
		
        // Card collections
        private CardMilitary[] m_soldiers;

        public CardMilitary[] Soldiers
        { get { return m_soldiers; } }

        private CardMilitary[] m_siegeEngines;

        public CardMilitary[] SiegeEngines
        { get { return m_siegeEngines; } }
	
        private CardMilitary[] m_defences;

        public CardMilitary[] Defences
        { get { return m_defences; } }

        //creates card arrays for court scene 
        private CardPapal[] m_papalBull;
        private CardPolitics[] m_politics;
		
        // Map Collections
        public Area[] AllAreas;
        private Area[] m_britain;
        private Area[] m_france;
        private Area[] m_hispania;
        private Area[] m_italy;
        private Area[] m_hungary;
        private Area[] m_germany;
        private Area[] m_kievanRus;
        private Area[] m_byantines;
        private Area[] m_denmark;

        //Region Bool
        public bool hasControl;

        // Map Region Collection Iterator
        private int[] m_mapIterators;
        //called BEFORE START
        void Awake ()
        {
            deckPolitics = GetComponent<DeckPolitics> (); 
            deckPapal = GetComponent<DeckPapal> (); 
            m_saveLoad = GetComponent<SaveLoad> ();

            Debug.Log ("Save file location is" + Application.persistentDataPath.ToString ());
        }

        // Use this for initialization
        void Start ()
        {
            m_gameVictoryState = VictoryState.None;
            m_battleAnimationsEnabled = true;
            m_gameAIDifficulty = AIDifficulty.Standard;
            m_lastGameTurn = m_shortGameLength;
            Debug.Log ("Last game turn is " +m_lastGameTurn);

            // Set Default active and control values for players
            for (int i = 0; i < players.Length; i++) {
                players [i].SetControlDefaults ();
                players [i].SetActiveDefaults ();
                players [i].SetPlayerIndex (i);
                // See start menu manager for default assignment of factions
            }

            // Set Default values for factions
            for (int i = 0; i < factions.Length; i++) {
                factions [i].SetFactionIndex (i);
                factions [i].SetFactionFlag (flagSprites [i]);
                factions [i].SetFactionDescription (i);
            }
		
            // Initialise the soldiers
            m_soldiers = new CardMilitary[3];
            // Assign blank military card as a child of the military game object
            for (int i = 0; i < m_soldiers.Length; i++) {
                m_soldiers [i] = Instantiate (militaryCard)as CardMilitary;
                m_soldiers [i].transform.parent = GameObject.Find ("Military").transform;
            }
            // Set the values of the soldier military cards
            m_soldiers [0].AssignValues (CardMilitary.MilitaryType.Spearmen);
            m_soldiers [1].AssignValues (CardMilitary.MilitaryType.Archers);
            m_soldiers [2].AssignValues (CardMilitary.MilitaryType.Knights);

            // Initialise the siege engines
            m_siegeEngines = new CardMilitary[3];
            // Assign blank military card as a child of the military game object
            for (int i = 0; i < m_siegeEngines.Length; i++) {
                m_siegeEngines [i] = Instantiate (militaryCard)as CardMilitary;
                m_siegeEngines [i].transform.parent = GameObject.Find ("Military").transform;
            }
            // Set the values of the siege engine military cards
            m_siegeEngines [0].AssignValues (CardMilitary.MilitaryType.Ram);
            m_siegeEngines [1].AssignValues (CardMilitary.MilitaryType.Belfry);
            m_siegeEngines [2].AssignValues (CardMilitary.MilitaryType.Artillery);

            // Initialise the defences	
            m_defences = new CardMilitary[3];
            // Assign blank military card as a child of the military game object
            for (int i = 0; i < m_defences.Length; i++) {
                m_defences [i] = Instantiate (militaryCard)as CardMilitary;
                m_defences [i].transform.parent = GameObject.Find ("Military").transform;
            }
            // Set the values of the defence military cards
            m_defences [0].AssignValues (CardMilitary.MilitaryType.Defence);
            m_defences [1].AssignValues (CardMilitary.MilitaryType.Wall);
            m_defences [2].AssignValues (CardMilitary.MilitaryType.Keep);
		
            //this initialises both card decks, calling the InitDeck Method in the respective scripts
            deckPolitics.InitDeck (); 
            deckPapal.InitDeck (); 

            // Initialise and populate starting area map positions collection
            // (This allows the map to move to the relevant position dependant on phasing player's faction)
            startingAreaPositions = new Vector2[8] {
                new Vector2 (470, -650),
                new Vector2 (440, -90),
                new Vector2 (-310, 325),
                new Vector2 (850, 420),
                new Vector2 (-750, -90),
                new Vector2 (-1030, 390),
                new Vector2 (-1030, -650),
                new Vector2 (-160, -630)
            };
            // Assign Starting Areas
            for (int i = 0; i< factions.Length; i++) {
                factions [i].AssignStartingArea (startingAreas [i], startingAreaPositions [i]);
            }
	
            // Assign Area Indexes
            for (int i = 0; i < AllAreas.Length; i++) {
                AllAreas [i].SetAreaIndex (i);
            }

            // Initialise the Map Regions
            m_britain = new Area[6];
            m_france = new Area[7];
            m_germany = new Area[6];
            m_hispania = new Area[7];
            m_italy = new Area[6];
            m_denmark = new Area[6];
            m_kievanRus = new Area[7];
            m_hungary = new Area[7];
            m_byantines = new Area[7];
				
            // Initialise the Map region iterator
            m_mapIterators = new int[9];
            // Set all map region iterators to 0 
            for (int i = 0; i < m_mapIterators.Length; i++) {
                m_mapIterators [i] = 0;
            }
				
            // For all areas...
            for (int i = 0; i < AllAreas.Length; i++) {
                // Assign area to the relevant map region determined by the area's tag
                // Each region uses a different map iterator, which increases on each assignation
                switch (AllAreas [i].tag) {
                    case "Britain":
                    {
                        m_britain [m_mapIterators [0]] = AllAreas [i];
                        m_mapIterators [0]++;
                        break;}
                    case "France":
                    {
                        m_france [m_mapIterators [1]] = AllAreas [i];
                        m_mapIterators [1]++;
                        break;}
                    case "Germany":
                    {
                        m_germany [m_mapIterators [2]] = AllAreas [i];
                        m_mapIterators [2]++;
                        break;}
                    case "Hispania":
                    {
                        m_hispania [m_mapIterators [3]] = AllAreas [i];
                        m_mapIterators [3]++;
                        break;}
                    case "Italy":
                    {
                        m_italy [m_mapIterators [4]] = AllAreas [i];
                        m_mapIterators [4]++;
                        break;}
                    case "Denmark":
                    {
                        m_denmark [m_mapIterators [5]] = AllAreas [i];
                        m_mapIterators [5]++;
                        break;}
                    case "Kievan Rus'":
                    {
                        m_kievanRus [m_mapIterators [6]] = AllAreas [i];
                        m_mapIterators [6]++;
                        break;}
                    case "Hungary":
                    {
                        m_hungary [m_mapIterators [7]] = AllAreas [i];
                        m_mapIterators [7]++;
                        break;}
                    case "Byzantine Empire":
                    {
                        m_byantines [m_mapIterators [8]] = AllAreas [i];
                        m_mapIterators [8]++;
                        break;}
                }
            }
        }

        // Update is called once per frame
        void Update ()
        {
            //Debug.Log ("No. of players: " + players.Length);

        }

        // Resets the players to the state they were in before the game started 
        public void ResetPlayers ()
        {
            // For all of the players
            for (int i = 0; i < players.Length; i++) {	
                if (players [i].IsActive) {
                    RemoveActivePlayer (players [i]);	
                }
                // Set their active and control default as happened at the start of the game
                players [i].SetActiveDefaults ();
                players [i].SetControlDefaults ();
                // Wipe the player's stats
                players [i].WipePlayerStats ();
            }
        }

        // Removes all occupying units from areas
        public void ResetAreas ()
        {
            for (int i = 0; i < AllAreas.Length; i++) {
                AllAreas [i].ClearArea ();
            }
        }

        // This sets up the list of active players
        public void SetActivePlayers ()
        {
            // For every player, check whether they are active and, if so, add them to the active player list
            for (int i = 0; i < players.Length; i++) {
                if (players [i].IsActive == true) {
                    activePlayers.Add (players [i]);
								
                }
            }
        }

        public void RemoveActivePlayer (Player playerToRemove)
        {
            for (int i = 0; i < activePlayers.Count; i++) {
                if (activePlayers [i] == playerToRemove) {

                    // Destroy their army
                    for (int j = 0; j < activePlayers[i].myArmy.Count; j++) {
                        activePlayers [i].myArmy [j].DestroyUnit ();
                    }
                    activePlayers [i].myArmy.Clear ();
                    activePlayers [i].CountArmyUnits ();
                    // Remove all of the areas in their empire
                    for (int k = 0; k < activePlayers[i].myEmpire.Count; k++) {
                        activePlayers [i].myEmpire [k].SetControllingPlayer (null);
                    }
                    activePlayers [i].myEmpire.Clear ();

                    // Remove all the player's castles
                    if (activePlayers [i].myCastles.Count > 0) {
                        for (int l = 0; l < activePlayers[i].myCastles.Count; l++) {
                            activePlayers [i].myCastles [l].DestroyCastle ();
                        }
                        activePlayers [i].myCastles.Clear ();
                    }
                    // Remove the active player from their faction
                    activePlayers [i].MyFaction.RemovePlayerFromFaction ();
                    activePlayers.RemoveAt (i);

                    Debug.Log ("" + playerToRemove + "has been removed.");
                }
            }
        }

        // This sorts the active players by influence
        public void SortActivePlayers ()
        {
            activePlayers.Sort ((p1, p2) => p1.Influence.CompareTo (p2.Influence));
        }

        // This sets the starting armies for the active players
        public void SetStartingArmies ()
        {
            // For each active player, add the following to their army from the soldier list
            for (int i = 0; i < activePlayers.Count; i++) {
                activePlayers [i].AddToArmy (m_soldiers [0], true);
                activePlayers [i].AddToArmy (m_soldiers [0], true);
                //activePlayers [i].AddToArmy (m_soldiers [0], true);
                activePlayers [i].AddToArmy (m_soldiers [1], true);
                //activePlayers [i].AddToArmy (m_soldiers [1], true);
                //activePlayers [i].AddToArmy (m_soldiers [2], true);

                // For testing only, adding some siege engines
                //activePlayers [i].AddToArmy (m_siegeEngines [0], true);
                activePlayers [i].CountArmyUnits ();
            }
        }

        // This sets the starting armies for the active players
        public void SetStartingCastles ()
        {
            // For each active player, add a castle to their castle list
            for (int i = 0; i < activePlayers.Count; i++) {
                activePlayers [i].AddNewCastle (castle, m_defences [2]);
                // activePlayers [i].mycastles [0].AddDefence (m_defences [1]);

                // FOR DEBUGGING - extra castle to player 1
                //activePlayers[1].AddNewcastle(castle, m_defences[2]);
            }
        }

        // This sets the starting empires for the active players
        public void SetStartingEmpires ()
        {
            // For each active player, 
            for (int i = 0; i < activePlayers.Count; i++) {
                // Add their faction's starting area to their empire list
                activePlayers [i].AddToEmpire (activePlayers [i].MyFaction.StartingArea);
                // Add an occupying army to the starting area
                activePlayers [i].MyFaction.StartingArea.AssignOccupyingUnit (m_soldiers [0], true);
                activePlayers [i].MyFaction.StartingArea.AssignOccupyingUnit (m_soldiers [1], true);
                activePlayers [i].MyFaction.StartingArea.AssignOccupyingUnit (m_soldiers [2], true);
                // Assign the player's castle to the starting area
                activePlayers [i].MyFaction.StartingArea.AssignCastle (activePlayers [i].myCastles [0]);
            }
            // FOR TESTING ONLY:
            // ASSIGN BRITTANY TO PLAYER 0
            //activePlayers [0].AddToEmpire (AllAreas [6]);
            // ASSIGN SOME OTHER AREAS TO PLAYER 1
            //activePlayers [1].AddToEmpire (AllAreas [8]);
            //activePlayers [1].AddToEmpire (AllAreas [9]);
        }

        // Sets the player's default popularity and piety
        public void SetStartingStats()
        {
            for (int i = 0; i < activePlayers.Count; i++) {
                activePlayers[i].AssignStartStats();
            }
        }

        // Sets the AI priority for all active players
        public void SetAIPriority()
        {
            for (int i = 0; i < activePlayers.Count; i++) {
                // Assign the AI priority
                activePlayers[i].AssignAIPriority();

            }
        }

//	// This sets the starting gold for the active players
//	public void SetStartingGold ()
//	{
//		// For each active player,
//		for (int i = 0; i < activePlayers.Count; i++) {
//			// Add 5 gold 
//			activePlayers [i].AddToGold (5);
//			Debug.Log ("" + activePlayers [i].name + " has " + activePlayers [i].Gold + " gold.");
//		}
//	}

        // This returns true if the referenced player controls all areas in the referenced region
        // It is usually called to help the AI prioritise which area to target
        //and to determine if a player owns a region
        public bool CheckRegionControl (Player checkPlayer, string checkRegion)
        {
            // temporary bool to store whether or not the player has control so far... 
            hasControl = false;

            // Depending on the region, check through each area referenced in the region
            // If the player controls the area, set 'has Control' to true and move on to check the next area
            // If the player doesn't control the area, set 'has Control' to false break out of the check
            // Once the check has completed or been broken out of, break out of the case
            switch (checkRegion) {
                case "Britain":
                {
                    for (int i = 0; i < m_britain.Length; i++) {
                        if ((m_britain [i].ControllingPlayer != null) && (m_britain [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "France":
                {
                    for (int i = 0; i < m_france.Length; i++) {
                        if ((m_france [i].ControllingPlayer != null) && (m_france [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "Germany":
                {
                    for (int i = 0; i < m_germany.Length; i++) {
                        if ((m_germany [i].ControllingPlayer != null) && (m_germany [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "Hispania":
                {
                    for (int i = 0; i < m_hispania.Length; i++) {
                        if ((m_hispania [i].ControllingPlayer != null) && (m_hispania [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "Italy":
                {
                    for (int i = 0; i < m_italy.Length; i++) {
                        if ((m_italy [i].ControllingPlayer != null) && (m_italy [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "Denmark":
                {
                    for (int i = 0; i < m_denmark.Length; i++) {
                        if ((m_denmark [i].ControllingPlayer != null) && (m_denmark [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "Kievan Rus'":
                {
                    for (int i = 0; i < m_kievanRus.Length; i++) {
                        if ((m_kievanRus [i].ControllingPlayer != null) && (m_kievanRus [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "Hungary":
                {
                    for (int i = 0; i < m_hungary.Length; i++) {
                        if ((m_hungary [i].ControllingPlayer != null) && (m_hungary [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
                case "Byzantine Empire":
                {
                    for (int i = 0; i < m_byantines.Length; i++) {
                        if ((m_byantines [i].ControllingPlayer != null) && (m_byantines [i].ControllingPlayer == checkPlayer))
                            hasControl = true;
                        else {
                            hasControl = false;
                            break;
                        }
                    }
                    break;
                }
            }
            // Once the case check has been run, return true or false depending on the value of 'hasControl'
            if (hasControl == true)
                return true;
            else
                return false;
        }

        public bool CheckRegionPresence (Player checkPlayer, string checkRegion)
        {
            // temporary bool to store whether or not the player has presence so far... 
            bool hasPresence = false;
		
            // Depending on the region, check through each area referenced in the region
            // If the player controls any area in the region, set 'has Presence' to true and break out of the check
            // If the player doesn't control any area in the region, set 'has Presence' to false and move on to check the next area
            // Once the check has completed or been broken out of, break out of the case
            switch (checkRegion) {
                case "Britain":
                {
                    for (int i = 0; i < m_britain.Length; i++) {
                        if (m_britain [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "France":
                {
                    for (int i = 0; i < m_france.Length; i++) {
                        if (m_france [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "Germany":
                {
                    for (int i = 0; i < m_germany.Length; i++) {
                        if (m_germany [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "Hispania":
                {
                    for (int i = 0; i < m_hispania.Length; i++) {
                        if (m_hispania [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "Italy":
                {
                    for (int i = 0; i < m_italy.Length; i++) {
                        if (m_italy [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "Denmark":
                {
                    for (int i = 0; i < m_denmark.Length; i++) {
                        if (m_denmark [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "Kievan Rus":
                {
                    for (int i = 0; i < m_kievanRus.Length; i++) {
                        if (m_kievanRus [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "Hungary":
                {
                    for (int i = 0; i < m_hungary.Length; i++) {
                        if (m_hungary [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
                case "Byzantine Empire":
                {
                    for (int i = 0; i < m_byantines.Length; i++) {
                        if (m_byantines [i].ControllingPlayer == checkPlayer) {
                            hasPresence = true;
                            break;
                        }
                    }
                    break;
                }
            }
            // Once the case check has been run, return true or false depending on the value of 'hasControl'
            if (hasPresence == true)
                return true;
            else
                return false;
        }

        public bool CheckIfHomeRegion (Player checkPlayer, Area checkArea, String checkRegion)
        {
            // temporary bool to store whether or not the area is in the player's home region 
            bool isHomeRegion = false;
		
            // Depending on the region, check through each area in the referenced region
            // If the referenced area is in the region, set 'isHomeRegion' to true and break out of the check
            // If the referenced area is in the region, set 'isHomeRegion' to false and move on to check the next area
            // Once the check has completed or been broken out of, break out of the case
            switch (checkPlayer.MyFaction.name) {
                case "England":
                {
                    for (int i = 0; i < m_britain.Length; i++) {
                        if (m_britain [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
                case "France":
                {
                    for (int i = 0; i < m_france.Length; i++) {
                        if (m_france [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
                case "Holy Roman Empire":
                {
                    for (int i = 0; i < m_italy.Length; i++) {
                        if (m_italy [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
                case "Spain":
                {
                    for (int i = 0; i < m_hispania.Length; i++) {
                        if (m_hispania [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
                case "Hungary":
                {
                    for (int i = 0; i < m_hungary.Length; i++) {
                        if (m_hungary [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
                case "Byzantine Empire":
                {
                    for (int i = 0; i < m_byantines.Length; i++) {
                        if (m_byantines [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
                case "Poland":
                {
                    for (int i = 0; i < m_kievanRus.Length; i++) {
                        if (m_kievanRus [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
		
                case "Denmark":
                {
                    for (int i = 0; i < m_denmark.Length; i++) {
                        if (m_denmark [i] == checkArea) {
                            isHomeRegion = true;
                            break;
                        }
                    }
                    break;
                }
            }
            // Once the case check has been run, return true or false depending on the value of 'isHomeRegion'
            if (isHomeRegion == true)
                return true;
            else
                return false;

        }

        public int CountCastles ()
        {
            int castlesCount = 0;
            for (int i = 0; i < AllAreas.Length; i++) {
                if (AllAreas [i].occupyingCastle != null)
                    castlesCount++;
            }
            Debug.Log ("There are " + castlesCount + " castles in the game"); 
            return castlesCount;
        }

        // This updates game data from the loaded save data file
        public void UpdateFromLoadedSaveData (int[,] playerStats, int[,] playerArmies, int[] loadedActivePlayers, int[,] areaStats, int[,] castleData, int[] gameSettings, string[] resolvedPoliticalCards, string[] resolvedPapalCards)
        {
            m_gameVictoryState = VictoryState.None;
            // Set the player info from the player stats 
            for (int i = 0; i < players.Length; i++) {

                players [i].SetActiveTo (Convert.ToBoolean (playerStats [i, 0]));
                players [i].SetIsHumanTo (Convert.ToBoolean (playerStats [i, 1]));
                players [i].AddToGold (playerStats [i, 2]);
                players [i].AddToInfluence (playerStats [i, 3]);
                players [i].AddToPiety (playerStats [i, 4]);
                players [i].AddToPopularity (playerStats [i, 5]);
                players [i].SetFaction (factions [playerStats [i, 6]], playerStats [i, 6]);
                if (playerStats [i, 7] != 5)
                    players [i].SetLastAttackedBy (players [playerStats [i, 7]]);
                switch (playerStats[i, 8])
                {
                    case 0:
                    {
                        players[i].SetAIPriority(Player.AIPriority.NotAI);
                        break;
                    }
                    case 1:
                    {
                        players[i].SetAIPriority(Player.AIPriority.Military);

                        break;
                    }
                    case 2:
                    {
                        players[i].SetAIPriority(Player.AIPriority.Popularity);

                        break;
                    }
                    case 3:
                    {
                        players[i].SetAIPriority(Player.AIPriority.Religion);
                        break;
                    }
                }
            }

            // Set the player armies from the saved player armies
            for (int x = 0; x < players.Length; x++) {
                players [x].myArmyUnitTotals = new int[6];
                for (int y = 0; y < players[x].myArmyUnitTotals.Length; y++) {
                    players [x].myArmyUnitTotals [y] = playerArmies [x, y];
                    for (int i = 0; i < playerArmies[x, y]; i++) {
                        if (y < players [x].myArmyUnitTotals.Length / 2)
                            players [x].AddToArmy (m_soldiers [y], true);
                        else
                            players [x].AddToArmy (m_siegeEngines [y - players [x].myArmyUnitTotals.Length / 2], true);
                    }
                }
            }

            // Set active players from the saved active players
            for (int i = 0; i < loadedActivePlayers.Length; i++) {
                activePlayers.Add (players [loadedActivePlayers [i]]);
            }

            // Set the area stats from the saved area stats
            for (int i = 0; i < AllAreas.Length; i++) {
                if (areaStats [i, 0] != 5) {
                    AllAreas [i].SetControllingPlayer (players [areaStats [i, 0]]);
                    players [areaStats [i, 0]].AddToEmpireLoad (AllAreas [i]);
                }
                for (int j = 0; j < areaStats[i, 1]; j++) {
                    AllAreas [i].AssignOccupyingUnit (m_soldiers [0], true);
                }
                for (int k = 0; k < areaStats[i, 2]; k++) {
                    AllAreas [i].AssignOccupyingUnit (m_soldiers [1], true);
                }
                for (int l = 0; l < areaStats[i, 3]; l++) {
                    AllAreas [i].AssignOccupyingUnit (m_soldiers [2], true);
                }
//				if (areaStats [i, 4] != 0) {
//					AllAreas [i].ControllingPlayer.AddNewCastle (castle, Defences [2]);
//					AllAreas [i].AssignCastle (AllAreas [i].ControllingPlayer.myCastles [AllAreas [i].ControllingPlayer.myCastles.Count - 1]);
//				}
			
            }

            // Set up the castles from the castle data
            for (int x = 0; x < castleData.GetLength(0); x++) {
                // Create a new castle
                players [castleData [x, 1]].AddNewCastle (castle, Defences [2]);
                AllAreas [castleData [x, 2]].AssignCastle (players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1]);
                // add the castles defences
                for (int i = 0; i < castleData[x, 3]; i++) {
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].AddDefence (m_defences [0]);
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences [players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.WatchTower);
                }
                for (int i = 0; i < castleData[x, 4]; i++) {
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].AddDefence (m_defences [0]);
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences [players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.Ditch);
                }
                for (int i = 0; i < castleData[x, 5]; i++) {
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].AddDefence (m_defences [0]);
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences [players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.BoilingOil);
                }
                for (int i = 0; i < castleData[x, 6]; i++) {
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].AddDefence (m_defences [1]);
                    players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences [players [castleData [x, 1]].myCastles [players [castleData [x, 1]].myCastles.Count - 1].castleDefences.Count - 1].SetDefenceType (CardMilitary.DefenceType.Wall);
                }
            }

            // Assign the game settings
            // Set battle animatiuons to on or off
            SetBattleAnimationsEnabled (Convert.ToBoolean (gameSettings [0]));
            // Set game difficulty
            switch (gameSettings [1]) {
                case 0:
                {
                    SetAIDifficulty (AIDifficulty.Easy);
                    break;
                }
                case 1:
                {
                    SetAIDifficulty (AIDifficulty.Standard);
                    break;
                }
                case 2:
                {
                    SetAIDifficulty (AIDifficulty.Difficult);
                    break;
                }
            }

            // Assign the resolved political cards
            for (int i = 0; i < resolvedPoliticalCards.Length; i++) {
                for (int j = 0; j < deckPolitics.politicalCards.Count; j++)
                {
                    if (resolvedPoliticalCards[i] == deckPolitics.politicalCards[j].title)
                    {
                        deckPolitics.politicalCards[j].SetPlayed(true);
                        deckPolitics.resolvedPoliticalCards.Add (deckPolitics.politicalCards[j]);
                        deckPolitics.politicalCards.RemoveAt(j);
                    }
                }
            }

            // Assign the resolved papal cards
            for (int i = 0; i < resolvedPapalCards.Length; i++) {
                for (int j = 0; j < deckPapal.papalCards.Count; j++)
                {
                    if (resolvedPapalCards[i] == deckPapal.papalCards[j].title)
                    {
                        deckPapal.papalCards[j].SetPlayed(true);
                        deckPapal.resolvedPapalCards.Add (deckPapal.papalCards[j]);
                        deckPapal.papalCards.RemoveAt(j);
                    }
                }
            }

            // Set last game turn
            SetLastGameTurn (gameSettings [2]);
        }

        public void SetVictoryState (VictoryState newVictoryState)
        {
            m_gameVictoryState = newVictoryState;
        }

        public void SetBattleAnimationsEnabled (bool newValue)
        {
            m_battleAnimationsEnabled = newValue;
            Debug.Log ("Battle animations are on? " + m_battleAnimationsEnabled);
        }

        /// <summary>
        /// Sets the AI difficulty.
        /// </summary>
        public void SetAIDifficulty (AIDifficulty newDifficulty)
        {
            m_gameAIDifficulty = newDifficulty;
            Debug.Log ("AI Difficulty has been set to: " + m_gameAIDifficulty);
        }

        public void SetLastGameTurn (int lastGameTurn)
        {
            m_lastGameTurn = lastGameTurn;
            Debug.Log ("Last game turn has been set to: " + m_lastGameTurn);
		
        }



//	// Sets the loaded game boolean to the new value
//	public void SetLoadedGame(bool newvalue)
//	{
//		m_loadedGame = newvalue;
//	}
	
    }
}


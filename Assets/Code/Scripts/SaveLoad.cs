using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Code.Scripts
{
    public class SaveLoad : MonoBehaviour
    {
        private GameData m_gameDataRef;
        //Music Manager Reference
        private MusicManager m_musicManagerRef;
	
        // Game Data Reference
        private GameObject m_gameManagerRef;
        private StateManager m_stateManagerRef;

        //called before Start()
        public void Awake ()
        {
            m_gameDataRef = GetComponent<GameData> ();
            m_stateManagerRef = GetComponent<StateManager> ();

            //Set up Music Manager
            m_musicManagerRef = m_gameDataRef.GetComponent<MusicManager> ();
        }


        //called to save game information
        public void Save ()
        {
            Debug.Log ("Called Save Method.");
//		// Delete existing file if one exists
//		if (File.Exists (Application.persistentDataPath + "/CKSaveData.dat")) {
//			File.Delete (Application.persistentDataPath + "/CKSaveData.dat");
//		}
            //creates new binary formatter which will handle the serialization
            BinaryFormatter bf = new BinaryFormatter (); 
            //creates a new filestream, which provides a pathway to a new file that data can be sent to
            FileStream file = File.Create (Application.persistentDataPath + "/CKSaveData.dat"); 

            SaveData saveData = new SaveData (m_gameDataRef, m_stateManagerRef); 
            //calls serialize to save to saveGames list
            bf.Serialize (file, saveData);
            //closes file after the save
            file.Close (); 
        }

        //called to load game information
        public void Load ()
        {
            int loadedGameTurn = 0;
            string loadedGameDay = null;
            int loadedGameMonth = 0;
            Debug.Log ("Called Load Method");
            //checks to see if any save games exist
            if (File.Exists (Application.persistentDataPath + "/CKSaveData.dat")) {
                //creates new binaryformatter - handles serializeatioss
                BinaryFormatter bf = new BinaryFormatter (); 
                //points to where the file is saved and opens it
                FileStream file = File.Open (Application.persistentDataPath + "/CKSaveData.dat", FileMode.Open); 
                //calls and finds file and deserializes it & converts binary data to a List (the List<GameData>) 
                SaveData loadedSaveData = (SaveData)bf.Deserialize (file);
                m_gameDataRef.UpdateFromLoadedSaveData (loadedSaveData.PlayerStats, loadedSaveData.PlayerArmies, loadedSaveData.ActivePlayers, loadedSaveData.AreaStats, loadedSaveData.Castles, loadedSaveData.GameSettings, loadedSaveData.ResolvedPoliticalCards, loadedSaveData.ResolvedPapalCards);
                loadedGameTurn = loadedSaveData.GameTurn;
                loadedGameDay = loadedSaveData.DateDay;
                loadedGameMonth = loadedSaveData.DateMonthNumber;
                file.Close ();
            }
            m_stateManagerRef.StartLoadedGame (loadedGameTurn, loadedGameDay, loadedGameMonth);
            m_musicManagerRef.PauseMenuMusic ();

        }

        public void ClearSaveGame ()
        {
            File.Delete (Application.persistentDataPath + "/CKSaveData.dat");
        }


        ///The serialisable save data class is the container class for the save data
        [Serializable]
        class SaveData
        {
            private int m_gameTurn;

            public int GameTurn
            { get { return m_gameTurn; } }

            // Player stats holds all of the player statistics as integer values in a 2D array
            private int[,] m_playerStats;

            public int[,] PlayerStats
            { get { return m_playerStats; } }

            private int[,] m_playerArmies;

            public int[,] PlayerArmies
            { get { return m_playerArmies; } }

            private int[,] m_castles;

            public int[,] Castles
            { get { return m_castles; } }

            private int[] m_activeplayers;

            public int[] ActivePlayers
            { get { return m_activeplayers; } }

            private int[,] m_areaStats;

            public int[,] AreaStats
            { get { return m_areaStats; } }

            private int[] m_gameSettings;

            public int[] GameSettings
            { get { return m_gameSettings; } }

            private string[] m_resolvedPoliticalCards;
            public string[] ResolvedPoliticalCards
            { get { return m_resolvedPoliticalCards; } }

            private string[] m_resolvedPapalCards;
            public string[] ResolvedPapalCards
            { get { return m_resolvedPapalCards; } }

            private string m_dateDay;

            public string DateDay
            { get { return m_dateDay; } }

            private int m_dateMonthNumber;

            public int DateMonthNumber
            { get { return m_dateMonthNumber; } }

            public SaveData (GameData gameData, StateManager stateManager)
            {
                // Populate the player stats
                m_playerStats = new int[gameData.players.Length, 9];
                for (int i = 0; i < gameData.players.Length; i++) {
                    m_playerStats [i, 0] = Convert.ToInt32 (gameData.players [i].IsActive);
                    m_playerStats [i, 1] = Convert.ToInt32 (gameData.players [i].IsHuman);
                    m_playerStats [i, 2] = gameData.players [i].Gold;
                    m_playerStats [i, 3] = gameData.players [i].Influence;
                    m_playerStats [i, 4] = gameData.players [i].Piety;
                    m_playerStats [i, 5] = gameData.players [i].Popularity;
                    m_playerStats [i, 6] = gameData.players [i].MyFactionIndex;
                    if (gameData.players [i].LastAttackedByPlayer != null)
                        m_playerStats [i, 7] = gameData.players [i].LastAttackedByPlayer.MyPlayerIndex;
                    else
                        m_playerStats [i, 7] = 5;
                    switch (gameData.players [i].aiPriority)
                    {
                        case Player.AIPriority.NotAI:
                        {
                            m_playerStats[i, 8] = 0;
                            break;
                        }
                        case Player.AIPriority.Military:
                        {
                            m_playerStats[i, 8] = 1;
                            break;
                        }
                        case Player.AIPriority.Popularity:
                        {
                            m_playerStats[i, 8] = 2;
                            break;
                        }
                        case Player.AIPriority.Religion:
                        {
                            m_playerStats[i, 8] = 3;
                            break;
                        }

                    }
                }

                // Populate the player army reference
                m_playerArmies = new int[gameData.players.Length, 6];
                for (int x = 0; x < gameData.players.Length; x++) {
                    // Count the players army units
                    gameData.players [x].CountArmyUnits ();
                    // Use the player's my army unit totals to populate the array
                    for (int y = 0; y < gameData.players[x].myArmyUnitTotals.Length; y++) {
                        m_playerArmies [x, y] = gameData.players [x].myArmyUnitTotals [y];
                    }
                }

                // Refresh the defences count
                // Count the number of castles in the game
                int castlesCount = gameData.CountCastles ();
                // populate the castles reference
                m_castles = new int[castlesCount, 7];
                for (int i = 0; i < gameData.players.Length; i++) {
                    Debug.Log ("Checking castles for player " + i);
                    for (int j = 0; j < gameData.players[i].myCastles.Count; j++) {
                        Debug.Log ("Checking player " + i + "'s castle at index " + j);
                        Debug.Log ("Checked " + gameData.players [i].myCastles [j].myArea.name + " castle");
                        for (int x = 0; x < castlesCount; x++) {
                            if (m_castles [x, 0] == 0) {
                                gameData.players [i].myCastles [j].CountDefences ();
						
                                m_castles [x, 0] = 1;
                                m_castles [x, 1] = gameData.players [i].MyPlayerIndex;
                                m_castles [x, 2] = gameData.players [i].myCastles [j].myArea.AreaIndex;
                                m_castles [x, 3] = gameData.players [i].myCastles [j].myDefenceTotals [0];
                                m_castles [x, 4] = gameData.players [i].myCastles [j].myDefenceTotals [1];
                                m_castles [x, 5] = gameData.players [i].myCastles [j].myDefenceTotals [2];
                                m_castles [x, 6] = gameData.players [i].myCastles [j].myDefenceTotals [3];
                                break;
                            }
                        }
                    }
                }
                // Debug: Show the saved data
                for (int x = 0; x < castlesCount; x++) {
                    Debug.Log ("Saved castle data at " + x + " as: " + m_castles [x, 0] + ", " + m_castles [x, 1] + ", " + m_castles [x, 2] + ", " + m_castles [x, 3] + ", " + m_castles [x, 4] + ", " + m_castles [x, 5] + ", " + m_castles [x, 6]);
                }


                // Populate the activeplayers
                m_activeplayers = new int[gameData.activePlayers.Count];
                for (int i = 0; i < m_activeplayers.Length; i++) {
                    m_activeplayers [i] = gameData.activePlayers [i].MyPlayerIndex;
                }

                // Populate the area stats
                m_areaStats = new int[gameData.AllAreas.Length, 4];
                for (int i = 0; i < gameData.AllAreas.Length; i++) {
                    if (gameData.AllAreas [i].ControllingPlayer != null) {
                        m_areaStats [i, 0] = gameData.AllAreas [i].ControllingPlayer.MyPlayerIndex;
                    } else {
                        m_areaStats [i, 0] = 5;
                    }
                    // If the area contains any units
                    if (gameData.AllAreas [i].AreaContainsUnits ()) {
                        m_areaStats [i, 1] = gameData.AllAreas [i].myOccupyingUnitTotals [0];
                        m_areaStats [i, 2] = gameData.AllAreas [i].myOccupyingUnitTotals [1];
                        m_areaStats [i, 3] = gameData.AllAreas [i].myOccupyingUnitTotals [2];
                    }
				
                } 

                // Set the game turn
                m_gameTurn = stateManager.GameTurn;
                // Set date 
                m_dateDay = stateManager.uiManagerRef.day;
                m_dateMonthNumber = stateManager.uiManagerRef.MonthNumber;

                // Populate the game settings array
                // element 0: holds battle animations enabled (bool value converted to int)
                // element 1: holds AI difficulty (0 for easy, 1 for standard, 2 for difficult
                // element 2: holds game length (actual int)
                m_gameSettings = new int[3];
                m_gameSettings [0] = Convert.ToInt32 (gameData.BattleAnimationsEnabled);
                switch (gameData.GameAIDifficulty) {
                    case GameData.AIDifficulty.Easy:
                    {
                        m_gameSettings[1] = 0;
                        break;
                    }
                    case GameData.AIDifficulty.Standard:
                    {
                        m_gameSettings[1] = 1;
                        break;
                    }
                    case GameData.AIDifficulty.Difficult:
                    {
                        m_gameSettings[1] = 2;
                        break;
                    }
                }
                m_gameSettings[2] = gameData.LastGameTurn;
                Debug.Log ("game settings saved as " +m_gameSettings[0]+", "+m_gameSettings[1]+", "+m_gameSettings[2]);

                // Get the names of the political cards that have been played
                m_resolvedPoliticalCards = new string[gameData.deckPolitics.resolvedPoliticalCards.Count];
                for (int i = 0; i < m_resolvedPoliticalCards.Length; i++)
                {
                    m_resolvedPoliticalCards[i] = gameData.deckPolitics.resolvedPoliticalCards[i].title;
                }
                // Get the names of the religious cards that have been played
                m_resolvedPapalCards = new string[gameData.deckPapal.resolvedPapalCards.Count];
                for (int i = 0; i < m_resolvedPapalCards.Length; i++)
                {
                    m_resolvedPapalCards[i] = gameData.deckPapal.resolvedPapalCards[i].title;
                }



            }
        }

    }
}

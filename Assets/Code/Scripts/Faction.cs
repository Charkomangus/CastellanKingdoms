using UnityEngine;

namespace Assets.Code.Scripts
{
    public class Faction : MonoBehaviour
    {

        private bool m_inPlay;

        public bool InPlay
        { get { return m_inPlay; } }

        private int m_controllingPlayerIndex;

        public int ControllingPlayerIndex
        { get { return m_controllingPlayerIndex; } }

        private int m_factionIndex;

        public int FactionIndex
        { get { return m_factionIndex; } }

        private Area m_startingArea;

        public Area StartingArea
        { get { return m_startingArea; } }

        private Sprite m_myFlagSprite;

        public Sprite MyFlagSprite
        { get { return m_myFlagSprite; } }

        private Vector2 m_startingAreaMapPosition;

        public Vector2 StartingAreaMapPosition
        { get { return m_startingAreaMapPosition; } }

        private string m_homeRegion;

        public string HomeRegion
        { get { return m_homeRegion; } }

        private string m_description;

        public string Description
        { get { return m_description; } }

        // Use this for initialization
        void Start ()
        {
			
        }
	
        // Update is called once per frame
        void Update ()
        {
//		if (m_inPlay == true)
//			Debug.Log ("" + this.name + " is in play");
        }

        // Assign the starting area
        public void AssignStartingArea (Area startingArea, Vector2 startingAreaPos)
        {
            m_startingArea = startingArea;
            m_startingAreaMapPosition = startingAreaPos;
            m_homeRegion = startingArea.tag;
        }

        // Set in play to the given value
        public void SetInPlay (bool inplay)
        {
            m_inPlay = inplay;
        }

        // Set faction index to the given value
        public void SetFactionIndex (int index)
        {
            m_factionIndex = index;
        }

        // Set Controlling Player Index to the given value
        public void SetControllingPlayerIndex (int index)
        {
            m_controllingPlayerIndex = index;
        }

        // Set Faction Flag to the given value
        public void SetFactionFlag (Sprite newSprite)
        {
            m_myFlagSprite = newSprite;
        }

        //Set Faction Description
        public void SetFactionDescription (int factionNumber)
        {
		 
            if (factionNumber == 0)
                m_description = "England is situated in a good area. It can quickly strike up North and conquer the Islands " +
                                "and quickly form a strong empire. " +
                                "" +
                                "The Norman invasion of England in 1066 led to the defeat and replacement of the Anglo-Saxon elite with " +
                                "Norman and French nobles and their supporters. William the Conqueror and his successors took over the existing " +
                                "state system, repressing local revolts and controlling the population through a network of castles.";
            if (factionNumber == 1)
                m_description = "The Kingdom of France in the Middle Ages was marked by the expansion of royal control by " +
                                "the “House of Capet”. This still did not change its real nature. Dozens of Duchies, " +
                                "Kingdoms and fiefs fight for power which you can advantage of and consolidate a strong empire.";
            if (factionNumber == 2)
                m_description = "Located, roughly in the centre of the map it has great opportunity to " + 
                                "expand. Beware not be surrounded by your enemies.	" +
                                "" +
                                "The Holy Roman Empire was a complex mess of territories in central Europe that " +
                                "developed during the Early Middle Ages. " +
                                "The largest territory of the empire after 962 was the Kingdom of Germany, though it " +
                                "included the Kingdom of Bohemia, the Kingdom of Burgundy, the Kingdom of Italy, and numerous" +
                                "other territories.";
            if (factionNumber == 3)
                m_description = "Almost cornered in the map Spain will have a tough time expanding past enemies. " +
                                "This also means that it can easily build a solid Defensive of Keeps and soldiers. " +
                                "In many ways, the history of Spain is marked by waves of conquerors who brought their distinct " +
                                "cultures to the peninsula.";
            if (factionNumber == 4)
                m_description = "Hungary is located in a gold mine of empty areas so it can easily expand. " +
                                "Beware your flanks as enemies may also move to claim them. " +
                                "The Kingdom of Hungary was a monarchy in Central Europe that existed from the Middle" +
                                "Ages into the twentieth century. The Principality of Hungary emerged as a Christian kingdom " +
                                "upon the coronation of the first king Stephen I.";
            if (factionNumber == 5)
                m_description = "The Empire is located superbly. With no enemies close by you can expand and grow before " +
                                "you have to meet your foes. " +
                                "" +
                                "The Byzantine Empire was the continuation of the Great Roman Empire during the Middle Ages. " +
                                "Its capital city was Constantinople. During most of its existence, the empire was the most powerful " +
                                "economic, cultural, and military force in Europe. Its citizens continued to refer to their empire as " +
                                "the Roman Empire.";
            if (factionNumber == 6)
                m_description = "Poland is located centrally and has almost unlimited potential to expand. " +
                                "Beware, your enemies will be closing in fast." +
                                "" +
                                "Following its emergence, the Polish nation was led by a series of rulers who converted the " +
                                "population to Christianity, created a strong kingdom and integrated Poland into the European culture.";
            if (factionNumber == 7)
                m_description = "Denmark begins in the North and if you successfully conquer it you can descend upon the Rest of " +
                                "Europe like a fist." +
                                "" +
                                "The history of Denmark as a unified kingdom, first begun in the 10th century, but historic documents " +
                                "describes the geographic area and the people living there - the Danes - as early as 500 AD.";

        }

        // Removes player link to a faction
        public void RemovePlayerFromFaction ()
        {
            m_inPlay = false;
            m_controllingPlayerIndex = 0;
        }

        public void setDescription(string newDescription)
        {
            m_description = newDescription;
        }

    }
}

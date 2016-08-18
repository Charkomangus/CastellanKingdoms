using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Scripts
{
    public class Castle : MonoBehaviour
    {

        public List<CardMilitary> castleDefences;

        public Area myArea;

        public int[] myDefenceTotals;
        private int m_watchTowerCount;
        private int m_ditchesCount;
        private int m_boilingOilCount;
        private int m_wallsCount;

	
        // Use this for initialization
        void Start ()
        {
            myDefenceTotals = new int[4];
        }
	
        // Update is called once per frame
        void Update ()
        {
	
        }

        // Use this to add a new defence card to the castle
        public void AddDefence (CardMilitary newCard)
        {
            // Add the new defence card to the castle's list 
            castleDefences.Add (Instantiate (newCard) as CardMilitary);
            // Make the new defence card a child of this castle
            castleDefences [castleDefences.Count - 1].transform.parent = this.transform;
        }

        // Counts the various types of defences which make up this castle
        public void CountDefences()
        {
            // reset all of the counts
            m_watchTowerCount = 0;
            m_ditchesCount = 0;
            m_boilingOilCount = 0;
            m_wallsCount = 0;

            // for every defence of the specified type in the castle, increase the defence count
            for (int i = 0; i < castleDefences.Count; i++)
            {
                if (castleDefences[i].myDefenceType == CardMilitary.DefenceType.WatchTower)
                    m_watchTowerCount++;
                if (castleDefences[i].myDefenceType == CardMilitary.DefenceType.Ditch)
                    m_ditchesCount++;
                if (castleDefences[i].myDefenceType == CardMilitary.DefenceType.BoilingOil)
                    m_boilingOilCount++;
                if (castleDefences[i].myDefenceType == CardMilitary.DefenceType.Wall)
                    m_wallsCount++;
            }

            // Set the new totals based on the refreshed count
            myDefenceTotals = new int[4];
            myDefenceTotals [0] = m_watchTowerCount;
            myDefenceTotals [1] = m_ditchesCount;
            myDefenceTotals [2] = m_boilingOilCount;
            myDefenceTotals [3] = m_wallsCount;

            Debug.Log ("" +myArea.name +" castle has " +m_watchTowerCount + " towers, " +m_ditchesCount +" ditches, " +m_boilingOilCount +" oil and " +m_wallsCount +"walls.");

        }

        // Set the castle's area to the area passed through 
        public void SetMyArea (Area area)
        {
            myArea = area;
            Debug.Log ("Castle area set to: " +myArea.name);
        }

        // Destroys the castle
        public void DestroyCastle ()
        {
            Destroy (this.gameObject);
        }
    }
}

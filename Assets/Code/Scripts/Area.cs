using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class Area : MonoBehaviour
    {

        private Player m_controllingPlayer;

        public Player ControllingPlayer {
            get { return m_controllingPlayer;}
        }

        private Button m_myMapButton;

        public Button MyMapButton {
            get { return m_myMapButton;}
        }

        public Area[] myNeighbours;

        public CardMilitary[] occupyingUnits;
        public int[] myOccupyingUnitTotals;
        private int m_spearmenCount;
        private int m_archersCount;
        private int m_knightsCount;

        public Castle occupyingCastle;

        private int m_areaIndex;
        public int AreaIndex
        { get { return m_areaIndex; } }

        // Use this for initialization
        void Start ()
        {
            occupyingUnits = new CardMilitary[3];
        }
	
        // Update is called once per frame
        void Update ()
        {
	
        }

        public void SetAreaIndex(int index)
        {
            m_areaIndex = index;
            //Debug.Log ("" +this.name +"'s index is: " +m_areaIndex);
        }

        public void SetControllingPlayer (Player newControllingPlayer)
        {
            m_controllingPlayer = newControllingPlayer;
        }

        public void AssignMapButton (Button myButton)
        {
            m_myMapButton = myButton;
        }

        // Assign the referenced unit to this area's occupying units.
        public void AssignOccupyingUnit (CardMilitary UnitRef, bool isNewUnit)
        {
            // If a new unit is to be created
            if (isNewUnit) {
                // Local variable to hold the new unit
                CardMilitary newUnit;
                // Create the new unit from the parameter reference
                newUnit = Instantiate (UnitRef) as CardMilitary;

                // Assign the new unit to the next available space in the array.
                for (int i = 0; i < occupyingUnits.Length; i++) {
                    if (occupyingUnits [i] == null) {
                        occupyingUnits [i] = newUnit;
                        occupyingUnits [i].transform.parent = GameObject.Find ("" + this.name).transform;
                        break;
                    }
                }
            }
            // If this is an existing unit
            else {
                // Assign the referenced unit to the next available space in the array.
                for (int i = 0; i < occupyingUnits.Length; i++) {
                    if (occupyingUnits [i] == null) {
                        occupyingUnits [i] = UnitRef;
                        occupyingUnits [i].transform.parent = GameObject.Find ("" + this.name).transform;
                        break;
                    }
                }
            }
        }

        public void CountOccupyingUnits ()
        {
            // reset all the counts
            m_spearmenCount = 0;
            m_archersCount = 0;
            m_knightsCount = 0;
		
            // For every unit of the specified type in the army, increase the count for that type by one
            for (int i = 0; i < occupyingUnits.Length; i++) {
                //Debug.Log ("Type of " + i + " is: " + myArmy [i].myMilitaryType);
                if (occupyingUnits [i] != null) {
                    if (occupyingUnits [i].myMilitaryType == CardMilitary.MilitaryType.Spearmen)
                        m_spearmenCount++;
                    if (occupyingUnits [i].myMilitaryType == CardMilitary.MilitaryType.Archers)
                        m_archersCount++;
                    if (occupyingUnits [i].myMilitaryType == CardMilitary.MilitaryType.Knights)
                        m_knightsCount++;
                }
            }
		
            // Set the new totals based on the new counts
            myOccupyingUnitTotals = new int[3] {
                m_spearmenCount,
                m_archersCount,
                m_knightsCount
            };
        }

        /// <summary>
        /// Returns true if the area contains any occupying units.
        /// This is determined by refreshing the count, then adding all of the unit totals together
        /// </summary>
        public bool AreaContainsUnits ()
        {
            int occupyingUnitsCount;
            occupyingUnitsCount = 0;
            CountOccupyingUnits ();
            occupyingUnitsCount = m_spearmenCount + m_archersCount + m_knightsCount;

            if (occupyingUnitsCount > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if that area has any spaces for units to be assigned.
        /// Returns true if it does, false if not
        /// </summary>
        public bool AreaFull()
        {
            bool areaFull;
            areaFull = true;
            for (int i = 0; i < occupyingUnits.Length; i ++) {
                if (occupyingUnits[i] == null)
                    areaFull = false;
            }
            if (areaFull)
                return true;
            else
                return false;
        }


        public void SortOccupyingUnits()
        {
            CountOccupyingUnits ();
            if ((myOccupyingUnitTotals [0] + myOccupyingUnitTotals [1] + myOccupyingUnitTotals [2]) > 2) {
                Array.Sort (occupyingUnits, delegate(CardMilitary unit1, CardMilitary unit2) { 
                                                                                                 return unit1.defenceOrder.CompareTo (unit2.defenceOrder);
                });
            }
        }

        // Assign the referenced castle to this area
        public void AssignCastle (Castle newCastle)
        {
            occupyingCastle = newCastle;
            newCastle.SetMyArea (this);
        }

        // Clear the area of occupying units and castles
        public void ClearArea()
        {
            for (int i = 0; i < occupyingUnits.Length; i ++) {
                if (occupyingUnits [i] != null) {
                    occupyingUnits [i].DestroyUnit ();
                    occupyingUnits [i] = null;
                }
            }

            occupyingCastle = null;
            m_controllingPlayer = null;
        }

    }
}

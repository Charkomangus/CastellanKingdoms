using UnityEngine;

namespace Assets.Code.Scripts
{
    public class CardMilitary : MonoBehaviour
    {

        // Enumerators
        public enum MilitaryType
        {
            Spearmen,
            Archers,
            Knights,
            Ram,
            Belfry,
            Artillery,
            Defence,
            Wall,
            Keep
        }

        public enum DefenceType
        {
            None,
            WatchTower,
            Ditch,
            BoilingOil,
            Wall
        }

        public enum BattleType
        {
            Pitched,
            Siege
        }
	
        // Variables
        public MilitaryType myMilitaryType;
        public DefenceType myDefenceType;

        // The cost in gold of the unit
        public int m_cost;

        public int Cost
        { get { return m_cost; } }

        // The default deployment order if the unit is defending a castle during a siege
        public int defenceOrder;

        // Maximum combat value for pitched and siege combat
        public int pitchedCombatMax;
        public int siegeCombatMax;

        // These determine the unit type against which advantage / disadvantage is applied
        public MilitaryType advantageVersus;
        public int baseCombatScore;
        public int advantageModifier = 2;
        public int combatScore;

        // Use this for initialization
        void Start ()
        {

        }

        public void DestroyUnit ()
        {
            Destroy (this.gameObject);
        }

        // Use this to assign values dependent on type
        public void AssignValues (MilitaryType myType)
        {
            // Set military card type
            myMilitaryType = myType;

            // Name the card after its type
            this.name = "" + myMilitaryType;

            // Assign values based on military type
            switch (myMilitaryType) {
                case MilitaryType.Spearmen:
                {
                    m_cost = 3;
                    defenceOrder = 2;
                    pitchedCombatMax = 4;
                    siegeCombatMax = 2;
                    advantageVersus = MilitaryType.Knights;
                    break;}
                case MilitaryType.Archers:
                {
                    m_cost = 5;
                    defenceOrder = 3;
                    pitchedCombatMax = 6;
                    siegeCombatMax = 3;
                    advantageVersus = MilitaryType.Spearmen;
                    break;}
                case MilitaryType.Knights:
                {
                    m_cost = 7;
                    defenceOrder = 1;
                    pitchedCombatMax = 8;
                    siegeCombatMax = 4;
                    advantageVersus = MilitaryType.Archers;
                    break;}
                case MilitaryType.Ram:
                {
                    m_cost = 5;
                    pitchedCombatMax = 2;
                    siegeCombatMax = 8;
                    advantageVersus = MilitaryType.Keep;

                    break;}
                case MilitaryType.Belfry:
                {
                    m_cost = 7;
                    pitchedCombatMax = 4;
                    siegeCombatMax = 10;
                    advantageVersus = MilitaryType.Wall;

                    break;}
                case MilitaryType.Artillery:
                {
                    m_cost = 10;
                    pitchedCombatMax = 6;
                    siegeCombatMax = 12;
                    advantageVersus = MilitaryType.Wall;

                    break;}
                case MilitaryType.Defence:
                {
                    m_cost = 5;
                    pitchedCombatMax = 0;
                    siegeCombatMax = 8;
                    advantageVersus = MilitaryType.Artillery;

                    break;}
                case MilitaryType.Wall:
                {
                    m_cost = 7;
                    pitchedCombatMax = 0;
                    siegeCombatMax = 10;
                    advantageVersus = MilitaryType.Ram;

                    break;}
                case MilitaryType.Keep:
                {
                    m_cost = 10;
                    pitchedCombatMax = 0;
                    siegeCombatMax = 12;
                    advantageVersus = MilitaryType.Belfry;

                    break;}

            }


        }

        // Update is called once per frame
        void Update ()
        {
        }

        // Calculate Combat Value
        public void CalculateCombatValue (BattleType currBatt, MilitaryType opponentType)
        {
            // Combat is resolved dependant on the type of battle 
            switch (currBatt) {
                // In the case of a pitched battle
                case BattleType.Pitched:
                {
                    // Base combat is random, between one and the unit's pitchedCombatMax
                    baseCombatScore = Random.Range (1, pitchedCombatMax);
                    // Apply +2 modifier if unit has advantage over the opponent type 
                    if (opponentType == advantageVersus) {
                        combatScore = baseCombatScore + advantageModifier;
                    } else {
                        combatScore = baseCombatScore;
                    }
                    break;
                }
                // In the case of a siege battle1
                case BattleType.Siege:
                {
                    // base combat is random, between one the unit's siegeCombatMax
                    baseCombatScore = Random.Range (1, siegeCombatMax);
                    // Apply a +2 modifier if unit has advantage over the oppponent type
                    if (opponentType == advantageVersus) {
                        combatScore = baseCombatScore + advantageModifier;
                    } else {
                        combatScore = baseCombatScore;
                    }
                    break;
                }
            }
        }

        public void SetDefenceType(CardMilitary.DefenceType type)
        {
            myDefenceType = type;
        }


    }
}

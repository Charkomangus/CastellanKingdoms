using UnityEngine;

namespace Assets.Code.Scripts
{
    public class PitchedBattleAnim : MonoBehaviour 
    {
        //variables
        private int m_animTimer = 0;
        private int m_animTime = 100;
        private bool m_animating;	
        public bool moving; 
        public bool attacking; 
        public bool defeated; 
        private PitchedBattleManager m_pitchedBattleManRef;
        public GameObject pitchedBattleManGO;
        private GameData m_gameDataRef; 
        private StateManager m_statemanagerRef; 
        private GameObject m_gameManager; 
        //stores the animator prefabs
        public GameObject anim_attackCupboard; 
        public GameObject anim_defenseCupboard;
        //stores the animator prefabs
        public GameObject anim_attDefeatCupboard;
        public GameObject anim_defDefeatCupboard; 

        //flipping
        public bool facingRight; 
        //reference to animator
        public Animator archerAnim; 
        public Animator spearmanAnim; 
        public Animator knightAnim; 
        //array of animators
        public Animator[] anim_attackingArmy; 
        public Animator[] anim_defendingArmy; 
        //position information
        public Vector3[] attackPositions; 
        public Vector3[] defensePositions; 
        //scale information
        public Vector3[] attackScales; 
        public Vector3[] defenseScales; 


        void Start()
        {
            m_gameManager = GameObject.Find ("GameManager"); 
            m_gameDataRef = m_gameManager.GetComponent<GameData> (); 
            m_pitchedBattleManRef = GetComponent<PitchedBattleManager> (); 
            m_statemanagerRef = m_gameManager.GetComponent<StateManager> (); 
            //assign array values
            anim_attackingArmy = new Animator[3]; 
            anim_defendingArmy = new Animator[3]; 
            //assign position number
            attackPositions = new Vector3[3];
            attackPositions [0] = new Vector3 (115, 115, 0); 
            attackPositions [1] = new Vector3 (220, 90, 0); 
            attackPositions [2] = new Vector3 (235, 80, 0);
            defensePositions = new Vector3[3];
            defensePositions [0] = new Vector3 (480, 115, 0); 
            defensePositions [1] = new Vector3 (415, 110, 0); 
            defensePositions [2] = new Vector3 (380, 100, 0);
            //assign scales
            attackScales = new Vector3[3]; 
            attackScales [0] = new Vector3 (30, 25, 0);
            attackScales [1] = new Vector3 (20, 15, 0); 
            attackScales [2] = new Vector3 (10, 8, 0); 
            defenseScales = new Vector3[3];
            defenseScales [0] = new Vector3 (30, 25, 0);
            defenseScales [1] = new Vector3 (20, 15, 0); 
            defenseScales [2] = new Vector3 (10, 8, 0); 
        }

        void Update()
        {
            // If we are animating, increase the animation timer
            if (m_animating)
                m_animTimer++;

            // When the animation time hits the timer limit
            if (m_animTimer >= m_animTime) {
                // We're no longer animating, 0 the timer
                m_animating = false;
                m_animTimer = 0;
                // Compare the results of the combat
                m_pitchedBattleManRef.CompareCombatResults();
                DestroyDefeatedAnim();
            }
        }
        //called when animation is about to play
        public void AssignAttackAnimations()
        {
            // creates new game object attack cupboard as a container for the animations
            anim_attackCupboard = new GameObject ();
            anim_attDefeatCupboard = new GameObject ();
            // make this a child of the pitched battle manager
            anim_attackCupboard.transform.parent = pitchedBattleManGO.transform;
            // Name the cupboard
            anim_attackCupboard.name = "AttackArmyCupboard";
            Debug.Log ("Are you assiginging?"); 
            for (int i = 0; i < anim_attackingArmy.Length; i++) //looping through the attacking army array
            {
                if (m_pitchedBattleManRef.attackingArmy[i] != null) //checks to see if there is a unit in the slot of the array
                {
                    switch (m_pitchedBattleManRef.attackingArmy[i].myMilitaryType) // this matches the attacking army array to the animation array, giving slots to slots
                    {
                        // If the corresponding attack army index is spearmen, instantiate a new spearmen animator
                        case CardMilitary.MilitaryType.Spearmen:
                            anim_attackingArmy[i] = Instantiate(spearmanAnim) as Animator; 
                            break;
                        // If the corresponding attack army index is archers, instantiate a new archers animator
                        case CardMilitary.MilitaryType.Archers:
                            anim_attackingArmy[i] = Instantiate(archerAnim) as Animator; 
                            break; 
                        // If the corresponding attack army index is archers, instantiate a new knights animator
                        case CardMilitary.MilitaryType.Knights:
                            anim_attackingArmy[i] = Instantiate(knightAnim) as Animator; 
                            break; 
				
                    }
                    // attach the new attacking army animation as a child of the attack cupboard
                    anim_attackingArmy[i].transform.parent = anim_attackCupboard.transform;
                }
            }
        }
        //called after the assign 
        public void AssignAttackPositions()
        {
            for (int i = 0; i <anim_attackingArmy.Length; i++)
            {
                if (anim_attackingArmy[i] != null)
                {
                    SpriteRenderer tempSR; //creates temprorray sprite rendersr, only exists in for loop 
                    tempSR = anim_attackingArmy[i].GetComponent<SpriteRenderer>(); 
                    //gives position to the animations
                    tempSR.transform.position = attackPositions[i]; 
                    tempSR.sortingOrder = 4- i; // sets the sorting layer of the animation 
                    //assigns the scale
                    tempSR.transform.localScale = attackScales[i]; 
                }
            }
        }

        //called when animation is about to play
        public void AssignDefenceAnimations()
        {
            // creates new game object attack cupboard as a container for the animations
            anim_defenseCupboard = new GameObject ();
            anim_defDefeatCupboard = new GameObject ();
            // make this a child of the pitched battle manager
            anim_defenseCupboard.transform.parent = pitchedBattleManGO.transform;
            // Name the cupboard
            anim_defenseCupboard.name = "AttackArmyCupboard";
            Debug.Log ("Are you assiginging?"); 
            for (int i = 0; i < anim_defendingArmy.Length; i++) //looping through the attacking army array
            {
                if (m_pitchedBattleManRef.defendingArmy[i] != null) //checks to see if there is a unit in the slot of the array
                {
                    switch (m_pitchedBattleManRef.defendingArmy[i].myMilitaryType) // this matches the attacking army array to the animation array, giving slots to slots
                    {
                        case CardMilitary.MilitaryType.Spearmen:
                            anim_defendingArmy[i] = Instantiate(spearmanAnim) as Animator; 
                            break;
                        case CardMilitary.MilitaryType.Archers:
                            anim_defendingArmy[i] = Instantiate(archerAnim) as Animator; 
                            break; 
                        case CardMilitary.MilitaryType.Knights:
                            anim_defendingArmy[i] = Instantiate(knightAnim) as Animator; 
                            break; 
                    }
                    anim_defendingArmy[i].transform.parent = anim_defenseCupboard.transform;
                }
            }
        }

        public void AssignDefencePositions()
        {
            for (int i = 0; i <anim_defendingArmy.Length; i++)
            {
                if (anim_defendingArmy[i] != null)
                {
                    SpriteRenderer tempSR; //creates temprorray sprite rendersr, only exists in for loop 
                    tempSR = anim_defendingArmy[i].GetComponent<SpriteRenderer>(); 
                    //gives position to the animations
                    tempSR.transform.position = defensePositions[i]; 
                    tempSR.sortingOrder = 4- i; // sets the sorting layer of the animation 
                    //assigns the scale
                    tempSR.transform.localScale = defenseScales[i]; 
                    // flip the sprite
                    Flip (tempSR);
                }
            }
        }

        public void StartAnimation()
        {
            m_animating = true;
        }

        // Set the defeated animation for destruction
        public void SetDefeatAnim(string armyType, int index)
        {
            if (armyType == "attacker") {
                anim_attackingArmy[index].transform.parent = anim_attDefeatCupboard.transform;
                anim_attDefeatCupboard.name = "AttCupboardDefeatGO";
            } else {
                anim_defendingArmy[index].transform.parent = anim_defDefeatCupboard.transform;
                anim_defDefeatCupboard.name = "DefCupboardDefeatGO";
            }
            // Start anim timer
        }

        public void DestroyDefeatedAnim()
        {
            Destroy (anim_attDefeatCupboard); //destroy the game objects containing the animators attack
            Destroy (anim_defDefeatCupboard); //destroy the game objects containing the animators defense
        }
	
        public void KillAnimation()
        {
            //Deals with attacking armies
            Destroy (anim_attackCupboard);
            //deals with defending armies
            Destroy (anim_defenseCupboard);
        }
	
        void Flip(SpriteRenderer sprite)
        {
            //facingRight = !facingRight; 
            Vector3 xScale = sprite.transform.localScale; //sets the transform relative to a parent
            xScale.x *= -1; //setting to -1 flips 
            sprite.transform.localScale = xScale; 
        }
    }
}

using UnityEngine;

namespace Assets.Code.Scripts
{
    public class SiegeBattleAnim : MonoBehaviour 
    {
        //variables
        public bool attacking; 
        public bool defeated; 
        private SiegeBattleManager m_siegeBattleManRef;
        private GameData m_gameDataRef; 
        private StateManager m_statemanagerRef; 
        private GameObject m_gameManager; 
        //flipping
        public bool facingRight; 
        //reference to animator
        public Animator archerAnim; 
        public Animator spearmanAnim; 

        void Start()
        {
            m_gameManager = GameObject.Find ("GameManager"); 
            m_gameDataRef = m_gameManager.GetComponent<GameData> (); 
            m_siegeBattleManRef = m_gameManager.GetComponent<SiegeBattleManager> (); 
            m_statemanagerRef = m_gameManager.GetComponent<StateManager> (); 
        }

        void Update()
        {
            //need a check to see whether it is the enemy or the player
            //need a check to see the units involved (number & type)
            //need a prompt to trigger the start of the animation phase: needs 2 armies
            //needs some means of resolution
        }

        void Flip()
        {
            facingRight = !facingRight; 
            Vector3 xScale = transform.localScale; //sets the transform relative to a parent
            xScale.x *= -1; //setting to -1 flips 
            transform.localScale = xScale; 
        }
    }
}

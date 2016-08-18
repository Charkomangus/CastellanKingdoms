using Assets.Code.Scripts;
using UnityEngine;

namespace Assets.Code.States.CastleStates
{
    public class LibraryState : MonoBehaviour {

        // Game Data Reference
        private GameObject m_gameManagerRef;

        // State m_stateManagerRef
        private StateManager m_stateManagerRef;


        // Use this for initialization
        void Start () {
            m_gameManagerRef = GameObject.Find ("GameManager");	
            m_stateManagerRef = m_gameManagerRef.GetComponent<StateManager> ();
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
        // Update is called once per frame
        public void OpenTutorial () {
            m_stateManagerRef.uiManagerRef.ActivateTutorial ();
        }
    }
}

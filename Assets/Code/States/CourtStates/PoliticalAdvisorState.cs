using Assets.Code.Interfaces;
using Assets.Code.Scripts;

namespace Assets.Code.States.CourtStates
{
		public class PoliticalAdvisorState : IStateBase
		{
		//variables
			private CourtManager m_courtManager; 
		//constructor
			public PoliticalAdvisorState (CourtManager managerRef) //constructor
			{
				m_courtManager = managerRef; 
			}
			
			public void StateUpdate()
			{
				
			}
			
			public void ShowUI()
			{

			}

		public void StateTrigger()
		{
		}
		}
}


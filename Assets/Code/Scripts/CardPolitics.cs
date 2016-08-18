using System;
using UnityEngine;

namespace Assets.Code.Scripts
{
    public class CardPolitics : MonoBehaviour 
    {
        //card details
        public string title; 
        public string description;
        public string history; 
        public string option1; 
        public string option2; 
        public string theOutcome1; 
        public string theOutcome2; 
        public int pop1; 
        public int pop2; 
        public int gold1; 
        public int gold2; 
        public int inf1; 
        public int inf2; 
        public int piety1; 
        public int piety2; 

        // Has this card been played?
        private bool m_played;
        public bool Played
        {get {return m_played;}}

        //constructor method
        public void AssignValues(string cardTitle, string cardDescription, string cardHistoricalTeachings, string cardOption1, string cardOption2, string cardOutcome1,string cardOutcome2,string cardPop1, string cardPop2, string cardGold1, string cardGold2, string cardInf1, string cardInf2, string cardPiety1, string cardPiety2 )
        {
            title = cardTitle; 
            description = cardDescription;
            history = cardHistoricalTeachings; 
            option1 = cardOption1; 
            option2 = cardOption2; 
            theOutcome1 = cardOutcome1; 
            theOutcome2 = cardOutcome2; 
            pop1 = Convert.ToInt32(cardPop1); 
            pop2 = Convert.ToInt32 (cardPop2); 
            gold1 = Convert.ToInt32(cardGold1); 
            gold2 = Convert.ToInt32(cardGold2); 
            inf1 = Convert.ToInt32(cardInf1); 
            inf2 = Convert.ToInt32(cardInf2); 
            piety1 = Convert.ToInt32(cardPiety1); 
            piety2 = Convert.ToInt32(cardPiety2); 
        }
	

        // Use this to set the card's 'played' value to true or false
        public void SetPlayed (bool played)
        {
            m_played = played;
        }
    }
}

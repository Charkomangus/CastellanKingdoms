using System;
using UnityEngine;

namespace Assets.Code.Scripts
{
    public class CardPapal : MonoBehaviour {

        // Variables
        //card details
        public string title; 
        public string description; 
        public string papalHistory; 
        public int costGold; 
        public int costInfluence; 
        public int costPiety; 
        public int costPopularity; 
        public int rewardGold; 
        public int rewardInfluence; 
        public int rewardPiety; 
        public int rewardPopularity; 
        public int penaltyGold; 
        public int penaltyInfluence; 
        public int penaltyPiety; 
        public int penaltyPopularity; 
        // Has the card been played?
        private bool m_played;
        public bool Played
        {get {return m_played;}}

        //constructior method assigns values, as read from the xml to the cards	
        public void AssignValues(string cardTitle, string cardDescription, string cardHistory, string cardCostGold, string cardCostInfluence, string cardCostPiety, string cardCostPopularity, string cardRewardGold, string cardRewardInfluence, string cardRewardPiety, string cardRewardPopularity, string cardPenaltyGold, string cardPenaltyInfluence, string cardPenaltyPiety, string cardPenaltyPopularity)
        {
            //assign the variables in card class to corresponding value in constructor method
            title = cardTitle; 
            description = cardDescription; 
            papalHistory = cardHistory; 
            costGold = Convert.ToInt32(cardCostGold);  
            costInfluence = Convert.ToInt32(cardCostInfluence); 
            costPiety = Convert.ToInt32(cardCostPiety); 
            costPopularity = Convert.ToInt32(cardCostPopularity); 
            rewardGold = Convert.ToInt32(cardRewardGold); 
            rewardInfluence = Convert.ToInt32(cardRewardInfluence); 
            rewardPiety = Convert.ToInt32(cardRewardPiety); 
            rewardPopularity = Convert.ToInt32(cardRewardPopularity); 
            penaltyGold = Convert.ToInt32(cardPenaltyGold); 
            penaltyInfluence = Convert.ToInt32(cardPenaltyInfluence); 
            penaltyPiety = Convert.ToInt32(cardPenaltyPiety); 
            penaltyPopularity = Convert.ToInt32(cardPenaltyPopularity); 
        }

        // Use this to set the card's 'played' value to true or false
        public void SetPlayed (bool played)
        {
            m_played = played;
        }
    }
}

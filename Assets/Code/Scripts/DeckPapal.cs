using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Assets.Code.Scripts
{
    public class DeckPapal : MonoBehaviour 
    {
        //list of strings
        static public string title = "";  
        static public string description = ""; 
        static public string papHistory = ""; 
        static public string costGold = ""; 
        static public string costInfluence = ""; 
        static public string costPiety = ""; 
        static public string costPopularity = ""; 
        static public string rewardGold = ""; 
        static public string rewardInfluence = ""; 
        static public string rewardPiety = ""; 
        static public string rewardPopularity = ""; 
        static public string penaltyGold = ""; 
        static public string penaltyInfluence = ""; 
        static public string penaltyPiety = ""; 
        static public string penaltyPopularity = ""; 
        //prefab references
        public CardPapal prefabCard; 
        //list of cards
        public List<CardPapal> papalCards;
        public List<CardPapal> resolvedPapalCards;
        public CardPapal[] selectedPapalCard; 
        //reference to the xml file
        public TextAsset papalCardsXML; 
        //list o' dictionary cards
        public List<Dictionary<string,string>> cards; 
        public Dictionary<string, string> cardText;
        //int
        public int randomIndex; 


        public void Start()
        {
            //gives 5 empty slots essentially
            selectedPapalCard = new CardPapal[5]; 
            //instantiates new cards referenceee
            cards = new List<Dictionary<string, string>> (); 
        }

        public void InitDeck()
        {
            ReadXML (); 
            MakeCards (); 
        }

        public void MakeCards()
        {
            for (int i = 0; i < cards.Count; i++) //loop through cards list
            {
                //instantiaets na ew card prefab of ccardPapal type
                papalCards.Add(Instantiate (prefabCard) as CardPapal); 
                //looks better in the heirarchy  
                papalCards [papalCards.Count -1].transform.parent = GameObject.Find ("Religious").transform;
                //title
                cards [papalCards.Count -1].TryGetValue ("title", out title); 
                //description
                cards[papalCards.Count-1].TryGetValue("description",out description);
                //history
                cards[papalCards.Count - 1].TryGetValue("history",out papHistory); 
                //cost
                cards[papalCards.Count -1].TryGetValue("CostGold", out costGold); 
                cards[papalCards.Count -1].TryGetValue("CostInfluence", out costInfluence); 
                cards[papalCards.Count-1].TryGetValue("CostPiety", out costPiety);
                cards[papalCards.Count-1].TryGetValue("CostPopularity", out costPopularity); 
                //reward
                cards[papalCards.Count -1].TryGetValue("RewardGold", out rewardGold);
                cards[papalCards.Count -1].TryGetValue("RewardInfluence", out rewardInfluence); 
                cards[papalCards.Count -1].TryGetValue("RewardPiety", out rewardPiety); 
                cards[papalCards.Count -1].TryGetValue("RewardPopularity", out rewardPopularity); 
                //Debug.Log (rewardGold); 
                //penalty
                cards[papalCards.Count-1].TryGetValue("PenaltyGold", out penaltyGold); 
                cards[papalCards.Count -1].TryGetValue("PenaltyInfluence", out penaltyInfluence); 
                cards[papalCards.Count-1].TryGetValue("PenaltyPiety", out penaltyPiety); 
                cards[papalCards.Count-1].TryGetValue("PenaltyPopularity", out penaltyPopularity); 
                //assign all the information to it
                papalCards [papalCards.Count-1].AssignValues (title, description, papHistory, costGold, costInfluence, costPiety, costPopularity, rewardGold, rewardInfluence, rewardPiety, rewardPopularity, penaltyGold, penaltyInfluence, penaltyPiety, penaltyPopularity);
            }
        }

        public void ReadXML()
        {
            cards = new List<Dictionary<string, string>> (); 
            //creates a new xml document
            XmlDocument docXML = new XmlDocument (); 
            //load in the xml file
            docXML.LoadXml (papalCardsXML.text); 
            //array of all the card nodes in the xml doctument
            XmlNodeList cardList = docXML.GetElementsByTagName ("card");

            //looks through all of the xml nodes and reads the file, searching for the 'card' elements
            foreach (XmlNode cardFeat in cardList)
            {
                //creates a new list of all the child nodes of the card element
                XmlNodeList cardInfo = cardFeat.ChildNodes;
                //ref to text dict
                cardText = new Dictionary<string, string>(); 

                foreach (XmlNode cardAtt in cardInfo)
                {
                    if (cardAtt.Name == "title") 
                    {
                        cardText.Add("title", cardAtt.InnerText); //the title is added to the dictionary
                    }

                    if (cardAtt.Name == "description")
                    {
                        cardText.Add("description", cardAtt.InnerText); //dictionary works by having a 'key & value', "description" is the key and the text in the xml is the value
                    }

                    if (cardAtt.Name == "history")
                    {
                        cardText.Add ("history", cardAtt.InnerText); //history text in dictionary
                    }

                    if (cardAtt.Name == "cost")
                    {
                        switch(cardAtt.Attributes["stat"].Value) //if the element has an attribute, create a switch statement case for each attribute
                        {
                            case "CostGold":
                                cardText.Add("CostGold", cardAtt.InnerText); 
                                break; 
                            case "CostInfluence":
                                cardText.Add("CostInfluence", cardAtt.InnerText); 
                                break; 
                            case "CostPiety":
                                cardText.Add("CostPiety", cardAtt.InnerXml);
                                break; 
                            case "CostPopularity":
                                cardText.Add("CostPopularity", cardAtt.InnerText); 
                                break; 
                        }
                    }

                    if (cardAtt.Name == "reward")
                    {
                        switch(cardAtt.Attributes["stat"].Value)
                        {
                            case "RewardGold":
                                cardText.Add("RewardGold", cardAtt.InnerText); 
                                break; 
                            case "RewardInfluence":
                                cardText.Add("RewardInfluence", cardAtt.InnerText); 
                                break; 
                            case "RewardPiety":
                                cardText.Add("RewardPiety", cardAtt.InnerXml);
                                break; 
                            case "RewardPopularity":
                                cardText.Add("RewardPopularity", cardAtt.InnerText); 
                                break; 
                        }
                    }

                    if (cardAtt.Name == "penalty")
                    {
                        switch(cardAtt.Attributes["stat"].Value)
                        {
                            case "PenaltyGold":
                                cardText.Add("PenaltyGold", cardAtt.InnerText); 
                                break; 
                            case "PenaltyInfluence":
                                cardText.Add("PenaltyInfluence", cardAtt.InnerText); 
                                break; 
                            case "PenaltyPiety":
                                cardText.Add("PenaltyPiety", cardAtt.InnerXml);
                                break; 
                            case "PenaltyPopularity":
                                cardText.Add("PenaltyPopularity", cardAtt.InnerText); 
                                break; 
                        }
                    }

                }
                cards.Add (cardText); //add the dictionary containing card key and values to the list
            }
        }

        //called when the cards need to be assigned to the screen
        public void DrawPapalCards ()
        {
            for (int i = 0; i < selectedPapalCard.Length; i++) {
                randomIndex = Random.Range (0, papalCards.Count -1); //-1 gives the index 
                //assign to the array of 5 cards
                selectedPapalCard [i] = papalCards [randomIndex]; 
                //delete it from main list
                papalCards.RemoveAt (randomIndex); 
            }
        }
	
        //removes selectedCards and places it into the resolved List
        public void ResolveSelectedPapalArray()
        {
            //goes through selectedcards
            for (int i = 0; i < selectedPapalCard.Length; i++) {
                //adds the cards to resolved deck
                resolvedPapalCards.Add(selectedPapalCard[i]); 
                //clears array index
                selectedPapalCard[i] = null; 
            }
		
		
            if (ResolvedAllPapalCards ()) {//if all the cards have been used
                RefillPapalCards (); //call the refill deck method
//		} else {
//			DrawPapalCards(); //draws cards at the beginning of turn
            }
        }

        //checks to see whether or not there is enough cards to produce an array of selectedCards
        public bool ResolvedAllPapalCards()
        {
            if (papalCards.Count <= 5) //checking if the list has no cards
            {
                return true; //this returns the value as true
            } 
            else 
            {
                return false; //if not, returns as false
            }
        }
        //adds all of the resolved papal cards to the drawDeck
        public void RefillPapalCards()
        {
            for (int i = 0; i < resolvedPapalCards.Count; i++) 
            {
                resolvedPapalCards[i].SetPlayed(false); //set all the resolved cards as having not been played
                papalCards.Add(resolvedPapalCards[i]); //add resolved deck to draw deck
            }
            resolvedPapalCards.Clear (); //clears the resolved deck after it is added
        }
    }
}

using System.Collections.Generic;
using System.Xml;
using UnityEngine;

//this class will handle the reading of the xml file
namespace Assets.Code.Scripts
{
    public class DeckPolitics : MonoBehaviour 
    {
        //list of strings
        static public string popularityOption1 = ""; 
        static public string popularityOption2 = ""; 
        static public string goldOption1 = ""; 
        static public string goldOption2 = ""; 
        static public string influenceOption1 = ""; 
        static public string influenceOption2 = ""; 
        static public string pietyOption1 = ""; 
        static public string pietyOption2 = ""; 
        static public string choiceOption1 = ""; 
        static public string choiceOption2 = ""; 
        static public string titleName = ""; 
        static public string description = "";
        static public string historicalText = "";
        static public string outcome1 = ""; 
        static public string outcome2 = ""; 
        //prefabs 
        public CardPolitics prefabCard; 
        //list of cards
        public List<CardPolitics> politicalCards; 
        public List<CardPolitics> resolvedPoliticalCards; 
        //reference to xml file
        public TextAsset politicalEventsXML;
        //list of dictionary cards
        public List<Dictionary<string,string>> cards; 
        public Dictionary<string,string> text;

        void Start()
        {
            //instantiates new list dictionaries
            cards = new List<Dictionary<string, string>> (); 
        }


        //called to initialize the deck
        public void InitDeck()
        {
            ReadDeck (); 
            MakeCards (); 
        }
	
        //make the card game objects
        public void MakeCards()
        {
            //loops through the cards list
            for (int i = 0; i < cards.Count; i++)
            {
                //instantiates a new card prefab
                politicalCards.Add (Instantiate (prefabCard) as CardPolitics); 
                //allows the card to be assigned as a child of the Politcal stuffs in the heirarchy 
                politicalCards [politicalCards.Count - 1].transform.parent = GameObject.Find ("Political").transform; //<= politicalCards.coun - 1 allows us to find the last index in the arry
                //title
                cards [politicalCards.Count - 1].TryGetValue ("title", out titleName); //plugs in the stuff from the xml file, gives out the variable as declared above
                //description
                cards[politicalCards.Count - 1].TryGetValue("description",out description);
                //historical text 
                cards[politicalCards.Count-1].TryGetValue("history", out historicalText); 
                //options
                cards[politicalCards.Count - 1].TryGetValue("Option1",out choiceOption1);
                cards [politicalCards.Count - 1].TryGetValue ("Option2", out choiceOption2); 
                //outcomes
                cards[politicalCards.Count -1].TryGetValue("Outcome1", out outcome1); 
                cards[politicalCards.Count -1].TryGetValue("Outcome2", out outcome2); 
                //ALL THE STATS
                cards [politicalCards.Count -1].TryGetValue("Popularity1", out popularityOption1);
                cards[politicalCards.Count -1].TryGetValue("Popularity2", out popularityOption2); 
                cards[politicalCards.Count -1].TryGetValue("Gold1", out goldOption1); 
                cards[politicalCards.Count -1].TryGetValue("Gold2", out goldOption2);
                cards[politicalCards.Count -1].TryGetValue("Influence1", out influenceOption1);
                cards[politicalCards.Count - 1].TryGetValue("Influence2",  out influenceOption2); 
                cards[politicalCards.Count -1].TryGetValue("Piety1", out pietyOption1); 
                cards[politicalCards.Count -1].TryGetValue("Piety2", out pietyOption2); 
                //assign all the information to it
                politicalCards [politicalCards.Count - 1].AssignValues (titleName, description, historicalText, choiceOption1, choiceOption2,outcome1,outcome2, popularityOption1, popularityOption2, goldOption1, goldOption2,influenceOption1, influenceOption2,pietyOption1,pietyOption2);
            } 
        }
        //deals with parsing the xml file 
        public void ReadDeck()
        {
            //new list of dictionaries
            cards = new List<Dictionary<string, string>> ();
            //creates new xml document
            XmlDocument xmlDoc = new XmlDocument();
            //load the file
            xmlDoc.LoadXml (politicalEventsXML.text); 
            //create an XMLNodeList of all the card nodes
            XmlNodeList cardList = xmlDoc.GetElementsByTagName ("card");

            //iterates through the nodes for "card"s
            foreach (XmlNode cardInfo in cardList)
            {
                //Create a new XMLNodeList, for looping through all the child nodes of the card element
                XmlNodeList cardDetails = cardInfo.ChildNodes;
                //reference to the text dictionary, which will hold the information from the xml file, the key being the name of the element - value being the text within it
                text = new Dictionary<string,string>(); 
                //looks through the child nodes in the xml file, child elements of card
                foreach (XmlNode cardItems in cardDetails)
                {
                    //if the xmlNode is called 'title' 
                    if (cardItems.Name == "title")
                    {
                        text.Add("title",cardItems.InnerText); // put this in the dictionary.
                    }
                    //if the element is description
                    if(cardItems.Name == "description")
                    {
                        text.Add("description",cardItems.InnerText); // put this in the dictionary.
                    }
                    //if the element name is history
                    if (cardItems.Name == "history")
                    {
                        text.Add("history", cardItems.InnerText); //add the key history, the text value to the dictionary
                    }
                    //if the element name is choice
                    if (cardItems.Name == "choice")
                    {
                        switch(cardItems.Attributes["name"].Value) //attribute 'name' create cases of each type of name
                        {
                            case "Option1":
                                text.Add("Option1",cardItems.InnerText);  // put this in the dictionary.
                                break;
                            case "Option2":
                                text.Add("Option2", cardItems.InnerText); //put this in le dictionarty
                                break; 
                        }
                    }

                    //if the element if outcome
                    if (cardItems.Name == "outcome")
                    {
                        switch(cardItems.Attributes["name"].Value)
                        {
                            case "Outcome1":
                                text.Add("Outcome1", cardItems.InnerText); 
                                break; 
                            case "Outcome2":
                                text.Add("Outcome2", cardItems.InnerText); 
                                break; 
                        }
                    }
                    //if the element is popularity
                    if (cardItems.Name == "popularity")
                    {
                        switch (cardItems.Attributes["name"].Value)
                        {
                            case "Popularity1":
                                text.Add("Popularity1", cardItems.InnerText); 
                                break; 
                            case "Popularity2":
                                text.Add("Popularity2", cardItems.InnerText); 
                                break; 
                        }
                    }

                    if (cardItems.Name == "gold")
                    {
                        switch (cardItems.Attributes["name"].Value)
                        {
                            case "Gold1":
                                text.Add("Gold1", cardItems.InnerText); 
                                break; 
                            case "Gold2":
                                text.Add("Gold2", cardItems.InnerText); 
                                break; 
                        }
                    }

                    if (cardItems.Name == "influence")
                    {
                        switch (cardItems.Attributes["name"].Value)
                        {
                            case "Influence1":
                                text.Add("Influence1", cardItems.InnerText); 
                                break; 
                            case "Influence2":
                                text.Add("Influence2", cardItems.InnerText); 
                                break; 
                        }
                    }

                    if (cardItems.Name == "piety")
                    {
                        switch (cardItems.Attributes["name"].Value)
                        {
                            case "Piety1":
                                text.Add("Piety1", cardItems.InnerText); 
                                break; 
                            case "Piety2":
                                text.Add("Piety2", cardItems.InnerText); 
                                break; 
                        }
                    }
                }
                cards.Add (text); //add all the text dictionary to the cards[]
            }
        }
        //called when all political cards have been resolved
        public bool ResolvedAllPoliticalCards()
        {
            if (politicalCards.Count == 0) //checking if the list has no cards
            {
                return true; //this returns the value as true
            } 
            else 
            {
                return false; //if not, returns as false
            }
        }
        //called when the list from which cards are drawn empties
        public void RefillDeck()
        {
            //goes through the whole list of resolved cards, then adds each index to the current list
            for (int i = 0; i < resolvedPoliticalCards.Count; i++) 
            {
                politicalCards.Add(resolvedPoliticalCards[i]); //add resolved deck to draw deck
            }
            resolvedPoliticalCards.Clear (); //clears the resolved list
        }
    }
}








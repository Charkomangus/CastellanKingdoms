using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts
{
    public class CourtManager : MonoBehaviour
    {
        //reference to the gameData script
        private GameObject m_gameManager; 
        // State Manager Reference
        private StateManager m_stateManagerRef;

        //reference to the UI
        public UIManager uiManagerRef; 
        //active state
        //Buttons
        public Button papalAdvisor;
        public Button politicalAdvisor;
        public Button militaryAdvisor;
        //Sub-Buttons
        public Button yesPolitical; 
        public Button yesMilitary; 
        //Different state options
        public GameObject courtMain;
        public GameObject courtPolitcal;
        public GameObject courtPapal;
        //military panels
        public GameObject yesMilitaryPanel; 
        public GameObject noMilitaryPanel; 
        //Text Objects
        public Text militaryText;
        public Text militaryNoText; 
        public Text papalText;
        public Text politcalText;
        //Dialogue
        public string[] militaryDialogue;
        public string[] nopeMilitaryMan; 
        public string[] papalDialogue;
        public string[] politicalDialogue; 
        //EVENTS
        public DeckPolitics deckPoliticsRef;
        public DeckPapal deckPapalRef;
        public CardPolitics polCard;
        public CardPapal papCard; 
        //random index integer
        public int randomIndex;
        //text objects for political cards
        public Text titleText;
        public Text descriptionText;
        public Text historicalText; 
        public Text option1Text;
        public Text option2Text; 
        public Text outcome1Text; 
        public Text outcome2Text; 
        //button options for political cards
        public Button option1Button;
        public Button option2Button; 
        public GameObject option1OutcomePage; 
        public GameObject option2OutcomePage; 
        //papal buttons
        public Button[] papalCardButtons;
        public Text[] papalCardTitle;
        public Text[] papalCardDes; 
        public Text[] papalCardHis; 
        public Text metCost; 
        public Text failedCost; 
        public string[] metCostWords; 
        public string[] failedCostWords;
        public GameObject successScreen; 
        public GameObject failedScreen; 
        //values
        public bool canPay;  

        //called on the start up 
        void Awake ()
        {
            m_gameManager = GameObject.Find ("GameManager"); 
            m_stateManagerRef = m_gameManager.GetComponent<StateManager> ();
            deckPoliticsRef = m_gameManager.GetComponent<DeckPolitics> (); 
            deckPapalRef = m_gameManager.GetComponent<DeckPapal> (); 
        }

        void Start()
        {
            //If the phasing player is the first in the turn sequence
            if (m_stateManagerRef.PhasingPlayerIndex == 0) {
                //call to the method to draw new papal cards
                deckPapalRef.DrawPapalCards (); 
            }
        }

	
        //checks to see which cards have been played, makes those uninteractable
        public void CheckPapalCards()
        {
            //loops through the selected cards to see if they've played
            for (int i = 0; i < deckPapalRef.selectedPapalCard.Length; i++) {
                if (deckPapalRef.selectedPapalCard[i].Played)
                {
                    //sets the buttons to being non interactable if it's been played
                    papalCardButtons[i].interactable = false; 
                }
                else
                {
                    papalCardButtons[i].interactable = true; //set card being interactable
                }
            }
        }
	
        //called when the player agrees to the papal Adivsor
        public void YesMrPope ()
        {
            //gives the title, history and description to the papal cards in the array
            for (int i = 0; i < deckPapalRef.selectedPapalCard.Length; i++) {
                papalCardTitle [i].text = deckPapalRef.selectedPapalCard [i].title; 
                papalCardDes [i].text = deckPapalRef.selectedPapalCard [i].description; 
                papalCardHis[i].text = deckPapalRef.selectedPapalCard[i].papalHistory; 
            }
            //checks to set non interactable cards
            CheckPapalCards (); 
        }
        //checks to see if all the advisors are clicked
        public bool AllAdvisorsClicked()
        {
            bool advisorsClicked; 
            advisorsClicked = false; 
            if (papalAdvisor.interactable == false && politicalAdvisor.interactable == false) {
                advisorsClicked = true;  
            } 
            if (advisorsClicked == true) {
                return true; 
            } else {
                return false; 
            }
        }

        public void MilitaryAdvisorClicked()
        {
            //if all the advisors have been clicked
            if (AllAdvisorsClicked ()) {
                yesMilitaryPanel.SetActive(true); //activate the panel has yes stuff
                yesMilitary.interactable = true; //set change scene button as true
                noMilitaryPanel.SetActive(false); 
                militaryText.text = militaryDialogue [Random.Range (0, militaryDialogue.Length)]; //show random military text 
            } else {
                noMilitaryPanel.SetActive(true); 
                yesMilitary.interactable = false;
                yesMilitaryPanel.SetActive(false); 
                militaryNoText.text = nopeMilitaryMan[Random.Range(0, nopeMilitaryMan.Length)]; 
            }
        }

        public void AdvisorClicked (string whatType)
        {
            //if you click on the papal advisor
            if (whatType == "Papal") {
                papalText.text = papalDialogue [Random.Range (0, papalDialogue.Length)]; //show random papal dialogue
            }
            //if political advisor is clicked
            if (whatType == "Political") {
                politcalText.text = politicalDialogue [Random.Range (0, politicalDialogue.Length)]; //show random political dialogue
            }
        }

        public void DrawPoliticalCard ()
        {
            //reference to random index
            randomIndex = Random.Range (0, deckPoliticsRef.politicalCards.Count); 
            //assign the polCard to the index from political cards
            polCard = deckPoliticsRef.politicalCards [randomIndex]; 
            //text objects
            titleText.text = polCard.title; 
            descriptionText.text = polCard.description; 
            historicalText.text = polCard.history; 
            option1Text.text = polCard.option1; 
            option2Text.text = polCard.option2; 
            outcome1Text.text = polCard.theOutcome1; 
            outcome2Text.text = polCard.theOutcome2; 
        }
	
        //deals with altering the players stats based on the political cards
        public void GiveOutcome (string buttonPressed)
        {
            //inflicts the changes as stated by option1, adding values to the relevant stat
            if (buttonPressed == "Option1") {
                m_stateManagerRef.PhasingPlayer.AddToPopularity (polCard.pop1); 
                m_stateManagerRef.PhasingPlayer.AddToGold (polCard.gold1); 
                m_stateManagerRef.PhasingPlayer.AddToInfluence (polCard.inf1); 
                m_stateManagerRef.PhasingPlayer.AddToPiety (polCard.piety1); 
                //sets the option1 outcome panel active
                option1OutcomePage.SetActive(true); 
            }
            //inflicts the changes as stated by option2, adding values to the relevant stat
            if (buttonPressed == "Option2") {
                m_stateManagerRef.PhasingPlayer.AddToPopularity (polCard.pop2); 
                m_stateManagerRef.PhasingPlayer.AddToGold (polCard.gold2); 
                m_stateManagerRef.PhasingPlayer.AddToInfluence (polCard.inf2);
                m_stateManagerRef.PhasingPlayer.AddToPiety (polCard.piety2);
                //sets the outcome2 panel active
                option2OutcomePage.SetActive(true); 

            }
            //updates the UI to show the changes
            m_stateManagerRef.uiManagerRef.UpdateUI (); 

            //add to resolved cards
            deckPoliticsRef.resolvedPoliticalCards.Add (polCard); 
            deckPoliticsRef.politicalCards.Remove (polCard); 

            if (deckPoliticsRef.ResolvedAllPoliticalCards ()) {//if all the cards have been used
                deckPoliticsRef.RefillDeck (); //call the refill deck method
            }
        }
	
        //called when the cost of a card needs to be checked
        public bool CheckPapalCost (CardPapal papCard)
        {
            bool playerMeetsCost; 
            playerMeetsCost = true; 
            //if the players stats are less than that stated on the xml
            if ((papCard.costGold > m_stateManagerRef.PhasingPlayer.Gold) || (papCard.costPiety > m_stateManagerRef.PhasingPlayer.Piety) || (papCard.costPopularity > m_stateManagerRef.PhasingPlayer.Popularity) || (papCard.costInfluence > m_stateManagerRef.PhasingPlayer.Influence)) {
                playerMeetsCost = false; //set the playerMets cost to false
            }

            if (playerMeetsCost == true) { //if the player does meet the cost
                return true; //return true
            } else {
                return false; 
            }

        }
        //called on the click of a chosen card, applies changes to the player's stats
        public void DealPapalValue (string slotSelected)
        {
            //checks to see which vard is selected
            if (slotSelected == "Slot 1") {
                //checks to see whether the player meets the cost
                if (CheckPapalCost (deckPapalRef.selectedPapalCard [0])) {
                    //if thet player meets the cost, apply the reward values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [0].rewardGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [0].rewardInfluence);
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [0].rewardPiety); 
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [0].rewardPopularity); 
                    successScreen.SetActive(true); 
                    metCost.text = metCostWords[Random.Range (0, metCostWords.Length)]; 
                } else {
                    //if the player does not meet the cost, apply the penalty values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [0].penaltyGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [0].penaltyInfluence); 
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [0].penaltyPiety);
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [0].penaltyPopularity); 
                    failedCost.text = failedCostWords[Random.Range (0, failedCostWords.Length)]; 
                    failedScreen.SetActive (true); 
                }
                //sets the card to played
                deckPapalRef.selectedPapalCard[0].SetPlayed(true); 
                //makes its button uninteractable
                papalCardButtons[0].interactable = false; 
            }

            if (slotSelected == "Slot 2") {
                //checks to see whether the player meets the cost
                if (CheckPapalCost (deckPapalRef.selectedPapalCard [1])) {
                    //if thet player meets the cost, apply the reward values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [1].rewardGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [1].rewardInfluence);
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [1].rewardPiety); 
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [1].rewardPopularity);
                    successScreen.SetActive(true); 
                    metCost.text = metCostWords[Random.Range (0, metCostWords.Length)]; 
                } else {
                    //if the player does not meet the cost, apply the penalty values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [1].penaltyGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [1].penaltyInfluence); 
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [1].penaltyPiety);
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [1].penaltyPopularity); 
                    failedScreen.SetActive(true); 
                    failedCost.text = failedCostWords[Random.Range (0, failedCostWords.Length)]; 
                }
                //sets the card to played
                deckPapalRef.selectedPapalCard[1].SetPlayed(true); 
                //makes its button uninteractable
                papalCardButtons[1].interactable = false; 
            }

            if (slotSelected == "Slot 3") {
                //checks to see whether the player meets the cost
                if (CheckPapalCost (deckPapalRef.selectedPapalCard [2])) {
                    //if thet player meets the cost, apply the reward values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [2].rewardGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [2].rewardInfluence);
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [2].rewardPiety); 
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [2].rewardPopularity); 
                    successScreen.SetActive(true); 
                    metCost.text = metCostWords[Random.Range (0, metCostWords.Length)]; 
                } else {
                    //if the player does not meet the cost, apply the penalty values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [2].penaltyGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [2].penaltyInfluence); 
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [2].penaltyPiety);
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [2].penaltyPopularity); 
                    failedScreen.SetActive (true); 
                    failedCost.text = failedCostWords[Random.Range (0, failedCostWords.Length)]; 
                }
                //sets the card to played
                deckPapalRef.selectedPapalCard[2].SetPlayed(true); 
                //makes its button uninteractable
                papalCardButtons[2].interactable = false; 
            }

            if (slotSelected == "Slot 4") {
                //checks to see whether the player meets the cost
                if (CheckPapalCost (deckPapalRef.selectedPapalCard [3])) {
                    //if thet player meets the cost, apply the reward values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [3].rewardGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [3].rewardInfluence);
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [3].rewardPiety); 
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [3].rewardPopularity); 
                    successScreen.SetActive(true); 
                    metCost.text = metCostWords[Random.Range (0, metCostWords.Length)]; 
                } else {
                    //if the player does not meet the cost, apply the penalty values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [3].penaltyGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [3].penaltyInfluence); 
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [3].penaltyPiety);
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [3].penaltyPopularity); 
                    failedScreen.SetActive(true); 
                    failedCost.text = failedCostWords[Random.Range (0, failedCostWords.Length)]; 
                }
                //sets the card to played
                deckPapalRef.selectedPapalCard[3].SetPlayed(true); 
                //makes its button uninteractable
                papalCardButtons[3].interactable = false; 
            }

            if (slotSelected == "Slot 5") {
                //checks to see whether the player meets the cost
                if (CheckPapalCost (deckPapalRef.selectedPapalCard [4])) {
                    //if thet player meets the cost, apply the reward values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [4].rewardGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [4].rewardInfluence);
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [4].rewardPiety); 
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [4].rewardPopularity);
                    successScreen.SetActive(true); 
                    metCost.text = metCostWords[Random.Range (0, metCostWords.Length)]; 
                } else {
                    //if the player does not meet the cost, apply the penalty values
                    m_stateManagerRef.PhasingPlayer.AddToGold (deckPapalRef.selectedPapalCard [4].penaltyGold); 
                    m_stateManagerRef.PhasingPlayer.AddToInfluence (deckPapalRef.selectedPapalCard [4].penaltyInfluence); 
                    m_stateManagerRef.PhasingPlayer.AddToPiety (deckPapalRef.selectedPapalCard [4].penaltyPiety);
                    m_stateManagerRef.PhasingPlayer.AddToPopularity (deckPapalRef.selectedPapalCard [4].penaltyPopularity);
                    failedScreen.SetActive(true); 
                    failedCost.text = failedCostWords[Random.Range (0, failedCostWords.Length)]; 
                } 
                //sets the card to played
                deckPapalRef.selectedPapalCard[4].SetPlayed(true); 
                //makes its button uninteractable
                papalCardButtons[4].interactable = false; 
            }
            m_stateManagerRef.uiManagerRef.UpdateUI (); 
        }
        //called when the player wants to move to the conquest state
        public void ChangeScene ()
        {
            m_stateManagerRef.SwitchToConquest ();
        }
    }
}
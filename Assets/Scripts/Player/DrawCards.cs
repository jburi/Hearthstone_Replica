using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject card;
    public GameObject initCard;
    
    public GameObject playerArea;
    public GameObject oppArea;
    
    public int playerHandSize;
    public int oppHandSize;
    public int currentCard;
    public int oppCurrentCard;

    public GameObject[] Slots;

    public List<Card> playerDeck;
    public List<Card> oppDeck;

    Complete.GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = Complete.GameManager.gm;
        playerDeck = gm.MyDeckCards;
        oppDeck = gm.AIDeckCards;
        playerHandSize = 0;
        oppHandSize = 0;
        currentCard = 0;
        oppCurrentCard = 0;

        //Initial Draw
        for (int i = 0; i < 3; i++)
        {
            Draw();
            CreateOppCard();
        }
    }

    /// <summary>
    /// Checks if the templates in the slots are to be discarded.
    /// If so, then the card is removed from the deck
    /// 
    ///  The Slots and the Deck are different so a second counter is needed.
    /// </summary>
    public void EndInitialDraw()
    {
        //Get the deck and set the deck card counter
        currentCard = 0;

        //Parse through the three slots
        for (int i = 0; i < 3; i++)
        {
            //If the template in the slot will be discarded
            if (!Slots[i].GetComponentInChildren<DrawSelect>().selected)
            {
                Debug.Log("Discard");
                //Create a copy of the card and put it at the end of the deck
                Card temp = playerDeck.ElementAt<Card>(currentCard);
                playerDeck.RemoveAt(currentCard);
                playerDeck.Add(temp);
            }
            else
            {
                Debug.Log("Parse");
                //Parse through the deck
                currentCard++;
            }
        }
    }

    public void Draw()
    {
        if (gm.turn == Complete.GameManager.Turn.MyTurn)
        {
            if (gm.MyHandCards.Count() >= 9)
            {
                Debug.LogError("Max Hand Size is 9");
            }
            else if (currentCard == playerDeck.Count())
            {
                Debug.LogError("No More Cards in the Deck");
            }
            else
            {
                CreatePlayerCard();
            }
        }
        else if (gm.turn == Complete.GameManager.Turn.AITurn)
        {
            if (gm.AIHandCards.Count() >= 9)
            {
                Debug.LogError("Max Hand Size is 9");
            }
            else if (oppCurrentCard == oppDeck.Count())
            {
                Debug.LogError("No More Cards in the Deck");
            }
            else
            {
                CreateOppCard();
            }
        }
        else
        {
            Debug.LogError("Turn is Null");
        }
        
    }

    public void CreatePlayerCard()
    {
        //UnityEngine.Random.Range(0, cards.Count)
        Card cardRef = playerDeck[currentCard]; ;
        Card newCard = cardRef.Clone();
        GameObject playerCard;

        if (gm.initialDraw == true)
        {
            playerCard = Instantiate(initCard, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.GetComponent<CardDisplay>().card = newCard as Card;
            RectTransform rect = playerCard.GetComponent<RectTransform>();
            rect.localScale = new Vector3(1f, 1f, 1f);
            playerCard.transform.SetParent(Slots[currentCard].transform, false);
        }
        else if (newCard.cardType == CardSO.CardType.Monster)
        {
            playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.GetComponent<CardDisplay>().card = newCard as Card;
            playerCard.GetComponent<CardDisplay>().card.SetCardStatus(CardSO.CardStatus.InHand);
            playerCard.transform.SetParent(playerArea.transform, false);

            gm.MyHandCards.Add(playerCard);
        }
        else
        {
            playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.GetComponent<CardDisplay>().card = newCard;
            //playerCard.GetComponent<CardDisplay>().currentState = CardDisplay.State.hand;
            playerCard.GetComponent<CardDisplay>().card.SetCardStatus(CardSO.CardStatus.InHand);
            playerCard.transform.SetParent(playerArea.transform, false);

            gm.MyHandCards.Add(playerCard);
        }

        playerCard.GetComponent<CardDisplay>().card.owner = Card.Owner.My;
        currentCard++;
    }

    public void CreateOppCard()
    {
        //UnityEngine.Random.Range(0, cards.Count)

        Card cardRef = oppDeck[oppCurrentCard];
        Card newCard = cardRef.Clone();
        GameObject oppCard;

        if (newCard.cardType == CardSO.CardType.Monster)
        {
            oppCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
            oppCard.GetComponent<CardDisplay>().card = newCard as Card;
            //Change Later
            oppCard.GetComponent<CardDisplay>().currentState = CardDisplay.State.board;

            gm.AIHandCards.Add(oppCard);
        }
        else
        {
            oppCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
            oppCard.GetComponent<CardDisplay>().card = newCard;

            gm.AIHandCards.Add(oppCard);
        }

        oppCard.transform.SetParent(oppArea.transform, false);
        oppCard.GetComponent<CardDisplay>().card.owner = Card.Owner.AI;
        Destroy(oppCard.GetComponent<CardZoom>());
        Destroy(oppCard.GetComponent<DragDrop>());
        oppCurrentCard++;
    }
}

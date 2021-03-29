using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAttack : MonoBehaviour
{
    public bool canAttack = false;
    public bool isOverCard = false;
    public Slider AttackSlider;
    public CardSO.Owner cardOwner;

    Player playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerHealth").GetComponent<Player>();

        //If this GameObject is a card
        if(gameObject.GetComponent<CardDisplay>() == true)
        {
            cardOwner = gameObject.GetComponent<CardDisplay>().card.owner;
        }
        //Otherwise, this must be a player
        else
        {
            //cardOwner = gameObject.GetComponent<Player>().playerName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<CardDisplay>().currentState == CardDisplay.State.board)
        {
            canAttack = true;
        }
    }
    /*
    public void SelectCard()
    {
        // If the card is on the board ...
        if (canAttack == true)
        {
            //Select your card to attack with
            if (cardOwner == CardSO.Owner.My)
            {
                playerManager.playerCreature = gameObject;
            }
            //Select card you are attacking
            else if (cardOwner == CardSO.Owner.AI && playerManager.playerCreature != null)
            {
                playerManager.oppCreature = gameObject;
                Debug.Log("Attacking");
                Attack();
            }
            //There is no card you are attacking with
            else
            {
                Debug.Log("Select a card to attack with first");
            }
        }
        //The card clicked on is not on the board
        else
        {
            Debug.Log("Play card from hand");
        }
    }

    private void Attack()
    {
        GameObject playerCard = playerManager.playerCreature.gameObject;
        GameObject oppCard = playerManager.oppCreature.gameObject;

        int oppHealth = oppCard.GetComponent<CardDisplay>().card.health;
        int playerAttack = playerCard.GetComponent<CardDisplay>().card.attack;

        Debug.Log("Opponent Creature Health: " + oppHealth);
        Debug.Log("Player Creature Attack: " + playerAttack);

        oppHealth -= playerAttack;

        oppCard.GetComponent<CardDisplay>().card.health = oppHealth;

        Debug.Log("Opponent Creature after battle: " + oppCard.GetComponent<CardDisplay>().card.health);

        if (oppHealth <= 0)
        {
            //Destroy CardZoom that spawns on hover
            if (oppCard.GetComponent<CardZoom>().zoomCard != null)
            {
                Destroy(oppCard.GetComponent<CardZoom>().zoomCard);
            }
            Destroy(oppCard);
        }
    }
    */
}

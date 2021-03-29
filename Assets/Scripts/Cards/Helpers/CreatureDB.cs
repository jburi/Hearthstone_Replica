using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureDB : MonoBehaviour
{
    //List of Creature Cards
    public List<Card> creatures = new List<Card>();

    /// <summary>
    /// Since scriptable objects hold their modified values ...
    /// their values must be reset at the start of each game.
    /// </summary>
    void Start()
    {
        // Reset creature health and attack if modified from last game
        foreach (Card card in creatures)
        {
            card.attack = card.defaultAttack;
            card.health = card.defaultHealth;
        }
    }
}

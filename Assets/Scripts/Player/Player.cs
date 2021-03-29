using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health;
    public string _name;
    public bool canAttack;
    public int attack;

    public Text healthText;
    public Text AttackText;
    public Text DebugText;

    public delegate void CustomAction();

    // Start is called before the first frame update
    void Awake()
    {
        if (gameObject.name == "Player")
        {
            name = "Player";
        }
        else if (gameObject.name == "AI")
        {
            name = "AI";
        }
        health = 20;
        canAttack = false;
        attack = 0;
    }

    public void EnableControl()
    {

    }

    public void DisableControl()
    {

    }

    public Player Clone()
    {
        Player temp = gameObject.AddComponent<Player>();
        temp._name = _name;
        temp.health = health;
        temp.canAttack = canAttack;
        temp.attack = attack;

        return temp;
    }
}

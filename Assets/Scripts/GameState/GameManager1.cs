using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles permissions on each turn
public enum Turn {
    Idle,
    PlayerDraw,
    PlayerMP1,
    PlayerAttack,
    PlayerMP2,
    PlayerEnd,
    OppDraw,
    OppMP1,
    OppAttack,
    OppMP2,
    OppEnd,
}

public class GameManager : MonoBehaviour
{
    //Used to set the current turn state
    private Turn currentTurn = Turn.Idle;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Controls permissions for each player based on what turn it is currently
        switch (currentTurn.ToString())
        {
            case "Idle":
                break;

            case "PlayerDraw":
                break;

            case "PlayerMP1":
                break;

            case "PlayerAttack":
                break;

            case "PlayerMP2":
                break;

            case "PlayerEnd":
                break;

            case "OppDraw":
                break;

            case "OppMP1":
                break;

            case "OppAttack":
                break;

            case "OppMP2":
                break;

            case "OppEnd":
                break;
        }
    }
}

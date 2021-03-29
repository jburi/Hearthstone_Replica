using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    //Getters and Setters for each state
    public GameState playerDrawState { get; set; }
    public GameState playerMP1State { get; set; }
    public GameState playerAttackState { get; set; }
    public GameState playerMP2State { get; set; }
    public GameState playerEndState { get; set; }
    public GameState oppDrawState { get; set; }
    public GameState oppMP1State { get; set; }
    public GameState oppAttackState { get; set; }
    public GameState oppMP2State { get; set; }
    public GameState oppEndState { get; set; }
    public GameState idleState { get; set; }
    public GameState currentState { get; set; }

    // Initialize the states
    void Awake()
    {
        //Player
        playerDrawState = gameObject.AddComponent<PlayerDrawState>();
        playerMP1State = gameObject.AddComponent<PlayerMP1State>();
        playerAttackState = gameObject.AddComponent<PlayerAttackState>();
        playerMP2State = gameObject.AddComponent<PlayerMP2State>();
        playerEndState = gameObject.AddComponent<PlayerEndState>();

        //Opponent
        oppDrawState = gameObject.AddComponent<OppDrawState>();
        oppMP1State = gameObject.AddComponent<OppMP1State>();
        oppAttackState = gameObject.AddComponent<OppAttackState>();
        oppMP2State = gameObject.AddComponent<OppMP2State>();
        oppEndState = gameObject.AddComponent<OppEndState>();

        //GameOver
        idleState = gameObject.AddComponent<IdleState>();
    }

    //Used to call the methods of the current state
    public void PlayerDraw() { currentState.PlayerDraw(); }
    public void PlayerMP1() { currentState.PlayerMP1(); }
    public void PlayerAttack() { currentState.PlayerAttack(); }
    public void PlayerMP2() { currentState.PlayerMP2(); }
    public void PlayerEnd() { currentState.PlayerEnd(); }
    public void OppDraw() { currentState.OppDraw(); }
    public void OppMP1() { currentState.OppMP1(); }
    public void OppAttack() { currentState.OppAttack(); }
    public void OppMP2() { currentState.OppMP2(); }
    public void OppEnd() { currentState.OppEnd(); }
    public void Idle() { currentState.Idle(); }
}

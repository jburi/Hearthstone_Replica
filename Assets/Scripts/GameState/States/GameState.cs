using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState : MonoBehaviour
{
    public abstract void PlayerDraw();
    public abstract void PlayerMP1();
    public abstract void PlayerAttack();
    public abstract void PlayerMP2();
    public abstract void PlayerEnd();
    public abstract void OppDraw();
    public abstract void OppMP1();
    public abstract void OppAttack();
    public abstract void OppMP2();
    public abstract void OppEnd();
    public abstract void Idle();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected enum enemyState
    {
        idle,
        moving,
        attacking,
        recovering
    }

    protected enemyState currentState;

    protected int health;

    public float expValue;

    public virtual void takeDamage(int d) { }

    protected virtual void die() { }

    protected virtual void attack() { }
}

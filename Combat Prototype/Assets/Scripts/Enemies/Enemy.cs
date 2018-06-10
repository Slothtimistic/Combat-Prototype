using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public float expValue;

    public virtual void takeDamage(int d) { }

    protected virtual void die() { }

    protected virtual void attack() { }

    protected Vector3 findNavPosition(float dist)
    {

        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 randomPoint = new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle));

        Vector3 randomDir = new Vector3(randomPoint.x, 0, randomPoint.y) * dist;

        randomDir += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDir, out hit, 10, 1))
            return hit.position;
        else
            return Vector3.zero;
    }
}

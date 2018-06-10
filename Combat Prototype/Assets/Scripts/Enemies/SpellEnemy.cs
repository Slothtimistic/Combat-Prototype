using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpellEnemy : PrototypeEnemy
{

    public Animator mageAnimator;
    public GameObject spell;

    public float rangedAttackRange;

    Vector3 startPos;
    float v;
    bool setup, justAttacked;

    protected new void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    public void castSpell()
    {
        GameObject spellObject = Instantiate(spell, transform);

        spellObject.transform.localPosition = new Vector3(-1.6f, 1, 0.5f);
        spellObject.transform.parent = null;
        spellObject.transform.LookAt(playerObject.transform);

        mageAnimator.SetBool("casting", false);
    }

    public void Recalculate()
    {
        if (currentHealth > 0)
        {
            justAttacked = true;
            agent.isStopped = false;

            Vector3 finalPos = findNavPosition(Random.Range(5, 10));

            if (finalPos == Vector3.zero)
                finalPos = transform.position;

            agent.destination = finalPos;

            currentState = enemyState.moving;
        }
    }

    private void resetAnimator()
    {
        foreach (AnimatorControllerParameter param in mageAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
                mageAnimator.SetBool(param.name, false);
        }
    }

    public void startMoving()
    {
        currentState = enemyState.moving;
    }

    protected new void Update()
    {
        base.Update();

        float distFromPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        setup = mageAnimator.GetCurrentAnimatorStateInfo(0).IsName("Staff_Cast") && currentHealth > 0;

        if (currentState == enemyState.idle && distFromPlayer < visionRadius)
        {
            mageAnimator.SetBool("inCombat", true);
        }

        if (currentState == enemyState.moving && distFromPlayer < rangedAttackRange & !justAttacked)
        {
            currentState = enemyState.attacking;
            mageAnimator.SetBool("casting", true);
        }

        if (agent.isActiveAndEnabled)
        {
            if (currentState == enemyState.moving && justAttacked)
            {
                if (agent.remainingDistance < 1)
                {
                    currentState = enemyState.attacking;
                    mageAnimator.SetBool("casting", true);
                    justAttacked = false;
                }
            }

            if (currentState == enemyState.moving && distFromPlayer < visionRadius & !justAttacked)
            {
                agent.isStopped = false;
                agent.destination = playerObject.transform.position;
            }

            if (currentState == enemyState.attacking & !agent.isStopped)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
        }

        if (setup)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerObject.transform.position - transform.position);
            transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, ref v, 0.1f), 0);
        }
    }

    protected override void die()
    {
        base.die();

        resetAnimator();
        mageAnimator.SetBool("dead", true);
    }

}

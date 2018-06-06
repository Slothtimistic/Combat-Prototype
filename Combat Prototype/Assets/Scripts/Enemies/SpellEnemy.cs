using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEnemy : PrototypeEnemy
{

    public Animator mageAnimator;
    public GameObject spell;

    public float rangedAttackRange;

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
        mageAnimator.SetBool("casting", true);
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

    private void Update()
    {

        float distFromPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if(currentState == enemyState.idle && distFromPlayer < visionRadius)
        {
            mageAnimator.SetBool("inCombat", true);
        }

        if(currentState == enemyState.moving && distFromPlayer < rangedAttackRange)
        {
            currentState = enemyState.attacking;
            mageAnimator.SetBool("casting", true);
        }

        if(agent.isActiveAndEnabled)
        {
            if(currentState == enemyState.moving && distFromPlayer < visionRadius)
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
    }

    protected override void die()
    {
        base.die();

        resetAnimator();
        mageAnimator.SetBool("dead", true);
    }

}

using System.Collections;
using UnityEngine;

public class SlowEnemy : PrototypeEnemy
{

    public Animator slowAnimator;
    public BoxCollider weaponCollider;


    Vector3 startPos;


    float attackRange = 2, v;
    bool idle, setup, attacking, recovering, stunned;

    protected new void Update()
    {
        base.Update();

        AnimatorStateInfo currentAnimation = slowAnimator.GetCurrentAnimatorStateInfo(0);

        idle = currentAnimation.IsName("Idle");
        setup = currentAnimation.IsName("SwingSetup");
        attacking = currentAnimation.IsName("Swing");
        recovering = currentAnimation.IsName("SwingRecovery");

        if (currentHealth > 0)
            weaponCollider.enabled = attacking;

        float distFromPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distFromPlayer < visionRadius && distFromPlayer > attackRange && (currentState != enemyState.attacking && currentState != enemyState.recovering))
            currentState = enemyState.moving;

        if (distFromPlayer <= attackRange)
            currentState = enemyState.attacking;

        if (agent.isActiveAndEnabled)
        {
            if (distFromPlayer < visionRadius && currentState == enemyState.moving)
            {
                agent.isStopped = false;
                agent.destination = playerObject.transform.position;
            }

            if (currentState == enemyState.attacking & !agent.isStopped)
            {
                resetAnimator();
                slowAnimator.SetBool("attack", true);
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

    public void Reasses()
    {
        currentState = enemyState.recovering;
        resetAnimator();
        StartCoroutine(afterAttack());
    }

    IEnumerator afterAttack()
    {
        yield return new WaitForSeconds(0.5f);

        if (Vector3.Distance(transform.position, playerObject.transform.position) > attackRange)
            currentState = enemyState.moving;
        else
        {
            currentState = enemyState.attacking;
            transform.LookAt(new Vector3(playerObject.transform.position.x, transform.position.y, playerObject.transform.position.z));
            slowAnimator.SetBool("attack", true);
        }
    }

    private void resetAnimator()
    {
        foreach (AnimatorControllerParameter param in slowAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
                slowAnimator.SetBool(param.name, false);
        }
    }

    public bool isAttacking()
    {
        return attacking;
    }

    public void gotBlocked()
    {
        resetAnimator();
        slowAnimator.SetBool("blocked", true);
    }

    public void landedHit()
    {
        resetAnimator();
        slowAnimator.SetBool("landed", true);
    }

    protected override void die()
    {
        base.die();
        if (agent.isActiveAndEnabled)
            agent.isStopped = true;
        resetAnimator();
        slowAnimator.SetBool("dead", true);
    }

}

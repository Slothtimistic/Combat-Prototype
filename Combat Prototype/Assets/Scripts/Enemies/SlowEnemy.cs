using System.Collections;
using UnityEngine;

public class SlowEnemy : PrototypeEnemy
{

    public Animator slowAnimator;
    public BoxCollider weaponCollider;
    public SpriteRenderer healthBar, healthBackground;

    Vector3 startPos;

    Vector3 _3health = new Vector3(1, 0.1f, 1), _2health = new Vector3(0.66f, 0.1f, 1), _1health = new Vector3(0.33f, 0.1f, 1);

    float attackRange = 2;
    bool idle, setup, attacking, recovering, stunned;

    protected new void Start()
    {
        base.Start();
        health = 3;
    }

    private void Update()
    {
        AnimatorStateInfo currentAnimation = slowAnimator.GetCurrentAnimatorStateInfo(0);

        idle = currentAnimation.IsName("Idle");
        attacking = currentAnimation.IsName("Swing");
        recovering = currentAnimation.IsName("SwingRecovery");

        weaponCollider.enabled = attacking;

        float distFromPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distFromPlayer < visionRadius && distFromPlayer > attackRange && (currentState != enemyState.attacking && currentState != enemyState.recovering))
            currentState = enemyState.moving;

        if (distFromPlayer <= attackRange)
            currentState = enemyState.attacking;

        if(agent.isActiveAndEnabled)
        {
            if (distFromPlayer < visionRadius && currentState == enemyState.moving)
            {
                agent.isStopped = false;
                agent.destination = playerObject.transform.position;
            }

            if (currentState == enemyState.attacking &! agent.isStopped)
            {
                resetAnimator();
                slowAnimator.SetBool("attack", true);
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
        }

        if(health == 3)
        {
            //healthBar.transform.localScale = _3health;
            healthBar.enabled = false;
            healthBackground.enabled = false;
        }
        else if (health == 2)
        {
            healthBar.transform.localScale = _2health;
            healthBar.enabled = true;
            healthBackground.enabled = true;
        }
        else if (health == 1)
        {
            healthBar.transform.localScale = _1health;
            healthBar.enabled = true;
            healthBackground.enabled = true;
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
            if(param.type == AnimatorControllerParameterType.Bool)
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
        healthBar.enabled = false;
        healthBackground.enabled = false;
        resetAnimator();
        slowAnimator.SetBool("dead", true);
    }

}

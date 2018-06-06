using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainee : Player
{

    public static Trainee thisTrainee;

    public Animator swordAnimator;
    public BoxCollider damageCollider, blockCollider;

    public bool breakAttack;
    private bool queuedAttack, attackAnimations, walkAnimation, idleAnimation;

    protected new void Awake()
    {
        base.Awake();
        thisTrainee = this;
    }

    protected new void Start()
    {
        base.Start();
        damageCollider.enabled = false;
    }

    public void disableQueuedAttack()
    {
        queuedAttack = false;
    }

    public void attackEnd()
    {

        if (actionQueue.Count > 0 && actionQueue[0] == 1 && currentEnergy >= attackCost)
        {
            currentEnergy -= attackCost;
            regenEnergy = false;
            timeSinceEnergyUsed = 0;

            actionQueue.Remove(1);

            //queuedAttack = false;
            swordAnimator.SetBool("attack", true);
        }
        else
        {
            actionQueue.Clear();
            swordAnimator.SetBool("attack", false);
        }

    }

    protected new void Update()
    {
        base.Update();

        if (swordAnimator.GetBool("attack") & !queuedAttack)
            swordAnimator.SetBool("attack", false);


        AnimatorStateInfo weaponState = swordAnimator.GetCurrentAnimatorStateInfo(0);

        //Current Weapon State

        setup = weaponState.IsName("AttackSetup1") || weaponState.IsName("AttackSetup2");
        isAttacking = weaponState.IsTag("Damaging Attack");
        isBlocking = weaponState.IsName("Block");

        damageCollider.enabled = isAttacking;
        blockCollider.enabled = isBlocking;

        //Animation tracking

        attackAnimations = weaponState.IsTag("Attack") || weaponState.IsTag("Damaging Attack");
        walkAnimation = weaponState.IsName("Walk");
        idleAnimation = weaponState.IsName("Idle");

        swordAnimator.SetBool("attacking", attackAnimations);
        swordAnimator.SetBool("walking", walkAnimation);
        swordAnimator.SetBool("idling", idleAnimation);


        if (canAct)
        {
            if (Input.GetMouseButton(0) & !isAttacking)
            {
                swordAnimator.SetBool("blocking", true);
            }
            else
            {
                swordAnimator.SetBool("blocking", false);
            }

            if (Input.GetMouseButtonDown(1) & !isBlocking)
            {
                if ((isAttacking || setup) && actionQueue.Count < 1)
                {
                    actionQueue.Add(1);
                    queuedAttack = true;
                    swordAnimator.SetBool("attack", true);
                }
                else if (currentEnergy >= attackCost)
                {
                    swordAnimator.SetBool("attack", true);
                    currentEnergy -= attackCost;
                    regenEnergy = false;
                    timeSinceEnergyUsed = 0;
                }
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                //SPIN TO WIN BAY BEE
            }
        }
        else if (isBlocking)
        {
            swordAnimator.SetBool("blocking", false);
            isBlocking = false;
        }

        if (vel.magnitude > 0)
        {
            swordAnimator.SetBool("walk", true);
            swordAnimator.SetBool("idle", false);
        }
        else
        {
            swordAnimator.SetBool("idle", true);
            swordAnimator.SetBool("walk", false);
        }
    }
}

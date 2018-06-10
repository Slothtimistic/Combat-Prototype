using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainee : Player
{

    public static Trainee thisTrainee;

    public Animator swordAnimator;
    public BoxCollider damageCollider, blockCollider, whirlwindCollider;

    public bool breakAttack;

    [SerializeField]
    private float whirlwindCost;
    private bool queuedAttack, attackAnimations, blockAnimation, walkAnimation, idleAnimation, whirlwindAnimation;

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

    IEnumerator spinToWin()
    {
        whirlwindCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        whirlwindCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        whirlwindCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        whirlwindCollider.enabled = false;
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

        whirlwindAnimation = weaponState.IsName("Whirlwind");
        whirlwindCollider.enabled = whirlwindAnimation;

        //Animation tracking

        attackAnimations = weaponState.IsTag("Attack") || weaponState.IsTag("Damaging Attack");
        walkAnimation = weaponState.IsName("Walk");
        idleAnimation = weaponState.IsName("Idle");
        blockAnimation = weaponState.IsTag("Block");
        

        swordAnimator.SetBool("attacking", attackAnimations);
        swordAnimator.SetBool("walking", walkAnimation);
        swordAnimator.SetBool("idling", idleAnimation);
        swordAnimator.SetBool("blocking", blockAnimation);
        swordAnimator.SetBool("whirlwinding", whirlwindAnimation);

        if (canAct)
        {
            if (Input.GetMouseButton(0))
            {
                swordAnimator.SetBool("block", true);
            }
            else
            {
                swordAnimator.SetBool("block", false);
            }

            if (Input.GetMouseButtonDown(1) & !isBlocking &! swordAnimator.GetBool("attack"))
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

            if(Input.GetKeyDown(KeyCode.Q) &! whirlwindAnimation &! attackAnimations && currentEnergy >= whirlwindCost)
            {
                swordAnimator.SetBool("whirlwind", true);
                currentEnergy -= whirlwindCost;
                regenEnergy = false;
                timeSinceEnergyUsed = 0;
            }
        }
        else if (isBlocking)
        {
            swordAnimator.SetBool("block", false);
        }

        if (whirlwindAnimation & !overrideMovespeed)
        {
            overrideMovespeed = true;
            moveSpeedMultiplier = 0.5f;
        }
        else if (!whirlwindAnimation && overrideMovespeed)
            overrideMovespeed = false;

        if (vel.magnitude > 0)
        {
            swordAnimator.SetBool("walk", true);
            swordAnimator.SetBool("idle", false);

            if (Input.GetKey(KeyCode.LeftShift) && canAct)
                swordAnimator.SetFloat("moveSpeed", 2);
            else
                swordAnimator.SetFloat("moveSpeed", 1);
        }
        else
        {
            swordAnimator.SetBool("idle", true);
            swordAnimator.SetBool("walk", false);
            swordAnimator.SetFloat("moveSpeed", 1);
        }
    }
}

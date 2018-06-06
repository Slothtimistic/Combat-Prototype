using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : Player
{

    public static Defender thisDefender;

    public Animator shieldAnimator, swordAnimator;
    public BoxCollider swordCollider, shieldCollider;

    protected new void Awake()
    {
        base.Awake();
        thisDefender = this;
    }

    protected new void Start()
    {
        base.Start();
        swordCollider.enabled = false;
    }

    public void attackEnd(int anim)
    {

        if (actionQueue.Contains(1) && currentEnergy >= attackCost)
        {
            currentEnergy -= attackCost;
            regenEnergy = false;
            timeSinceEnergyUsed = 0;

            actionQueue.Remove(1);
            swordAnimator.SetBool("firstAttack", anim == 1);
            swordAnimator.SetBool("secondAttack", anim == 0);
        }
        else
        {
            actionQueue.Clear();
            swordAnimator.SetBool("firstAttack", false);
            swordAnimator.SetBool("secondAttack", false);
        }

    }

    protected new void Update()
    {
        base.Update();

        AnimatorStateInfo weaponState = swordAnimator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo shieldState = shieldAnimator.GetCurrentAnimatorStateInfo(0);

        setup = weaponState.IsName("FirstSetup") || weaponState.IsName("SecondSetup");
        isAttacking = weaponState.IsName("FirstSlice") || weaponState.IsName("SecondSlice");
        swordCollider.enabled = isAttacking;

        isBlocking = shieldState.IsName("HoldUp");
        shieldCollider.enabled = isBlocking;

        if (canAct)
        {
            if (Input.GetMouseButton(0) & !isAttacking)
            {
                shieldAnimator.SetBool("holding", true);
            }
            else
            {
                shieldAnimator.SetBool("holding", false);
            }

            if (Input.GetMouseButtonDown(1) & !isBlocking)
            {
                if ((isAttacking || setup) && actionQueue.Count < 2)
                {
                    actionQueue.Add(1);
                }
                else if (currentEnergy >= attackCost)
                {
                    swordAnimator.SetBool("firstAttack", true);
                    currentEnergy -= attackCost;
                    regenEnergy = false;
                    timeSinceEnergyUsed = 0;
                }
            }
        }
        else if (isBlocking)
        {
            shieldAnimator.SetBool("holding", false);
            isBlocking = false;
        }

    }
}

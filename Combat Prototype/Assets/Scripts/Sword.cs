using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    void startWhirlwind()
    {
        Trainee.thisTrainee.swordAnimator.SetBool("whirlwind", false);
    }

    public void attackEnd()
    {
        Trainee.thisTrainee.attackEnd();
    }

    void disableQueuedAttack()
    {
        Trainee.thisTrainee.disableQueuedAttack();
    }

    public void firstEnd()
    {
        Defender.thisDefender.attackEnd(0);
    }

    public void secondEnd()
    {
        Defender.thisDefender.attackEnd(1);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ewepSlow : MonoBehaviour
{

    public SlowEnemy enemyScript;

    public void Reassess()
    {
        enemyScript.Reasses();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (enemyScript.isAttacking())
        {
            if (other.gameObject.tag == "Player")
            {
                if (Player.thisPlayer.getBlocking())
                {
                    enemyScript.gotBlocked();
                    Player.thisPlayer.block(1);
                }
                else
                {
                    enemyScript.landedHit();
                    Player.thisPlayer.takeDamage(other.contacts[0].point, 1);
                }
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{

    public SpellEnemy enemyScript;

    void startMoving()
    {
        enemyScript.startMoving();
    }

    void castSpell()
    {
        enemyScript.castSpell();
    }

    void Recalculate()
    {
        enemyScript.Recalculate();
    }

}

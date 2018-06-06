using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrototypeEnemy : Enemy {


    //Walk around in an area
    //When the player moves too close, move towards the player
    //When enemy is close enough to the player, attack the player

    public GameObject playerObject, weaponObject;

    public float visionRadius;
    public bool hasWeapon;

    protected NavMeshAgent agent;
    protected Rigidbody enemyBody;

    protected void Start()
    {
        playerObject = Player.thisPlayer.gameObject;

        health = 1;
        agent = GetComponent<NavMeshAgent>();
    }

    public override void takeDamage(int d)
    {
        health -= d;

        if (health <= 0)
            die();
        
    }

    protected override void die()
    {
        agent.enabled = false;
        enemyBody = gameObject.AddComponent<Rigidbody>();

        Vector3 forceVect = (transform.position - playerObject.transform.position);
        forceVect.x *= 10;
        forceVect.z *= 10;
        forceVect.y = 0;

        enemyBody.AddForce(forceVect, ForceMode.Impulse);

        if(hasWeapon)
        {
            forceVect = (weaponObject.transform.position - transform.position) * 5;
            forceVect.y = 5;
            weaponObject.transform.parent = null;
            weaponObject.GetComponent<Animator>().enabled = false;
            Rigidbody weaponBody = weaponObject.GetComponent<Rigidbody>();
            weaponBody.useGravity = true;
            weaponBody.AddForce(forceVect, ForceMode.Impulse);
            weaponBody.angularVelocity = new Vector3(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "weapon" && health > 0)
        {
            takeDamage(1);
        }
    }
}

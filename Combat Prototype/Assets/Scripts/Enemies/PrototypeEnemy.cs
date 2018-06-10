using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrototypeEnemy : Enemy {


    //Walk around in an area
    //When the player moves too close, move towards the player
    //When enemy is close enough to the player, attack the player

    public GameObject playerObject, weaponObject;
    public BoxCollider weaponColliderToEnable;
    public GameObject healthBars, spriteMask;
    public SpriteRenderer healthBar, healthBackground;

    public float visionRadius, maxHealth;
    public bool hasWeapon;

    protected NavMeshAgent agent;
    protected Rigidbody enemyBody;

    protected float currentHealth;

    protected void Start()
    {
        playerObject = Player.thisPlayer.gameObject;
        agent = GetComponent<NavMeshAgent>();

        currentHealth = maxHealth;

    }

    protected void Update()
    {
        if ((currentHealth == maxHealth && healthBars.activeSelf) || (currentHealth == 0 && healthBars.activeSelf))
        {
            healthBars.SetActive(false);
        }
        else if (currentHealth < maxHealth &! healthBars.activeSelf && currentHealth != 0)
        {
            healthBars.SetActive(true);
        }

        if(currentHealth < maxHealth)
        {
            //healthBar.transform.localScale = new Vector3(currentHealth / maxHealth, 0.1f, 1);
            spriteMask.transform.localPosition = new Vector3(currentHealth / maxHealth, 1.3f, 0);
        }
    }

    public override void takeDamage(int d)
    {
        currentHealth -= d;

        if (currentHealth <= 0)
            die();
    }

    protected override void die()
    {
        healthBars.SetActive(false);

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
            weaponObject.tag = "Untagged";
            weaponObject.transform.parent = null;
            weaponColliderToEnable.enabled = true;
            weaponObject.GetComponent<Animator>().enabled = false;
            Rigidbody weaponBody = weaponObject.GetComponent<Rigidbody>();
            weaponBody.useGravity = true;
            weaponBody.AddForce(forceVect, ForceMode.Impulse);
            weaponBody.angularVelocity = new Vector3(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "weapon" && currentHealth > 0)
        {
            takeDamage(1);
        }
    }
}

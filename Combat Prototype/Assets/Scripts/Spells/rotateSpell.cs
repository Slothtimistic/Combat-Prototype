using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateSpell : MonoBehaviour
{

    public GameObject forwardSpell, backwardSpel;
    public Rigidbody spellBody;
    public ParticleSystem spellEffect;

    public float rotationSpeed, moveSpeed, damage;

    Vector3 toPos = Vector3.zero;

    public void placeObject(Vector3 to)
    {
        transform.LookAt(toPos);
        toPos = to;
    }

    void endSpell()
    {
        spellEffect.Stop();
        spellEffect.transform.parent = null;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().takeDamage(transform.position, damage);
            endSpell();
        }
        else if (other.gameObject.tag == "Player Shield")
        {
            endSpell();
        }
        else if (other.gameObject.layer == 10)
        {
            endSpell();
        }

    }

    void FixedUpdate()
    {
        transform.LookAt(Player.thisPlayer.transform);
        forwardSpell.transform.Rotate(transform.InverseTransformDirection(Vector3.forward * rotationSpeed));
        backwardSpel.transform.Rotate(transform.InverseTransformDirection(-Vector3.forward * rotationSpeed));

        //spellBody.velocity = transform.InverseTransformDirection(-Vector3.forward * moveSpeed);
        spellBody.velocity = (transform.TransformDirection(Vector3.forward) * moveSpeed);
    }

}
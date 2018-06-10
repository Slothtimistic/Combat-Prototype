using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerState
{
    public float level,
        currentExp,
        expNeeded,
        currentHealth,
        maxHealth,
        currentEnergy,
        maxEnergy;
}

public class Player : MonoBehaviour
{
    public static Player thisPlayer;

    public CharacterController playerController;
    public GameObject camObject;
    public CapsuleCollider playerCollider;

    public LayerMask jumpMask;
    public Renderer playerRenderer;
    public Material playerMaterial, damagedMaterial;

    public static PlayerState currentState;

    protected List<int> actionQueue = new List<int>();

    protected Dictionary<string, int> experienceDictionary = new Dictionary<string, int>()
    {
        { "level_one", 30 },
        { "level_two", 50 }
    };

    protected Vector3 inputs, vel, damagedVelocity, deadVelocity;
    protected float v, distToGround, moveSpeedMultiplier, turnTime, timeSinceEnergyUsed;
    protected bool isBlocking, setup, isAttacking, canAct = true, regenEnergy = true, isDamaged, isDead, hasJumped, overrideMovespeed;

    protected float level = 1, currentExp = 0, currentHealth = 5, maxHealth = 5, currentEnergy = 200, maxEnergy = 200, attackCost = 50;

    public delegate void UpdateUI(string e, PlayerState p = new PlayerState());

    public static event UpdateUI OnUIChange;

    protected void Awake()
    {
        thisPlayer = this;
        camObject = GameObject.Find("Main Camera");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected void Start()
    {
        distToGround = playerCollider.bounds.extents.y;
        canAct = true;
    }

    void updateState()
    {
        currentState.level = level;
    }

    protected void Update()
    {
        inputs.x = Input.GetAxis("Vertical");
        inputs.z = Input.GetAxis("Horizontal");
        inputs.y = Input.GetAxis("Jump");

        if (Input.GetKeyDown(KeyCode.C))
            OnUIChange("EVENT_OPEN_INVENTORY");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && canAct;

        if (!overrideMovespeed)
        {
            if (isBlocking)
            {
                moveSpeedMultiplier = 0.5f;
                turnTime = 0.3f;
            }
            else if (isSprinting)
            {
                moveSpeedMultiplier = 1.5f;
                turnTime = 0.1f;
            }
            else
            {
                moveSpeedMultiplier = 1;
                turnTime = 0.1f;
            }
        }

        if (!isDead)
            transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, camObject.transform.eulerAngles.y, ref v, turnTime), 0);
        else if (!isDead)
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        bool checkGround = false;

        if (!isDead)
            checkGround = isGrounded();

        vel = transform.forward * inputs.x * 5;

        if (checkGround & !isDead)
        {
            Vector3 normal = groundNormal();
            transform.eulerAngles = new Vector3(normal.x, transform.eulerAngles.y, normal.z);
        }

        vel += transform.right * inputs.z * 4.5f;

        if (inputs.y > 0 && checkGround &! hasJumped)
            vel.y = 15f;
        else if (!checkGround & !isDead)
            vel.y = playerController.velocity.y - 0.75f;

        if (inputs.y > 0)
            hasJumped = true;
        else
            hasJumped = false;

        vel.x *= moveSpeedMultiplier;
        vel.z *= moveSpeedMultiplier;

        if (!isDead)
        {
            playerController.Move(vel * Time.deltaTime);
        }

        if (isBlocking)
        {
            regenEnergy = false;
            timeSinceEnergyUsed = 0;

            if (currentEnergy > 0)
                currentEnergy = Mathf.Clamp(currentEnergy - 1, 0, maxEnergy);

            if (currentEnergy == 0)
            {
                canAct = false;
            }
        }
        else if (isSprinting)
        {
            if (currentEnergy > 0)
                currentEnergy = Mathf.Clamp(currentEnergy - 1.5f, 0, maxEnergy);

            if (currentEnergy == 0)
            {
                canAct = false;
            }
        }
        else
        {
            if (currentEnergy < maxEnergy && regenEnergy)
                currentEnergy = Mathf.Clamp(currentEnergy + 2, 0, maxEnergy);

            if (canAct == false && currentEnergy == maxEnergy)
                canAct = true;
        }

        if (!regenEnergy)
        {
            if (timeSinceEnergyUsed < 0.5f)
                timeSinceEnergyUsed += Time.deltaTime;
            else
                regenEnergy = true;
        }

        updateState();

    }

    public void takeDamage(Vector3 contact, float damage)
    {
        isDamaged = true;
        currentHealth -= damage;

        if (currentHealth <= 0 & !isDead)
        {
            Vector3 dir = (transform.position - contact).normalized;
            StartCoroutine(youDied(dir));
        }
        else
            StartCoroutine(damaged());
    }

    IEnumerator damaged()
    {
        playerRenderer.material = damagedMaterial;
        yield return new WaitForSeconds(0.25f);
        isDamaged = false;
        playerRenderer.material = playerMaterial;
    }

    IEnumerator youDied(Vector3 dir)
    {
        isDead = true;
        Destroy(playerController);
        Rigidbody playerBody = gameObject.AddComponent<Rigidbody>();
        playerCollider.enabled = true;
        playerBody.interpolation = RigidbodyInterpolation.Interpolate;
        playerBody.AddForce(new Vector3(dir.x * 20, 2.5f, dir.z * 20), ForceMode.Impulse);
        playerBody.angularVelocity = dir;

        yield return new WaitForSeconds(1f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void FixedUpdate()
    {
        UIManager.UpdatePlayerUI(currentEnergy, maxEnergy, currentHealth, maxHealth);
    }

    public bool getBlocking()
    {
        return isBlocking;
    }

    public void block(int damage)
    {
        currentEnergy = Mathf.Clamp(currentEnergy - (50 * damage), 0, maxEnergy);

        if (currentEnergy == 0)
            canAct = false;
    }

    private bool isGrounded()
    {
        if (playerController.isGrounded)
            return true;

        Vector3 bottom = playerController.transform.position - new Vector3(0, playerController.height / 2, 0);

        RaycastHit hit;
        if (Physics.Raycast(bottom, new Vector3(0, -1, 0), out hit, 0.3f)) // was 0.25f
        {
            playerController.Move(new Vector3(0, -hit.distance, 0));
            return true;
        }

        return false;
    }

    private Vector3 groundNormal()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f, jumpMask);
        return hit.normal;
    }
}
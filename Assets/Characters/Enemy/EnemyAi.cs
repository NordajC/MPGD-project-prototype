using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;

public class EnemyAi : MonoBehaviour, ICombat
{
    //delete this if not working
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [Header("References")]
    public NavMeshAgent agent;
    private Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Transform moveToPosition;
    public float moveToFactor = 1;
    public float minMoveDistance = 1.4f;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Sight perception")]
    public float sightRange;
    public bool playerInSightRange;

    [Header("Attacking")]
    public float attackRange;
    public bool playerInAttackRange;
    public int attackDamage = 10;
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth;
    public StatsBar HPBar;
    private Transform cameraRotation;

    [Header("Animations")]
    [HideInInspector] public Animator animator;

    [Header("Effects")]
    public GameObject hitParticle;

    //delete difficulty settings if not working
    [System.Serializable]
    public class DifficultySettings
    {
        public float maxHealth;
        public int attackDamage;
        public float sightRange;
        public float attackSpeed;
        // Add other relevant parameters here
    }

    [Header("Difficulty Settings")]
    public Difficulty currentDifficulty;
    public DifficultySettings easySettings;
    public DifficultySettings mediumSettings;
    public DifficultySettings hardSettings;

    [Header("Idle Behaviour")]
    public float idleTime = 2f; // Time for which the enemy idles
    private bool isIdling = false;
    private float idleTimer = 0;


    //delete this start function if not working
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        cameraRotation = Camera.main.transform;

        ApplyDifficulty(currentDifficulty); // Apply difficulty settings
        currentHealth = maxHealth; // Initialize current health based on difficulty
        HPBar.maxValue = maxHealth;
    }


    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        // HPBar.SetActive(false);
        cameraRotation = Camera.main.transform;

        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        HPBar.maxValue = maxHealth;
    }

    private void Update()
    {
        // Check if enemy in range of player for sight and attacking.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Choose wether to patrol, chase or attack.
        if (!playerInSightRange && !playerInAttackRange)
            Patrolling();
        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        if (playerInSightRange && playerInAttackRange)
            AttackPlayer();

        // Chase player if in sight.
        if (playerInSightRange)
        {
            ChasePlayer();

            HPBar.gameObject.SetActive(true); // Only show enemys HP bar if locked on to player.
            HPBar.transform.LookAt(cameraRotation);
            HPBar.transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            Patrolling();
            HPBar.gameObject.SetActive(false);
        }

        float speed = agent.velocity.magnitude;
        animator.SetFloat("magnitude", speed);
    }

    private void Patrolling()
    {
        if (isIdling)
        {
            animator.SetBool("IsIdling", true);
            // Increase the idle timer
            idleTimer += Time.deltaTime;

            // Check if idle time is over
            if (idleTimer >= idleTime)
            {
                isIdling = false;
                idleTimer = 0;
            }
        }
        else
        {
            animator.SetBool("IsIdling", false);
            // Select patrol state and destination
            if (!walkPointSet)
                SearchWalkPoint();

            if (walkPointSet)
                agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            // Check if the enemy has reached near the walk point
            if (distanceToWalkPoint.magnitude < 1f)
            {
                walkPointSet = false;
                isIdling = true; // Start idling
            }
        }
    }

    private void SearchWalkPoint()
    {
        // Random floats set for random move to destination.
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Only move to the destination if it is on ground.
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (Vector3.Distance(player.position, transform.position) > 2)
            agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Stop the enemy from moving while attacking

        agent.SetDestination(transform.position);
        agent.isStopped = true;

        // Ensure the enemy is facing the player when attacking
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);

        if (!alreadyAttacked)
        {
            //Deal damage
            /*            PlayerAttributes playerHealth = player.GetComponent<PlayerAttributes>();
                        if (playerHealth != null)
                            playerHealth.adjustHealth(attackDamage, false);*/

            // If there's an animator, you'd play your attack animation here.
            animator.SetTrigger("Attack");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        agent.isStopped = false; // Allow the enemy to move again

        // After attack, decide whether to chase the player or go back to patrolling
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (!playerInSightRange)
        {
            walkPointSet = false;
            Patrolling();
        }
    }

    public void updateHealth(float amount)
    {
        currentHealth -= amount;
        HPBar.updateStatBarValue(currentHealth);
        
        if(currentHealth <= 0)
        {
            onDeath();
        }
    }

    public virtual void onDeath()
    {
        // Marked as virtual so different enemies can have different death effects.
        Destroy(transform.Find("EnemyUI").gameObject);
    }

    public void onHitReaction(bool heavyAttack, Vector3 hitLocation, HitDirection hitDirection, HitHeight hitHeight, GameObject receivedFrom, float damage)
    {
        // Play hit reaction based on direction and height.
        if (heavyAttack)
        {
            // animator.Play("Heavy" + hitDirection.ToString(), 0, 0);
        }
        else
        {
            if (hitDirection == HitDirection.Front)
            {
                // Only front reaction animations have varying ones for lower and upper.
                // animator.Play("Light" + hitDirection.ToString() + hitHeight.ToString(), 0, 0);
            }
            else
            {
                // animator.Play("Light" + hitDirection.ToString(), 0, 0);
            }
        }

        // Instantiate(hitParticle, hitLocation, Quaternion.identity);

        // Add knockback to enemy by adding force.
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = transform.position - receivedFrom.transform.position;
        direction.Normalize();
        direction.y = 0; // Don't want to push enemy vertically.
        rb.AddForce(direction * 100);

        updateHealth(damage);
    }

    public void DealDamageToPlayer()
    {
        if (playerInAttackRange)
        {
            PlayerAttributes playerHealth = player.GetComponent<PlayerAttributes>();
            if (playerHealth != null)
                playerHealth.adjustHealth(attackDamage, false);
        }
    }

    //delete this if not working
    private void ApplyDifficulty(Difficulty difficulty)
    {
        DifficultySettings settings;
        switch (difficulty)
        {
            case Difficulty.Easy:
                settings = easySettings;
                break;
            case Difficulty.Medium:
                settings = mediumSettings;
                break;
            case Difficulty.Hard:
                settings = hardSettings;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        maxHealth = settings.maxHealth;
        attackDamage = settings.attackDamage;
        sightRange = settings.sightRange;
        timeBetweenAttacks = settings.attackSpeed;

        // Apply other settings as necessary
    }
}


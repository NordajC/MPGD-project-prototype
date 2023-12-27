using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
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
    
    [Header("UI")]    
    public GameObject HPBar;
    private Transform cameraRotation;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        HPBar.SetActive(false);
        cameraRotation = Camera.main.transform;
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
            
            HPBar.SetActive(true); // Only show enemys HP bar if locked on to player.
            HPBar.transform.LookAt(cameraRotation);
            HPBar.transform.Rotate(0f, 180f, 0f);
        } else {
            Patrolling();

            HPBar.SetActive(false);
        }
    }

    private void Patrolling()
    {
        // Select patrol state and destination.
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Random floats set for random move to destination.
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Only move to the destination if it is on ground.
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if(Vector3.Distance(player.position, transform.position) > 2)
            agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Stop the enemy from moving while attacking
        agent.SetDestination(transform.position);

        // Ensure the enemy is facing the player when attacking
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            PlayerAttributes playerHealth = player.GetComponent<PlayerAttributes>();
            if (playerHealth != null)
                playerHealth.adjustHealth(attackDamage, false);

            // If there's an animator, you'd play your attack animation here.
            // animator.SetTrigger("Attack");

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
}
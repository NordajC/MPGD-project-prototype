using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAi : MonoBehaviour
{

    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;


    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float sightRange;
    public bool playerInSightRange;

    public float attackRange;
    public bool playerInAttackRange;
    public int attackDamage = 10;

    public float timeBetweenAttacks;
    bool alreadyAttacked;


    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        Debug.Log($"Player in Sight Range: {playerInSightRange}, Player in Attack Range: {playerInAttackRange}");

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patrolling()
    {
        Debug.Log("Patrolling");
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }


    private void ChasePlayer()
    {
        Debug.Log("Chasing Player");
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        Debug.Log("Attacking Player");
        // Stop the enemy from moving while attacking
        agent.SetDestination(transform.position);

        // Ensure the enemy is facing the player when attacking
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Your attack code here.
            PlayerAttributes playerHealth = player.GetComponent<PlayerAttributes>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            // If there's an animator, you'd play your attack animation here.
            // animator.SetTrigger("Attack");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        Debug.Log("Resetting Attack");
        alreadyAttacked = false;
        agent.isStopped = false; // Allow the enemy to move again

        // After attack, decide whether to chase the player or go back to patrolling
        if (playerInSightRange && !playerInAttackRange)
        {
            Debug.Log("Reset: Chasing Player");
            ChasePlayer();
        }
        else if (!playerInSightRange)
        {
            Debug.Log("Reset: Going back to Patrolling");
            walkPointSet = false;
            Patrolling();
        }
    }

}
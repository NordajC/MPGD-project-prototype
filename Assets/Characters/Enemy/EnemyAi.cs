using System.Collections;
using System.Collections.Generic;
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

    public GameObject HPBar;
    private Transform cameraRotation;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        HPBar.SetActive(false);
        cameraRotation = Camera.main.transform;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);


        if (playerInSightRange)
        {   
            ChasePlayer();
            HPBar.SetActive(true);
            HPBar.transform.LookAt(cameraRotation);
            HPBar.transform.Rotate(0f, 180f, 0f);
        } else {
            Patrolling();
            HPBar.SetActive(false);
        }
    }

    private void Patrolling()
    {
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

        if (Physics.Raycast(walkPoint,-transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }


    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }


}

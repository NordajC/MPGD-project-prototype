using System.Collections;
using UnityEngine;

public class BossAI : EnemyAi
{

    private bool useSwitchAttack = false; // Flag to determine the attack type

    bool isSwitchAttack;
    void Update()
    {
        base.Update();

        if (playerInAttackRange)
        {
            if (!alreadyAttacked)
            {
                CycleRegularAttacks();
            }
        }
    }

    private void CycleRegularAttacks()
    {
        if (useSwitchAttack)
        {
            animator.SetTrigger("SwitchAttack");
        }
        AttackPlayer();
        useSwitchAttack = !useSwitchAttack; // Toggle the attack type for the next cycle
    }

    public override void AttackPlayer()
    {
        // Stop the enemy from moving while attacking
        agent.SetDestination(transform.position);
        agent.isStopped = true;

        // Ensure the enemy is facing the player when attacking
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        direction.y = 0f; // Keep the enemy level on the ground
        transform.rotation = Quaternion.LookRotation(direction);

        if (!alreadyAttacked)
        {
            if (isSwitchAttack)
            {
                // Logic for switch attack
                animator.SetTrigger("SwitchAttack"); // Replace with your actual switch attack animation trigger
                // Additional logic for switch attack (e.g., different damage or effects)
                Debug.Log("Performing Switch Attack");
                isSwitchAttack = false;
            }
            else if (!isSwitchAttack)
            {
                // Logic for regular attack
                animator.SetTrigger("Attack"); // This trigger should match the regular attack animation
                // Additional logic for regular attack
                Debug.Log("Performing Regular Attack");
                isSwitchAttack = true;
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

}

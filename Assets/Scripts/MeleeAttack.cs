using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public int attackDamage = 10;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    public float attackCooldown = 0.6f; // cooldown in seconds

    private float nextAttackTime = 0f; // Time when the next attack is allowed

    private Animator animator;

    // Use this to find the Animator component in the Shoulder GameObject when the game starts
    void Start()
    {
        // This will find the PlayerObject child, then find the Shoulder child of PlayerObject,
        // and finally get the Animator component on Shoulder.
        Transform playerObject = transform.Find("PlayerObj");
        if (playerObject != null)
        {
            Transform shoulderTransform = playerObject.Find("shoulder");
            if (shoulderTransform != null)
            {
                animator = shoulderTransform.GetComponent<Animator>();
            }
            else
            {
                Debug.LogError("Shoulder object not found!");
            }
        }
        else
        {
            Debug.LogError("PlayerObject not found!");
        }
    }

    private void Update()
    {
        if (Time.time >= nextAttackTime) // Check if enough time has passed for another attack
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
                if (animator != null)
                {
                    animator.SetTrigger("SwordSwing");
                }
                nextAttackTime = Time.time + 1f / attackCooldown; // Set the next attack time
            }
        }
    }

    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            // Make sure to check if enemy has the PlayerAttributes component before calling TakeDamage
            PlayerAttributes enemyAttributes = enemy.GetComponent<PlayerAttributes>();
            if (enemyAttributes != null)
            {
                enemyAttributes.TakeDamage(attackDamage);
            }
        }
    }
}

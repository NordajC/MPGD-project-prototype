using System.Collections;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class BossAI : EnemyAi
{

    [Header("Boss")]
    public int weakPoints = 4;
    private int weakPointsRemaining;
    private float nextDamageThreshold;
    private float damageIncrement;
    private bool stunned = false;

    private IEnumerator timerCoroutine;

    [Header("Effects")]
    public GameObject weakPointBreakParticle;
    public GameObject shockwaveParticle;
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip[] regenerateSounds;

    [Header("BossUI")]
    public Animator uiAnimator;
    public StatsBar secondaryHPBar;
    public TextMeshProUGUI stunnedText;

    private bool useSwitchAttack = false; // Flag to determine the attack type

    bool isSwitchAttack;
    
    void Start()
    {
        base.Start();
        weakPointsRemaining = weakPoints;
        damageIncrement = maxHealth/weakPoints;
        nextDamageThreshold = maxHealth - damageIncrement;
    }

    void Update()
    {
        // Check if enemy in range of player for sight and attacking.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!stunned)
        {
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
            }
            else
            {
                Patrolling();
            }
        } else {
            GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
            agent.velocity = Vector3.zero;
        }

        float speed = agent.velocity.magnitude;
        animator.SetFloat("magnitude", speed);

        if (playerInAttackRange && !stunned)
        {
            if (!alreadyAttacked)
            {
                CycleRegularAttacks();
            }
        }
        
        // string debugString = "";
        // debugString += "currentHealth: " + currentHealth;
        // debugString += "\n" + "weakPointsRemaining: " + weakPointsRemaining;
        // debugString += "\n" + "nextDamageThreshold: " + nextDamageThreshold;
        // transform.Find("Canvas").Find("DebugInventoryText").GetComponent<TextMeshProUGUI>().text = debugString;
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
                isSwitchAttack = false;
            }
            else if (!isSwitchAttack)
            {
                // Logic for regular attack
                animator.SetTrigger("Attack"); // This trigger should match the regular attack animation
                // Additional logic for regular attack
                isSwitchAttack = true;
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public override void onHitReaction(bool heavyAttack, Vector3 hitLocation, HitDirection hitDirection, HitHeight hitHeight, GameObject receivedFrom, float damage)
    {
        if(!stunned)
        { 
            Instantiate(hitParticle, hitLocation, Quaternion.identity);

            // Add knockback to enemy by adding force.
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 direction = transform.position - receivedFrom.transform.position;
            direction.Normalize();
            direction.y = 0; // Don't want to push enemy vertically.
            rb.AddForce(direction * 10);

            updateHealth(damage);
        }
    }
    
    public override void updateHealth(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, nextDamageThreshold, maxHealth);
        HPBar.updateStatBarValue(currentHealth);
        secondaryHPBar.GetComponent<Slider>().value = HPBar.GetComponent<Slider>().value;

        if(currentHealth <= nextDamageThreshold)
        {
            stunBoss(true);
            playerKnockback();
        } else if(currentHealth <= 0) {
            onDeath();
        }
    }

    public override void onDeath()
    {
        Destroy(uiAnimator.transform.gameObject);

        animator.CrossFade("Death", 0.1f);
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(gameObject, 10f); // Destroy the game object after a short delay.
        Destroy(this);
    }

    public void heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, nextDamageThreshold, maxHealth);
        HPBar.updateStatBarValue(currentHealth);
        secondaryHPBar.GetComponent<Slider>().value = HPBar.GetComponent<Slider>().value;
    }
    
    public void stunBoss(bool stun)
    {
        if(stun)
        {
            animator.CrossFade("Stunned", 0.1f);
            stunned = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            uiAnimator.SetBool("stunned", true); 
            stunnedText.text = "The boss is stunned! Attack its weak point!";
            timerCoroutine = startTimer(10);
            StartCoroutine(timerCoroutine);
        } else {
            stunned = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            uiAnimator.SetBool("stunned", false);
            stunnedText.text = "";
            GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
            StopCoroutine(timerCoroutine);
        }
    }

    public void unstunBoss()
    {
        stunBoss(false);
    }
    
    public void onWeakPointHit()
    {
        weakPointsRemaining--;
        
        currentHealth = nextDamageThreshold;
        nextDamageThreshold = maxHealth - (damageIncrement * ((weakPoints - weakPointsRemaining) + 1));
        animator.CrossFade("Damaged", 0.1f);

        if(weakPointsRemaining == 0)
        {
            onDeath();
        }


    }
    
    public void destroyWeakPoint(GameObject weakPointObject)
    {
        Debug.Log("Destroy weak");
        if(stunned)
        { 
            Instantiate(weakPointBreakParticle, weakPointObject.transform.position, Quaternion.identity);
            Destroy(weakPointObject);
            onWeakPointHit();

            uiAnimator.SetBool("healed", true); 
            stunnedText.text = "Weak point destroyed! " + weakPointsRemaining + " Remaining!";
        }
    }

    public void tryHeal()
    {
        Debug.Log("call");
        if(stunned)
        {
            animator.CrossFade("Regenerate", 0.1f);
            heal(damageIncrement);
            uiAnimator.SetBool("healed", true); 
            stunnedText.text = "The boss has regenerated!";
        }
    }
    
    IEnumerator startTimer(int duration) {
        int elapsed = duration;
        
        while (elapsed > 0) {
            // Debug.Log(elapsed);
            yield return new WaitForSeconds (10);
            elapsed--;
        }
        
        tryHeal();
    }

    public void playerKnockback()
    {
        Instantiate(shockwaveParticle, transform.position, Quaternion.identity);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        if(distance <= 3f)
        {
            Vector3 targetPos = GameObject.Find("PlayerMain").transform.position - (player.forward * 2);
            player.GetComponent<MovementStateManager>().enabled = false;
            StartCoroutine(player.GetComponent<PlayerCombat>().SmoothMoveTo(GameObject.Find("PlayerMain"), targetPos, 0.5f));
            player.GetComponent<Animator>().CrossFade(Quaternion.LookRotation(player.forward).y > 0 ? "Knockback_Front" : "Knockback_Back", 0.1f);
        }
    }

    public void playRoarSound()
    {
        Manager.playSound(ref audioSource, damageSound, 0.2f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

// Enum used to determine where hit from.
public enum HitBone
{
    PT_RightHand,
    PT_LeftHand,
    PT_RightFoot,
    PT_LeftFoot,
    WeaponMelee,
    WeaponRanged
}

// Enum used to determine hit direction.
public enum HitDirection
{
    Front,
    Right,
    Back,
    Left
}

// Enum used to determine where hit from.
public enum HitHeight
{
    Lower,
    Upper
}

public class PlayerCombat : MonoBehaviour
{
    [Header("Defaults")]
    public GameObject playerMain;
    [HideInInspector] public Animator animator;
    public LayerMask attackableLayers;
    
    [Header("Combat system")]
    public MovementStateManager movementStateManager;
    private int attackCounter = 0;
    public int maxCombo = 4;
    public bool canAttack = true;
    public string animationTriggerName;
    public string animationIntegerName;
    public float unarmedMinDamage = 8;
    public float unarmedMaxDamage = 10;
    public bool isBlocking = false;

    [Header("Enemy detection")]
    public bool doMinDistanceCheck;
    public float moveToEnemySpeed = 0.2f;
    private GameObject closestEnemy;
    public BoxCollider hitDetection;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAttack()
    {
        PlayerInventory playerInventory = GetComponent<PlayerInventory>();
        WeaponryItem weaponryItem = playerInventory.playerWeaponPrimary.itemTemplate as WeaponryItem;

        MovementStateManager movementStateManager = GetComponent<MovementStateManager>();

        if(playerInventory.playerWeaponPrimary.itemTemplate.ItemId == 0 || weaponryItem.weaponType == WeaponType.Melee)
        {
            closestEnemy = GetNearestEnemy(2);
            
            if(GetComponent<PlayerInventory>().currentScreen == CurrentScreen.None)
            {
                if(canAttack)
                {
                    if(weaponryItem == null)
                    {
                        maxCombo = 4;
                    } else {
                        maxCombo = weaponryItem.maxCombo;
                    }
                    
                    startAttack();
                }
            }
        } else if (playerInventory.playerWeaponPrimary.itemTemplate.ItemId == 0 || weaponryItem.weaponType == WeaponType.Ranged && movementStateManager.rotationMode == RotationMode.Aiming) {
            RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
            rangedWeapon.tryShoot();
        }
    }
     
    public void moveTowardsEnemy(float moveToFactorMultiplier)
    {
        // Parameter is extra multiplier based on animation.
        if(closestEnemy != null)
        {
            Transform moveToPosition = closestEnemy.GetComponent<EnemyAi>().moveToPosition; // Gets the move to position based on the enemy.
            Vector3 direction = moveToPosition.transform.position - playerMain.transform.position; // Gets unit direction between player and enemy.
            direction.Normalize(); // Normalized as only direction needed, not magnitude.
            float moveToFactor = closestEnemy.GetComponent<EnemyAi>().moveToFactor; // Different enemies may be different sizes so the move to closeness needs to be different.
            Vector3 targetPosition = moveToPosition.transform.position + (direction * moveToFactor * moveToFactorMultiplier * -1); // Find final move to position.
            
            // Only if player is above a certain distance from the enemy should they move to them.
            if((Vector3.Distance(playerMain.transform.position, closestEnemy.transform.position) > closestEnemy.GetComponent<EnemyAi>().minMoveDistance && doMinDistanceCheck) || !doMinDistanceCheck)
            {
                StartCoroutine(SmoothMoveTo(playerMain, targetPosition, moveToEnemySpeed));
            }
            
            direction.y = 0f; // Zero out y before calling rotate coroutine.

            Quaternion targetRotation = Quaternion.LookRotation(direction); // Get the rotation for player to face the enemy.
            StartCoroutine(SmoothRotateTo(gameObject, targetRotation, moveToEnemySpeed));
        }

        // Scale hit detection box based on animation.
        GameObject.Find("HitDetection").GetComponent<BoxCollider>().center = new Vector3(0f, 0f, moveToFactorMultiplier * 0.1f);
        GameObject.Find("HitDetection").GetComponent<BoxCollider>().size = new Vector3(1f, 1.5f, 0.6f + (moveToFactorMultiplier * 0.4f));
    }
    public GameObject GetNearestEnemy(float traceRadius)
    {
        List<GameObject> inRange = new List<GameObject>();
        // Raycast to detect for enemies.
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, traceRadius, transform.forward, 0, attackableLayers, QueryTriggerInteraction.UseGlobal);
    
        GameObject targetObject;
        float compareDistance;

        inRange.Clear();

        foreach (RaycastHit hit in raycastHits)
        {
            inRange.Add(hit.transform.gameObject); // Adds all enemies in range to the list so the closest can be found.
        }

        if(inRange.Count > 0)
        {
            targetObject = inRange[0]; // Initial closest is the first enemy found.
            compareDistance = Vector3.Distance(transform.position, targetObject.transform.position); // A float distance variable used to compare.

            for (int i = 0; i < inRange.Count; i++)
            {
                // If the distance between the current enemy checked is lower than the current enemy set as the closest, then update it.
                if(Vector3.Distance(transform.position, inRange[i].transform.position) < compareDistance)
                    targetObject = inRange[i];
            }
            
            return targetObject;
        } else {
            return null; // If no enemies are detected, return null.
        }
    }

    // Functions to smoothly move and rotate player to a target destination. Linear interpolation used to make it smooth.
    public IEnumerator SmoothMoveTo(GameObject targetObject, Vector3 targetLocation, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPos = targetObject.transform.position;
        
        while (elapsedTime < duration)
        {
            targetObject.transform.position = Vector3.Lerp(startingPos, targetLocation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        targetObject.transform.position = targetLocation;
    }

    public IEnumerator SmoothRotateTo(GameObject targetObject, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        Quaternion startingRot = targetObject.transform.rotation;
        
        while (elapsedTime < duration)
        {
            targetObject.transform.rotation = Quaternion.Lerp(startingRot, targetRotation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetObject.transform.rotation = targetRotation;
    }
    
    public void startAttack()
    {
        // Play attack animation.
        // animator.SetTrigger(animationTriggerName);
        // animator.SetInteger(animationIntegerName, attackCounter);
        animator.CrossFade("Attack" + (attackCounter + 1).ToString(), 0.1f);
        
        canAttack = false; // Attack disabled to prevent spam.
        movementStateManager.enabled = false; // Disable movement while attacking.
    }

    public void attackEvent(string eventData)
    {
        // Since animation events do not support multiple parameters, a single string is used and split.
        // "Bone Name, Heavy attack, Hit height".
        string[] attackEventData = eventData.Split(',');
        
        HitBone boneName = (HitBone)System.Enum.Parse(typeof(HitBone), attackEventData[0]); // String to enum.
        
        bool heavyAttack = attackEventData[1].ToLower() == "true";

        // Get hit location as vector by getting location of either bone, or weapon.
        Vector3 hitLocation = new Vector3();
        switch (boneName)
        {
            default:
                hitLocation = GameObject.Find(boneName.ToString()).transform.position;
                break;
            case HitBone.WeaponMelee:
                // Get location of weapon hit point
                break;
            case HitBone.WeaponRanged:
                // Get location of weapon hit point
                break;
        }

        HitHeight hitHeight = (HitHeight)System.Enum.Parse(typeof(HitHeight), attackEventData[2]);
            
        onDealDamage(boneName, heavyAttack, hitLocation, hitHeight);
    }
    
    public void resetAttack()
    {
        // Called when next combo attack should be triggerable.
        canAttack = true;
    }

    public void resetCombo()
    {
        // Called when the combo should reset.
        attackCounter = 0;
        resetAttack();
        animator.SetTrigger("resetState");
        movementStateManager.enabled = true;
    }

    public HitDirection getHitDirection(GameObject target)
    {
        // Calculate direction.
        HitDirection hitDirection;
        float forwardDir = Vector3.Dot((transform.position - target.transform.position).normalized, target.transform.forward);
        float rightDir = Vector3.Dot((transform.position - target.transform.position).normalized, target.transform.right);
        float angle = Mathf.Atan2(rightDir, forwardDir) * Mathf.Rad2Deg; // Find angle using forward and right components.
        if(angle < 45 && angle > -45)
        {
            hitDirection = HitDirection.Front;
        } else if(angle < -45 && angle > -135) {
            hitDirection = HitDirection.Right;
        } else if(angle < 135 && angle > 45) {
            hitDirection = HitDirection.Left;
        } else {
            hitDirection = HitDirection.Back;
        }

        return hitDirection;
    }

    public void onDealDamage(HitBone hitBone, bool heavyAttack, Vector3 hitLocation, HitHeight hitHeight)
    {
        // Get actors in collider range.
        Vector3 origin = hitDetection.transform.position + hitDetection.center;
        Vector3 extent = hitDetection.size;
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        Collider[] inRange = Physics.OverlapBox(origin, extent, rotation, attackableLayers); // Overlap box using same parameters as box collider.

        // Get enemy AI script of each game object in range. If it is not null, call hit reaction function.
        foreach(Collider collider in inRange)
        {
            EnemyAi enemyAi = collider.gameObject.GetComponent<EnemyAi>();

            if(enemyAi != null)
            {
                float damageAmount = getDamageAmount(hitBone, heavyAttack);
                enemyAi.onHitReaction(heavyAttack, hitLocation, getHitDirection(collider.gameObject), hitHeight, gameObject, damageAmount);
            }
        }
        
        attackCounter++; // After hit, increment attack counter for next combo.

        if(attackCounter >= maxCombo)
            attackCounter = 0; // Reset attack counter if all combo animations have been cycled through.
    }

    public float getDamageAmount(HitBone hitBone, bool heavyAttack)
    {
        PlayerInventory playerInventory = GetComponent<PlayerInventory>();
        float dealDamage = 0f;
        float multiplier = 1f; // Multipliers for extra damage.
        if(playerInventory.playerWeaponPrimary.itemTemplate.ItemId == 0)
        {
            dealDamage = UnityEngine.Random.Range(unarmedMinDamage, unarmedMaxDamage);
            
            if((hitBone == HitBone.PT_RightHand || hitBone == HitBone.PT_LeftHand) && playerInventory.playerGauntlets.itemTemplate.ItemId != 0 && playerInventory.playerShield.itemTemplate.ItemId == 0)
                multiplier += playerInventory.playerGauntlets.itemTemplate.getAttackValue(); // If hit with hand and gauntlets equipped.
            if(hitBone == HitBone.PT_LeftHand && playerInventory.playerShield.itemTemplate.ItemId != 0)
                multiplier += playerInventory.playerShield.itemTemplate.getDefenceValue() / 2; // If hit with hand and shield equipped.
            if((hitBone == HitBone.PT_RightFoot || hitBone == HitBone.PT_LeftFoot) && playerInventory.playerBoots.itemTemplate.ItemId != 0)
                multiplier += playerInventory.playerBoots.itemTemplate.getAttackValue(); // If hit with foot and boots equipped.
            multiplier += heavyAttack ? 0.1f : 0f; // If unarmed attack, if it was heavy attack then add small multiplier.

            dealDamage *= multiplier;
        } else {
            WeaponryItem weaponryItem = playerInventory.playerWeaponPrimary.itemTemplate as WeaponryItem;
            float halfway = weaponryItem.minDamageAmount + weaponryItem.maxDamageAmount / 2; // If heavy attack, find value from second half.
            dealDamage = heavyAttack ? UnityEngine.Random.Range(halfway, weaponryItem.maxDamageAmount) : UnityEngine.Random.Range(weaponryItem.minDamageAmount, halfway);
        }
        
        return dealDamage;
    }
    
    public void enableDisableShoot(string enable)
    {
        // Enables and disables ranged weapon shooting.
        RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
        rangedWeapon.setCanShoot(enable.ToLower() != "false");
    }

    public void takeArrowVisual()
    {
        // Spawns the arrow visual in the player's hand.
        RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
        Instantiate(rangedWeapon.ammoPrefab, GameObject.Find("Male_Weapon_Ammo_Left").transform);
    }

    public void placeArrowVisual()
    {
        // Places the arrow in the crossbow chamber.
        Destroy(GameObject.Find("Male_Weapon_Ammo_Left ").transform.GetChild(0).gameObject);
        RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
        rangedWeapon.toggleAmmoVisibility(true);
        GetComponent<PlayerInventory>().reduceAmmo();
    }
    
    public void arrowStringPull()
    {
        RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
        rangedWeapon.pullString();
    }
    
    public void fireArrow()
    {
        RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
        rangedWeapon.shootArrow();
    }

    public void onFireEnd()
    {
        RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
        
        // If still aiming when fire animation end, try reloading.
        MovementStateManager movementStateManager = GetComponent<MovementStateManager>();
        if(movementStateManager.rotationMode == RotationMode.Aiming)
            rangedWeapon.tryReload();
    }
}
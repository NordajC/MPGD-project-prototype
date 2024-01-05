using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("References")]
    Animator animator;
    PlayerInventory playerInventory;
    PlayerCombat playerCombat;

    [Header("Ammo")]
    public int inChamber = 0;
    public int maxInChamber = 1;
    public int totalAmmo;
    public bool reloading;
    public GameObject ammoPrefab;
    public GameObject ammoVisual;
    
    [Header("Ranged")]
    private bool canShoot = false;
    private bool shooting = false;
    public GameObject arrowPrefab;
    public Transform projectileSpawnPoint;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        playerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();
    }

    public void setCanShoot(bool enable)
    {
        // Enables and disables ranged weapon shooting.
        canShoot = enable;
    }

    public void toggleAmmoVisibility(bool visible)
    {
        // When string pull, show arrow visual on crossbow.
        ammoVisual.SetActive(visible);
    }
    
    public void pullString()
    {
        // Pull string by playing animation.
        animator.Play("StringPull");
    }
    
    public void updateAmmo()
    {
        inChamber = maxInChamber; // Set in chamber to max amount.
        playerInventory.updateAmmoDisplay();
        reloading = false;

        playerCombat.canAttack = true;
    }
    
    public void tryReload()
    {
        // Can only reload if not already reloading, have ammo, and no ammo in chamber.
        if(!reloading && totalAmmo > 0 && inChamber == 0)
        {
            // Reload by playing animation.
            GameObject.FindWithTag("Player").GetComponent<Animator>().CrossFade("Reloading", 0.1f);
            GameObject.FindWithTag("Player").GetComponent<PlayerCombat>().canAttack = false;
            reloading = true;
        }
    }

    public void shootArrow()
    {
        // Spawn arrow prefab and call shoot function.
        GameObject arrow = Instantiate(arrowPrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        arrow.GetComponent<Projectile>().shootProjectile();
        inChamber = 0;
        playerInventory.updateAmmoDisplay();
        shooting = false;

        playerCombat.canAttack = true;
    }

    public void tryShoot()
    {
        // Can only shoot if not already shooting and ammo in chamber.
        if(!shooting && inChamber > 0)
        {
            // Shoot by playing animation.
            GameObject.FindWithTag("Player").GetComponent<Animator>().CrossFade("ShootArrow", 0.1f);
            playerCombat.canAttack = false;
            
            Animator animator = GetComponent<Animator>();
            animator.CrossFade("Fire", 0f);
            toggleAmmoVisibility(false);
            
            shooting = true;
            
        }
    }
}

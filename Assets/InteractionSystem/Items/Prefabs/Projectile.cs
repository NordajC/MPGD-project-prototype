using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Defaults")]
    public float destroyAfter = 20f;
    public float shootForce = 40f;
    public Rigidbody rb;
    public float traceLength = 500f;
    public LayerMask layerMask;
    public float gravityMultiplier = 0.4f;

    Vector3 hitPos;
    
    public void Start()
    {
        // If not destroyed by impact, it will destroy after a set time.
        Destroy(gameObject, destroyAfter);
    }

    public void shootProjectile()
    {
        Vector3 rayStart = Camera.main.transform.position + (Camera.main.transform.forward * 4f);
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Raycast to center of screen.

        RaycastHit hit;
        Vector3 hitPosition;
        if (Physics.Raycast(rayStart, ray.direction, out hit, traceLength, layerMask)) 
        {
            hitPosition = hit.point; // If raycast hit something, set that as the target location.
        } else {
            hitPosition = ray.GetPoint(traceLength); // If raycast does not hit, then end point of ray is set.
        }
        hitPos = hitPosition;
        rb.velocity = (hitPosition - transform.position).normalized * shootForce; // To move the arrow.
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if((layerMask.value & (1 << other.transform.gameObject.layer)) > 0) // Checks that the hit object was in the layer mask.
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // If hit object was enemy, hit reaction called.
                EnemyAi enemyAi = other.gameObject.GetComponent<EnemyAi>();
                HitDirection hitDirection = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>().getHitDirection(gameObject);
                float damageAmount = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>().getDamageAmount(HitBone.WeaponRanged, false);
                enemyAi.onHitReaction(false, transform.position, hitDirection, HitHeight.Lower, GameObject.FindWithTag("Player"), damageAmount);
            }
            
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        transform.Rotate(0, -90, 0);
        rb.AddForce(Physics.gravity * gravityMultiplier);
    }
}

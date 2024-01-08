// using System.Collections;OnRockBreak
// using UnityEngine;

// public class Rock : MonoBehaviour
// {
//     [Header("Health")]
//     public float maxHealth = 50;
//     private float currentHealth;
//     public StatsBar HPBar;
//     public event System.Action OnRockBreak;

//     [SerializeField]
//     private GameObject DestroyedRock;

//     public enum DamageSource
//     {
//         Pickaxe,
//         Axe,

//     }

//     [SerializeField]
//     public DamageSource allowedDamageSource;

//     void Start()
//     {
//         if (DestroyedRock != null)
//         {
//             DestroyedRock.SetActive(false);
//         }
//         else
//         {
//             Debug.LogError("DestroyedRock not assigned in the Inspector.");
//         }

//         currentHealth = maxHealth;
        

//         // Assuming HPBar is a UI element like a Slider or an Image fill, make it visible here.
//         // If it's not a Slider or Image fill, you'll need to adjust this accordingly.
//         if (HPBar != null)
//         {
//             HPBar.gameObject.SetActive(true); // Ensure the GameObject that contains the HP bar is active.
//             HPBar.maxValue = maxHealth;
//             HPBar.updateStatBarValue(currentHealth);
//         }
//         else
//         {
//             Debug.LogError("HPBar not assigned in the Inspector.");
//         }
//     }


//     private IEnumerator TestDecreaseHealth()
//     {
//         // Decrease health over 3 seconds
//         float time = 0;
//         while (time < 3)
//         {
//             currentHealth -= maxHealth / 3 * Time.deltaTime; // Decrease a third of health per second
//             if (HPBar != null)
//             {
//                 HPBar.updateStatBarValue(currentHealth);
//             }
//             if (currentHealth <= 0)
//             {
//                 BreakRock();
//                 yield break; // Exit the coroutine if the rock is broken
//             }
//             time += Time.deltaTime;
//             yield return null;
//         }
//     }

//     public void TakeDamage(float damageAmount)
//     {
//         currentHealth -= damageAmount;
//         if (HPBar != null)
//         {
//             HPBar.updateStatBarValue(currentHealth);
//         }

//         if (currentHealth <= 0)
//         {
//             BreakRock();
//         }
//     }

//     private void BreakRock()
//     {
//         gameObject.SetActive(false);

//         if (DestroyedRock != null)
//         {
//             DestroyedRock.SetActive(true);
//             foreach (Rigidbody rb in DestroyedRock.GetComponentsInChildren<Rigidbody>(true))
//             {
//                 rb.isKinematic = false;
//                 rb.AddExplosionForce(500f, transform.position, 1f);
//             }
//         }
//         OnRockBreak?.Invoke();
//     }

//     public void AttemptToDamageRock()
//     {
//         // Call TakeDamage with a fixed damage value for testing.
//         TakeDamage(10f);
//     }


//     public void OnCollisionEnter(Collision collision)
//     {
//         Tool tool = collision.gameObject.GetComponent<Tool>();
//         if (tool != null && collision.gameObject.tag == allowedDamageSource.ToString())
//         {
//             float damage = tool.GetDamage();
//             TakeDamage(damage);
//         }
//     }
// }

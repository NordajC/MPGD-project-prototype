using UnityEngine;

public class OverrideTest : MonoBehaviour
{
    public Animator animator;
    public AnimatorOverrideController defaultController;
    public AnimatorOverrideController SwordShieldOverride;
    public AnimatorOverrideController CrossbowOverride;
    public AnimatorOverrideController TorchOverride;
    public AnimatorOverrideController SingleHandedOverride;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Switching to default controller");
            animator.runtimeAnimatorController = defaultController;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Switching to shield controller");
            animator.runtimeAnimatorController = SwordShieldOverride;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Switching to crossbow controller");
            animator.runtimeAnimatorController = CrossbowOverride;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Switching to crossbow controller");
            animator.runtimeAnimatorController = TorchOverride;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Switching to crossbow controller");
            animator.runtimeAnimatorController = SingleHandedOverride;
        }
    }

}

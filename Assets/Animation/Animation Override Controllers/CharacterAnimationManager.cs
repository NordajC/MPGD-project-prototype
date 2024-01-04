using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    public Animator animator;
    public AnimatorOverrideController SwordShieldOverride;
    
    // Add more override controllers as needed

    private RuntimeAnimatorController originalAnimatorController;

    void Start()
    {
        // Cache the original Animator Controller
        originalAnimatorController = animator.runtimeAnimatorController;
    }

    public void ApplyAnimationOverrides(string equippedItem)
    {
        switch (equippedItem)
        {
            case "SwordShield":
                animator.runtimeAnimatorController = SwordShieldOverride;
                break;
            // Add cases for other items
            default:
                animator.runtimeAnimatorController = originalAnimatorController;
                break;
        }
    }
}

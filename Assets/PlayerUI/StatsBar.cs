using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [Header("Defaults")]
    [HideInInspector] public float maxValue;
    Animator animator;
    public Slider slider;
    public Image lagBar;
    public float lagSpeed = 0.08f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // If lag bar and main bar not at same level, then smoothly move the lag bar.
        if(slider.value != lagBar.fillAmount)
            lagBar.fillAmount = Mathf.MoveTowards(lagBar.fillAmount, slider.value, lagSpeed * Time.deltaTime);
    }

    public void updateStatBarValue(float value)
    {
        slider.value = value/maxValue;
    }
    
    public void updateStatBar(float value, bool regen)
    {
        slider.value = value/maxValue; // Sets the bar. Value is divided so it is in range from 0 - 1 for the progress slider.

        // Play respective animation based on if gained or lost stat.
        if(regen)
        {
            animator.SetTrigger("Increased");
        } else {
            animator.SetTrigger("Decreased");
            animator.SetFloat("Random", Random.Range(0, 3)); // For random shake direction.
        }
    }
}
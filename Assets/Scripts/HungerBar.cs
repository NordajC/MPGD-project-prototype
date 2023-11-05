using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    Slider hungSlider;

    private void Start()
    {
        hungSlider = GetComponent<Slider>();
    }

    public void SetMaxHung(int maxHung)
    {
        hungSlider.maxValue = maxHung;
        hungSlider.value = maxHung;
    }

    public void SetHung(int hung)
    {
        hungSlider.value = hung;
    }
}

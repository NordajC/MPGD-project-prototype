using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    Slider stamSlider;

    private void Start()
    {
        stamSlider = GetComponent<Slider>();
    }

    public void SetMaxStam(int maxStam)
    {
        stamSlider.maxValue = maxStam;
        stamSlider.value = maxStam;
    }

    public void SetStam(int stam)
    {
        stamSlider.value = stam;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterBar : MonoBehaviour
{
    Slider waterSlider;

    private void Start()
    {
        waterSlider = GetComponent<Slider>();
    }

    public void SetMaxWater(int maxWater)
    {
        waterSlider.maxValue = maxWater;
        waterSlider.value = maxWater;
    }

    public void SetWater(int water)
    {
        waterSlider.value = water;
    }
}

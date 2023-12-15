using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public GameObject mainLight;
    public TextMeshProUGUI dayCountText;
    public float timepassing = 0.1f;
    public float time;
    public int dayCount = 1;

    private void Start()
    {
        dayCountText.text = "Day: " + dayCount.ToString();
    }


    private void Update()
    {
        time += timepassing * Time.deltaTime;
        mainLight.transform.rotation = Quaternion.Euler(time, 0, 0);

        if (time >= 360)
        {
            time -= 360;
            dayCount++;
            dayCountText.text = "Day: " + dayCount.ToString();
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public Light mainLight;
    public TextMeshProUGUI dayCountText;
    public Material skyboxMaterial;
    public float timepassing = 0.1f;
    public float time;
    public int dayCount = 1;

    // Variables for skybox color adjustment
    public Color dayColor = Color.white;
    public Color nightColor = Color.black;

    // Variables for light intensity control
    public float maxIntensity = 0.8f; // Maximum brightness during the day
    public float minIntensity = 0f; // No light at night

    // Ambient Light settings
    public Color dayAmbientColor = new Color(0.3f, 0.3f, 0.3f);
    public Color nightAmbientColor = Color.black;

    private void Start()
    {
        dayCountText.text = "Day: " + dayCount.ToString();
        RenderSettings.skybox = skyboxMaterial;
    }

    private void Update()
    {
        time += timepassing * Time.deltaTime;

        // Adjust light rotation
        mainLight.transform.rotation = Quaternion.Euler(time, 0, 0);

        // Control light intensity
        float intensityFactor = GetIntensityFactor(time);
        mainLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, intensityFactor);

        // Adjust skybox and ambient light color
        AdjustSkyboxAndAmbientLight(intensityFactor);

        if (time >= 360)
        {
            time -= 360;
            dayCount++;
            dayCountText.text = "Day: " + dayCount.ToString();
        }

        DynamicGI.UpdateEnvironment();
    }

    private float GetIntensityFactor(float currentTime)
    {
        float dawnStart = 30f;
        float dayStart = 60f;
        float duskStart = 270f;
        float nightStart = 300f;

        if (currentTime < dawnStart || currentTime >= nightStart)
            return 0f;
        else if (currentTime >= dayStart && currentTime < duskStart)
            return 1f;
        else if (currentTime < dayStart)
            return (currentTime - dawnStart) / (dayStart - dawnStart);
        else
            return (nightStart - currentTime) / (nightStart - duskStart);
    }

    private void AdjustSkyboxAndAmbientLight(float intensityFactor)
    {
        Color currentSkyboxColor = Color.Lerp(dayColor, nightColor, intensityFactor);
        skyboxMaterial.SetColor("_Tint", currentSkyboxColor);

        Color currentAmbientColor = Color.Lerp(dayAmbientColor, nightAmbientColor, intensityFactor);
        RenderSettings.ambientLight = currentAmbientColor;
    }
}

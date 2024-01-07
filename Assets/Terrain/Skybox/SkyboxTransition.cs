using System.Collections;
using UnityEngine;
using TMPro;

public class SimpleDayNightCycle : MonoBehaviour
{
    public Material skyboxMaterial; // Assign your single skybox material here
    public Light directionalLight; // Assign your directional light representing the sun
    public TextMeshProUGUI dayCountText; // UI Text element to display day count
    public float maxLightIntensity = 1f; // Maximum intensity of the directional light
    public float minExposure = 0.2f; // Minimum exposure for the skybox at night
    public float dayCycleDuration = 120f; // Total duration of a day in seconds
    public Vector3 rotationAxis = new Vector3(1, 0, 0); // Axis around which the light will rotate, typically X-axis for day/night
    public float rotationSpeed = 15f; // Speed at which the directional light will rotate, you can adjust this value as needed

    private float currentExposure; // Current exposure value
    private float dayCount = 0; // Start at day 0
    private float cycleTimer = 0; // Timer to track the cycle duration

    private void Start()
    {
        // Set the initial exposure to the skybox's "_Exposure" if it exists or to a default value
        currentExposure = skyboxMaterial.HasProperty("_Exposure") ? skyboxMaterial.GetFloat("_Exposure") : 1f;
        RenderSettings.skybox = skyboxMaterial;
        UpdateDayCount();
    }

    private void Update()
    {
        // Update the cycle timer
        cycleTimer += Time.deltaTime;

        // Calculate the percentage of the day cycle completed
        float cycleProgress = cycleTimer / dayCycleDuration;
        // Use a sine wave to get a smooth transition for the exposure and light intensity
        float exposure = minExposure + (Mathf.Sin(cycleProgress * Mathf.PI * 2) * 0.5f + 0.5f) * (maxLightIntensity - minExposure);
        float intensity = Mathf.Sin(cycleProgress * Mathf.PI * 2) * 0.5f + 0.5f; // Ranges from 0 to 1

        // Set the exposure and intensity based on the sine wave
        skyboxMaterial.SetFloat("_Exposure", exposure);
        directionalLight.intensity = intensity * maxLightIntensity;

        // Rotate the directional light around the specified axis
        directionalLight.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime * intensity);

        // Check if a full day cycle has passed
        if (cycleTimer >= dayCycleDuration)
        {
            // Increment the day count
            dayCount++;
            // Reset the timer for the next day
            cycleTimer = 0;
            UpdateDayCount();
        }
    }

    private void UpdateDayCount()
    {
        // Update the day count UI text
        dayCountText.text = $"Day: {dayCount + 1}";
    }
}


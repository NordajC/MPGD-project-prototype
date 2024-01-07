using System.Collections;
using UnityEngine;
using TMPro;

public class SkyboxDayNightCycle : MonoBehaviour
{
    public Material daySkybox;
    public Material blendSkybox;
    public Material nightSkybox;
    public Light directionalLight; // Assign your directional light representing the sun
    public TextMeshProUGUI dayCountText; // UI Text element to display day count
    public int dayCount = 0; // Start at day 1

    public float transitionDuration = 2.0f; // Duration of the transition between skyboxes
    public float dayDuration = 120.0f; // Total duration of a day in seconds

    private Material currentSkybox;
    private Material targetSkybox;

    private void Start()
    {
        // Start with the day skybox
        currentSkybox = daySkybox;
        RenderSettings.skybox = currentSkybox;
        dayCountText.text = $"Day: {dayCount}";

        StartCoroutine(DayNightCycle());
    }

    private IEnumerator DayNightCycle()
    {
        while (true)
        {
            // Wait for the daytime duration
            yield return new WaitForSeconds(dayDuration - transitionDuration);
            // Start the transition to blend skybox
            targetSkybox = blendSkybox;
            StartCoroutine(TransitionSkybox());

            // Wait for the transition to complete
            yield return new WaitForSeconds(transitionDuration);

            // Increment day count after the full day cycle
            dayCount++;
            dayCountText.text = $"Day: {dayCount}";

            // Wait for the nighttime duration
            yield return new WaitForSeconds(dayDuration - transitionDuration);
            // Start the transition to night skybox
            targetSkybox = nightSkybox;
            StartCoroutine(TransitionSkybox());

            // Wait for the transition to complete
            yield return new WaitForSeconds(transitionDuration);

            // Prepare for the transition back to day skybox after the night cycle
            currentSkybox = nightSkybox;
            targetSkybox = daySkybox;
        }
    }

    private IEnumerator TransitionSkybox()
    {
        float elapsedTime = 0.0f;

        // Get initial light intensity and exposure
        float initialIntensity = directionalLight.intensity;
        float initialExposure = currentSkybox.GetFloat("_Exposure");

        // Calculate target exposure based on whether we are transitioning to day or night
        float targetExposure = targetSkybox == daySkybox ? daySkybox.GetFloat("_Exposure") : 0f; // Set to 0 for night

        // Calculate target intensity for the light
        float targetIntensity = targetSkybox == daySkybox ? 1f : 0f; // Full intensity for day, 0 for night

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float blend = elapsedTime / transitionDuration;

            // Interpolate the exposure and light intensity
            float exposure = Mathf.Lerp(initialExposure, targetExposure, blend);
            float intensity = Mathf.Lerp(initialIntensity, targetIntensity, blend);

            // Assign the interpolated values
            RenderSettings.skybox.SetFloat("_Exposure", exposure);
            directionalLight.intensity = intensity;

            // For debugging
            Debug.Log($"Transitioning Skybox: {blend * 100}% complete");

            yield return null;
        }

        // Ensure we end with the exact target skybox and settings
        RenderSettings.skybox = targetSkybox;
        currentSkybox = targetSkybox;
        directionalLight.intensity = targetIntensity;

        // For debugging
        Debug.Log("Transition complete");
    }
}

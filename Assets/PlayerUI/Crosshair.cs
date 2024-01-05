using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("References")]
    private Image image;
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    // Fade crosshair in and out.
    public void updateOpacity(float opacity)
    {
        canvasGroup.alpha = opacity;
    }
}
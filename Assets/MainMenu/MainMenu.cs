using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject camera;
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mousePositionNormalized = new Vector3(Mathf.Clamp01(mousePosition.x / Screen.width), Mathf.Clamp01(mousePosition.y / Screen.height));
        
        Vector3 targetPosition = new Vector3(mousePositionNormalized.x * 0.15f, mousePositionNormalized.y * 0.15f, -10f);
        camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, 0.02f);
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene("Terrain");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
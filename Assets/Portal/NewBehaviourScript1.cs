using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInteraction : MonoBehaviour
{
    public Transform playerTransform; // Assign this in the inspector with your player's transform

    private void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= 2f && Input.GetKeyDown(KeyCode.E))
        {
            EndGame();
        }
    }

    void EndGame()
    {
        // Your game ending logic here
        Debug.Log("Game Ended"); // Placeholder action
        // SceneManager.LoadScene("GameOverScene"); // Example of loading a new scene
    }
}

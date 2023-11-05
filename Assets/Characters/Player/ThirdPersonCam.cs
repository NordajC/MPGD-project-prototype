using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    public CameraStyle currentStyle;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject crosshair;

    public bool cameraEnabled = true;

    public enum CameraStyle
    {
        Basic,
        Combat
    }

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //switch camera styles
        if (Input.GetKeyDown(KeyCode.F1)) { SwitchCameraStyle(CameraStyle.Basic); }
        if (Input.GetKeyDown(KeyCode.F2)) { SwitchCameraStyle(CameraStyle.Combat);}

        //rotate player obj
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //rotate player obj
        if (currentStyle == CameraStyle.Basic)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);

            }
        }

        else if (currentStyle == CameraStyle.Combat) 
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;
        }

    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        crosshair.SetActive(false);

        if (currentStyle == CameraStyle.Basic) 
        { 
            thirdPersonCam.SetActive(true);
            crosshair.SetActive(false);
        }

        if (currentStyle == CameraStyle.Combat)
        {  
            combatCam.SetActive(true);
            crosshair.SetActive(true);
        }

        currentStyle = newStyle;
    }

}

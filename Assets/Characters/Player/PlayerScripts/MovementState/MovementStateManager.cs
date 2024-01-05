using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

// Different rotation modes. Default will rotate to move direction. Aiming will use directional strafing.
public enum RotationMode
{
    Default,
    Aiming
}

public class MovementStateManager : MonoBehaviour
{
    [Header("Movement")]
    public float currentMoveSpeed = 0;
    public float walkSpeed = 3, walkBackSpeed =2;
    public float crouchSpeed = 2, crouchBackSpeed =1;
    public float runSpeed =7, runBackSpeed =5;
    [HideInInspector] public bool canMove = true;
    
    [Header("Jump/Air movement")]
    public float airSpeed = 1.5f;
    private float gravity = -9.81f;
    [SerializeField] float jumpForce = 1;
    [HideInInspector] public bool jumped;
    Vector3 velocity;

    [Header("Character Controller")]
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float hzInput, vInput, magnitude;
    CharacterController controller;

    [Header("Ground check")]
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [Header("States")]
    public MovementBaseState previousState;
    MovementBaseState currentState;
    public IdleState idle = new IdleState();
    public WalkState walk = new WalkState();
    public CrouchState crouch = new CrouchState();
    public RunState run = new RunState();
    public JumpState jump = new JumpState();

    [Header("Camera")]
    public RotationMode rotationMode;
    public GameObject aimCamera;
    public bool canAim;
    public Transform cameraFollowPos;
    public float defaultHeight = 1.4f;
    public float crouchHeight = 1.1f;

    [Header("Animations")]
    [HideInInspector] public Animator anim;

    void Start()
    {
        // Initialise defaults.
        anim = GetComponent<Animator>();
        controller = GameObject.Find("PlayerMain").GetComponent<CharacterController>();
        SwitchState(idle);

        // Cursor disabled by default.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Can move flag used to easily disable and enable movement.
        if(canMove)
        {
            GetDirectionAndMove();
            magnitude = Mathf.MoveTowards(magnitude, new Vector3(hzInput, 0, vInput).normalized.magnitude, 5f * Time.deltaTime);
        } else {
            hzInput = Mathf.Lerp(hzInput, 0f, 5f * Time.deltaTime);
            vInput = Mathf.Lerp(vInput, 0f, 5f * Time.deltaTime);
            magnitude = Mathf.MoveTowards(magnitude, 0, 5f * Time.deltaTime);
        }    

        Gravity();
        Falling();

        // Setting animation properties.
        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);
        anim.SetFloat("RotationMode", (int)rotationMode);
        anim.SetBool("aiming", rotationMode == RotationMode.Aiming);
        anim.SetFloat("magnitude", magnitude);

        currentState.UpdateState(this); // Setting the current movement state.
    }

    public void SwitchState(MovementBaseState state)
    {
        // Used to easily switch movement state.
        currentState = state;
        currentState.EnterState(this);
    }

    private void OnAim(InputValue value)
    {
        // Called when right mouse button pressed. Switches to aiming mode.

        bool hasCameraInput = gameObject.GetComponent<AimStateManager>().enabled;
        
        // Can only aim if a ranged weapon is equipped.
        PlayerInventory playerInventory = GetComponent<PlayerInventory>();
        WeaponryItem weaponryItem = playerInventory.playerWeaponPrimary.itemTemplate as WeaponryItem;
        bool isRangedWeapon = false;
        if(weaponryItem != null)
            isRangedWeapon = weaponryItem.weaponType == WeaponType.Ranged;
        
        if(value.isPressed && canAim && hasCameraInput && isRangedWeapon)
        {
            rotationMode = RotationMode.Aiming;
            aimCamera.SetActive(true); // Sets the aim camera to be active which has higher priority than the default. Smoothly switches.
            RangedWeapon rangedWeapon = (RangedWeapon)GetComponent<PlayerInventory>().equippedPrimary;
            rangedWeapon.tryReload();
        } else if(!isRangedWeapon && playerInventory.playerShield.itemTemplate.ItemId != 0) {
            GetComponent<PlayerCombat>().isBlocking = true;
        } else {
            rotationMode = RotationMode.Default;
            aimCamera.SetActive(false);

            GetComponent<PlayerCombat>().isBlocking = false;
        }
        
        anim.SetBool("blocking", GetComponent<PlayerCombat>().isBlocking);
    }

    void GetDirectionAndMove()
    {
        // Get input values.
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        
        // Sets rotation speed variable based on if grounded or not.
        float rotationSpeed = IsGrounded() ? 15f : 1f;
        
        // Forward and right components of camera used so player moves in that direction.
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;

        dir = cameraForward * vInput + cameraRight * hzInput; // Combines horizontal and vertical components.

        controller.Move((dir.normalized * currentMoveSpeed) * Time.deltaTime); // Moves the player.
        
        if(rotationMode == RotationMode.Default && dir != Vector3.zero)
        {
            // If rotation mode is default, rotate to face movement direction.
            var targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        } else if(rotationMode == RotationMode.Aiming) {
            // If rotation mode is aiming, rotate to face camera forward direction so player rotates with mouse movement.
            var targetRotation = Quaternion.LookRotation(Camera.main.transform.forward);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }        
    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y + controller.radius - 0.2f, transform.position.z);
        
        // Checks if player is grounded by checking if an overlap sphere is hitting the ground.
        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask))
        {
            return true;
        } else {
            return false;
        }
    }

    void Gravity()
    {
        // Sets velocity based on if grounded or not.
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        } else if (velocity.y < 0) {
            velocity.y = -2;
        }

        controller.Move(velocity *  Time.deltaTime);
    }

    // Sets falling animation bool.
    void Falling()=>anim.SetBool("Falling", !IsGrounded());

    // Sets velocity for jumping. Only y component needed for vertical change.
    public void JumpForce() => velocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravity);

    // Sets jumped variable to true when play jumps.
    public void Jumped() => jumped = true;
}

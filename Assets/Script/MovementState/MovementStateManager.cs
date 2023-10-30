using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float currentMoveSpeed = 0;
    public float walkSpeed = 3, walkBackSpeed =2;
    public float crouchSpeed = 2, crouchBackSpeed =1;
    public float runSpeed =7, runBackSpeed =5;
    public float airSpeed = 1.5f;


    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float hzInput, vInput;
    CharacterController controller;

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;
    [HideInInspector] public bool jumped;
    Vector3 velocity;

    public MovementBaseState previousState;
    MovementBaseState currentState;

    public IdleState idle = new IdleState();
    public WalkState walk = new WalkState();
    public CrouchState crouch = new CrouchState();
    public RunState run = new RunState();
    public JumpState jump = new JumpState();

    [HideInInspector] public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        SwitchState(idle);
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionAndMove();
        Gravity();
        Falling();

        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

        currentState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void GetDirectionAndMove()
    {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        Vector3 airDir = Vector3.zero;

        if (!IsGrounded())
        {
            airDir = transform.forward * vInput + transform.right * hzInput;
        }
        else
        {
            dir = transform.forward * vInput + transform.right * hzInput;
        }

        

        controller.Move((dir.normalized * currentMoveSpeed + airDir.normalized * airSpeed) * Time.deltaTime);

    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y + controller.radius - 0.08f, transform.position.z);

        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2;
        }

        controller.Move(velocity *  Time.deltaTime);
    }

    void Falling()=>anim.SetBool("Falling", !IsGrounded());

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spherePos, controller.radius - 0.05f);
    }

    public void JumpForce() => velocity.y += jumpForce;

    public void Jumped() => jumped = true;
}

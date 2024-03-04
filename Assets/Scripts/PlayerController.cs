using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Running, Jumping, Channeling, Dead };
    private PlayerState currentState = PlayerState.Idle;
    private float inputHorizontal;
    private float inputVertical;
    private Rigidbody rb;

    private Vector3 inputDirection = Vector3.zero;
    [SerializeField] private GameObject animatorCharacterModel;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float mouseRotSpeed;
    [SerializeField] private GameObject groundCheckObject;
    [SerializeField] private bool isTouchingGround;
    [SerializeField] private int currJumpCount;
    [SerializeField] private int maxJumpCount;

    void Start()
    {

        animator = animatorCharacterModel.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        StateManager();
        AnimatorController();
        MovementInput();
        JumpHandler();
    }

    void AnimatorController()
    {


    }
    void StateManager()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                GetMovementTrigger();
                MovementInput();
                break;

            case PlayerState.Running:
                GetMovementTrigger();
                MovementInput();
                break;

            case PlayerState.Jumping:
                AnimTrigger("Jumping");
                break;

            case PlayerState.Channeling:
                break;

            case PlayerState.Dead:
                break;

        }

    }

    void SetAnimInputs()
    {

    }



    void GetMovementTrigger()
    {
        if (inputVertical > 0.5f && inputHorizontal == 0f)
            AnimTrigger("RunningN");
        else if (inputVertical < -0.5f && inputHorizontal == 0f)
            AnimTrigger("RunningS");
        else if (inputVertical > 0.5f && inputHorizontal > 0.5f)
            AnimTrigger("RunningNE");
        else if (inputVertical > 0.5f && inputHorizontal > 0.5f)
            AnimTrigger("RunningNW");
        else if (inputHorizontal < -0.5f && inputVertical < 0.5f && inputVertical > -0.5f)
            AnimTrigger("RunningW");
        else if (inputHorizontal > 0.5f && inputVertical < 0.5f && inputVertical > -0.5f)
            AnimTrigger("RunningE");
        else if (inputVertical < -0.5f && inputHorizontal > 0.5f)
            AnimTrigger("RunningSE");
        else if (inputVertical < -0.5f && inputHorizontal > 0.5f)
            AnimTrigger("RunningSW");
        else
            AnimTrigger("Idle");
    }

    void AnimTrigger(string trigger)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(trigger))
            animator.SetTrigger(trigger);
    }

    void MovementInput()
    {
        // foreach(string enumName in System.Enum.GetNames())
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputDirection = transform.TransformDirection(new Vector3(inputHorizontal, 0f, inputVertical));

        if (inputHorizontal > 0f || inputVertical > 0f)
            currentState = PlayerState.Running;
        else
            currentState = PlayerState.Idle;
    }

    void SetRunningAnimInputs()
    {

    }

    // void RunningDirectionAnimationTrigger()

    void JumpHandler()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        currentState = PlayerState.Jumping;


    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(inputDirection.x * moveSpeed, rb.velocity.y, inputDirection.z * moveSpeed);
        if (Input.GetAxis("Mouse X") > 0) transform.Rotate((Vector3.up) * mouseRotSpeed);
        if (Input.GetAxis("Mouse X") < 0) transform.Rotate((Vector3.up) * -mouseRotSpeed);
        // if (Input.GetAxis("Mouse Y") > 0) transform.Rotate((Vector3.up) * mouseRotSpeed);
        // if (Input.GetAxis("Mouse Y") < 0) transform.Rotate((Vector3.up) * -mouseRotSpeed);

    }
}

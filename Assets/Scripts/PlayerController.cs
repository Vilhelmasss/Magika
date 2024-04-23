using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Running, Jumping, ChannelWalk, ChannelStand, Dead };
    public PlayerState currentState = PlayerState.Idle;
    public static PlayerController Instance { get; private set; }
    public float channelWalkingMultiplier;
    private float inputHorizontal;
    private float inputVertical;
    private Rigidbody rb;
    public Vector3 testVector;
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
        Instance = this;
        animator = animatorCharacterModel.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        testVector = groundCheckObject.transform.position;

        StateManager();
        MovementInput();
        JumpHandler();
    }

    void StateManager()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                GetMovementTrigger();
                CheckGround();
                MovementInput();
                break;

            case PlayerState.Running:
                GetMovementTrigger();
                CheckGround();
                MovementInput();
                break;

            case PlayerState.Jumping:
                // GetJumpingTrigger();
                CheckGround();
                break;

            case PlayerState.ChannelStand:

                break;
            case PlayerState.ChannelWalk:
                Debug.Log($"currentState: {currentState}");
                GetMovementTrigger();
                MovementInput();
                break;
            case PlayerState.Dead:

                break;

        }
    }

    void CheckGround()
    {
        Collider[] hitColliders = Physics.OverlapSphere(testVector, 0.1f, ~7);
        Debug.Log($"Colliders: {currentState}");
        if (hitColliders.Length > 0)
        {
            isTouchingGround = true;
            if (currentState == PlayerState.Jumping)
                ChangeState(PlayerState.Idle);
        }
        else
        {
            isTouchingGround = false;
            ChangeState(PlayerState.Jumping);
        }
    }

    void GetMovementTrigger()
    {
        if (inputVertical > 0.01f && inputHorizontal == 0f)
            AnimTrigger("RunningForwards");
        else if (inputVertical < -0.01f && inputHorizontal == 0f)
            AnimTrigger("RunningBackwards");
        else if (inputVertical > 0.01f && inputHorizontal > 0.01f)
            AnimTrigger("RunningForwardsRight");
        else if (inputVertical > 0.01f && inputHorizontal < -0.01f)
            AnimTrigger("RunningForwardsLeft");
        else if (inputHorizontal < -0.01f && inputVertical < 0.01f && inputVertical > -0.01f)
            AnimTrigger("RunningLeft");
        else if (inputHorizontal > 0.01f && inputVertical < 0.01f && inputVertical > -0.01f)
            AnimTrigger("RunningRight");
        else if (inputVertical < -0.01f && inputHorizontal > 0.01f)
            AnimTrigger("RunningBackwardsRight");
        else if (inputVertical < -0.01f && inputHorizontal < -0.01f)
            AnimTrigger("RunningBackwardsLeft");
        else
            AnimTrigger("Idle");
    }

    void GetJumpingTrigger()
    {
        AnimTrigger(currentState.ToString());
    }

    void ChangeState(PlayerState newState)
    {
        currentState = newState;
    }

    void AnimTrigger(string trigger)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(trigger))
            animator.SetTrigger(trigger);
    }

    void MovementInput()
    {
        if (currentState != PlayerState.ChannelWalk)
            channelWalkingMultiplier = 1f;

        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputDirection = transform.TransformDirection(new Vector3(inputHorizontal, 0f, inputVertical));

        if (inputHorizontal != 0f && inputVertical != 0f)
            ChangeState(PlayerState.Running);
        else
            ChangeState(PlayerState.Idle);
    }

    void JumpHandler()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;
        AnimTrigger("Jumping");
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        ChangeState(PlayerState.Jumping);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(inputDirection.x * moveSpeed * channelWalkingMultiplier, rb.velocity.y, inputDirection.z * moveSpeed * channelWalkingMultiplier);

        // Horizontal rot
        if (Input.GetAxis("Mouse X") > 0)
            transform.Rotate(Vector3.up * mouseRotSpeed);
        if (Input.GetAxis("Mouse X") < 0)
            transform.Rotate(Vector3.up * -mouseRotSpeed);

        // Vertical rot
        // if (Input.GetAxis("Mouse Y") > 0)
        //     transform.Rotate(Vector3.right * -mouseRotSpeed);
        // if (Input.GetAxis("Mouse Y") < 0)
        //     transform.Rotate(Vector3.right * mouseRotSpeed);
    }
}

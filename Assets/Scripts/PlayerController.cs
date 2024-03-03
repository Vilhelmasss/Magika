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


        // moveDirection = new Vector3(inputHorizontal * moveSpeed, , inputVertical * moveSpeed);
    }

    void AnimatorController()
    {


    }
    void StateManager()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                SetAnimTrigger("Idle");
                MovementInput();
                break;

            case PlayerState.Running:
                SetAnimTrigger("RunningForward");
                MovementInput();
                break;
        }

    }

    void SetAnimTrigger(string trigger)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(trigger))
            animator.SetTrigger(trigger);
    }
    void MovementInput()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputDirection = transform.TransformDirection(new Vector3(inputHorizontal, 0f, inputVertical));

        if (inputHorizontal > 0f || inputVertical > 0f)
            currentState = PlayerState.Running;
        else
            currentState = PlayerState.Idle;

    }

    void JumpHandler()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        animator.SetTrigger("Jumping");
        currentState = PlayerState.Jumping;


    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(inputDirection.x * moveSpeed, rb.velocity.y, inputDirection.z * moveSpeed);
        if (Input.GetAxis("Mouse X") > 0) transform.Rotate((Vector3.up) * mouseRotSpeed);
        if (Input.GetAxis("Mouse X") < 0) transform.Rotate((Vector3.up) * -mouseRotSpeed);

    }
}

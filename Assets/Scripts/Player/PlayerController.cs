using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum CharacterState
    {
        IDLE,
        WALKING,
        JUMPING,
        CASTING,
        ATTACKING
    }


    public Rigidbody rb;
    public CharacterState currentState;
    public GameObject groundChecker;
    public bool isGrounded;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private float horizontalMove;
    private float verticalMove;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case CharacterState.IDLE:
                // Handle idle state
                break;
            case CharacterState.WALKING:
                // Handle walking state
                break;
            case CharacterState.JUMPING:
                // Handle jumping state
                break;
            case CharacterState.CASTING:
                // Handle casting state
                break;
            case CharacterState.ATTACKING:
                // Handle attacking state
                break;
            default:
                break;
        }


        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * moveSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        MoveCharacter(horizontalMove);
    }

    void MoveCharacter(float horizontal)
    {
        Vector3 movement = new Vector3(horizontal, 0f, verticalMove) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        currentState = CharacterState.JUMPING;
    }
}

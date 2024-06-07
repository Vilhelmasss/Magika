using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    float horizontalInput = 0f;
    float verticalInput = 0f;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float sensitivity;
    Rigidbody rb;
    void Awake()
    {
        
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        rb.velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("attack");
            Collider[] contacts = Physics.OverlapSphere(gameObject.transform.position, 10f);
            foreach (var contact in contacts)
            {
                if (contact.gameObject.layer == 6)
                {
                    Debug.Log("attack successful");
                    contact.GetComponent<HP>().TakeDamage(10);
                    contact.GetComponent<HP>().Bleed();
                }
            }
        }
    }
}
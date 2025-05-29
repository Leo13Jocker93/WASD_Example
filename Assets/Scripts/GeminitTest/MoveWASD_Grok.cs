using UnityEngine;

public class MoveWASD_Grok : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float moveSpeed = 5f;
    private float sprintSpeed = 8f;
    private float jumpForce = 5f;
    private float rotationSpeed = 720f; // Degrees per second for smooth 90-degree turns
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Lock cursor for third-person camera control
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check if grounded (simple ground check using raycast)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.2f);

        // Process movement input
        ProcessInput();
    }

    void FixedUpdate()
    {
        Move();
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void ProcessInput()
    {
        // Reset move direction
        moveDirection = Vector3.zero;

        // Forward movement (W)
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection = transform.forward;
        }
        // Backward movement (S) - 180-degree turn
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection = -transform.forward;
        }

        // Right movement (D) - 90-degree turn
        if (Input.GetKey(KeyCode.D))
        {
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            moveDirection = transform.forward;
        }
        // Left movement (A) - 90-degree turn
        else if (Input.GetKey(KeyCode.A))
        {
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            moveDirection = transform.forward;
        }
    }

    void Move()
    {
        // Apply movement speed (sprint or walk)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        Vector3 velocity = moveDirection.normalized * currentSpeed;
        velocity.y = rb.linearVelocity.y; // Preserve vertical velocity (for gravity/jumping)
        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    // Optional: Visualize ground check ray in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * 0.2f);
    }
}
/*using Unity.Android.Gradle.Manifest;
using UnityEngine;
[RequireComponent (typeof(CharacterController))]
public class New_CharacterController : MonoBehaviour
{
    
    [Header("Movimiento")]
    public float WalkSpeed = 4f;
    public float SrpintSpeed = 6f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 1f;
    public float gravity = -20f;

    [Header("Referenciación")]
    public Transform cameraTransform;
    public Animator animator;
    
    private CharacterController characterController;
    private Vector3 Velocity;
    private float currentSpeed;
    private float yaw;
    private Vector3 externalVelocity = Vector3.zero;

    public bool IsMoving {get; private set;}
    public Vector2 CurrentInput {get; private set;}
    public bool IsGrounded{get;private set;}
    public float CurrentYaw => yaw;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController> ();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        updateAnimator();
    }

    void HandleMovement()
    {
        IsGrounded = characterController.isGrounded;//verificación de tocar o no el suelo

        if (IsGrounded && Velocity.y < 0)
        {
            if(externalVelocity.y > -0.05f && externalVelocity.y < 0.05f)
                Velocity.y=0;
            else
                Velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        IsMoving = inputDirection.magnitude > 0.1f;

        Vector3 moveDirection = Vector3.zero;

        if(IsMoving)
        {
            moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isSprinting ? SrpintSpeed : WalkSpeed;
        }

        if(Input.GetButtonDown("Jump") && IsGrounded)
        {
            Velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator?.SetBool("isJumping", true);
        }

        Velocity.y += gravity*Time.deltaTime;

        Vector3 finalMovement = (moveDirection * currentSpeed + externalVelocity) * Time.deltaTime;
        finalMovement.y += Velocity.y * Time.deltaTime;

        characterController.Move(finalMovement);

        if(IsGrounded && Velocity.y < 0f)
        {
            animator?.SetBool("isJumping", false);
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw += mouseX;

        if(IsMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yaw, 0f), rotationSpeed*Time.deltaTime);
        }
    }

    void updateAnimator()
    {
        float SpeedPercent = IsMoving? (currentSpeed == SrpintSpeed? 1f : 0.5f) :0f;
        animator?.SetFloat("Speed", SpeedPercent, 0.1f, Time.deltaTime);
        animator?.SetBool("IsGrounded", IsGrounded);
        animator?.SetFloat("VerticalSpeed", Velocity.y);
    }

    public void SetExternalVelocity (Vector3 platformVelocity)
    {
        externalVelocity = platformVelocity;
    }
}*/
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class New_CharacterController : MonoBehaviour
{
    [Header("Movimiento")]
    public float WalkSpeed = 4f;
    public float SprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 1f;
    public float gravity = -20f;

    [Header("Ground Detection")]
    public float groundCheckRadius = 0.4f; // Radius for sphere check
    public float groundCheckDistance = 0.2f; // Distance below feet to check ground
    public LayerMask groundLayer; // Layers considered as ground

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f; // Time window to buffer jump input

    [Header("Referenciación")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController characterController;
    private Vector3 velocity;
    private float currentSpeed;
    private float yaw;
    private Vector3 externalVelocity = Vector3.zero;

    private bool jumpRequested; // Tracks if jump input was pressed
    private float jumpBufferTimer; // Timer for jump buffer

    public bool IsMoving { get; private set; }
    public Vector2 CurrentInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public float CurrentYaw => yaw;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Handle input in Update for responsiveness
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        CurrentInput = new Vector2(horizontal, vertical);

        // Jump input detection with buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
            jumpBufferTimer = jumpBufferTime;
        }

        // Decrease jump buffer timer
        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        else
        {
            jumpRequested = false;
        }
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        HandleMovement();
        HandleRotation();
        UpdateAnimator();
    }

    void CheckGroundStatus()
    {
        // Perform a sphere cast at the bottom of the character for ground detection
        Vector3 spherePosition = transform.position + Vector3.up * (characterController.radius * 0.5f);
        IsGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);

        // Fallback to CharacterController.isGrounded for edge cases
        IsGrounded |= characterController.isGrounded;
    }

    void HandleMovement()
    {
        // Reset vertical velocity when grounded
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = externalVelocity.y > -0.05f && externalVelocity.y < 0.05f ? 0f : -2f;
        }

        // Calculate movement direction
        Vector3 inputDirection = new Vector3(CurrentInput.x, 0f, CurrentInput.y).normalized;
        IsMoving = inputDirection.magnitude > 0.1f;

        Vector3 moveDirection = Vector3.zero;
        if (IsMoving)
        {
            moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;
        }

        // Handle jump with buffer
        if (jumpRequested && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator?.SetBool("isJumping", true);
            jumpRequested = false; // Clear jump request after applying
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine movement and velocity
        Vector3 finalMovement = (moveDirection * currentSpeed + externalVelocity) * Time.deltaTime;
        finalMovement.y += velocity.y * Time.deltaTime;

        characterController.Move(finalMovement);

        // Reset jumping animation when grounded
        if (IsGrounded && velocity.y < 0f)
        {
            animator?.SetBool("isJumping", false);
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw += mouseX;

        if (IsMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yaw, 0f), rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateAnimator()
    {
        float speedParam = 0f;
        float verticalInput = Input.GetAxis("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            speedParam = verticalInput < 0 ? -1f : (isSprinting ? 2f : 1f);
        }

        animator?.SetFloat("Speed", speedParam, 0.1f, Time.deltaTime);
        animator?.SetBool("IsGrounded", IsGrounded);
        animator?.SetFloat("VerticalSpeed", velocity.y);
    }

    public void SetExternalVelocity(Vector3 platformVelocity)
    {
        externalVelocity = platformVelocity;
    }

    // Optional: Visualize ground check in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 spherePosition = transform.position + Vector3.up * (characterController ? characterController.radius * 0.5f : 0.5f);
        Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
    }
}

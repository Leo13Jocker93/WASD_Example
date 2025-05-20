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
    public float groundCheckRadius = 0.4f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;

    [Header("Referenciación")]
    public Transform cameraTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private int combatLayerIndex = 1;
    [SerializeField] private CombatController combatController;

    private CharacterController characterController;
    private Vector3 velocity;
    private float currentSpeed;
    private float yaw;
    private Vector3 externalVelocity = Vector3.zero;

    private bool jumpRequested;
    private float jumpBufferTimer;

    public bool IsMoving { get; private set; }
    public Vector2 CurrentInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public float CurrentYaw => yaw;

    private bool turningLeft = false;
    private bool turningRight = false;
    private float turnResetTimer = 0f;
    private const float turnResetDelay = 0.1f;

    // Nuevo: rotación suave
    private Quaternion targetRotation;
    private bool isTurning = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        targetRotation = transform.rotation;

        if (combatController == null)
        {
            combatController = GetComponent<CombatController>();
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        CurrentInput = new Vector2(horizontal, vertical);

        // Combate: Activación de capa
        float targetWeight = combatController.IsInCombatMode() ? 1f : 0f;
        float currentWeight = animator.GetLayerWeight(combatLayerIndex);
        animator.SetLayerWeight(combatLayerIndex, Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 5f));
        animator.SetBool("IsInCombatMode", targetWeight > 0f);

        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
            jumpBufferTimer = jumpBufferTime;
        }

        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        else
        {
            jumpRequested = false;
        }

        // Giros suaves en idle
        if (Input.GetKey(KeyCode.A) && !isTurning && !IsMoving && turnResetTimer <= 0f)
        {
            targetRotation *= Quaternion.Euler(0f, -90f, 0f);
            turningLeft = true;
            turningRight = false;
            isTurning = true;
        }

        if (Input.GetKey(KeyCode.D) && !isTurning && !IsMoving && turnResetTimer <= 0f)
        {
            targetRotation *= Quaternion.Euler(0f, 90f, 0f);
            turningRight = true;
            turningLeft = false;
            isTurning = true;
        }

        if (turnResetTimer > 0f)
        {
            turnResetTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
        HandleMovement();
        HandleRotation();
        UpdateAnimator();
    }

    void CheckGroundStatus()
    {
        Vector3 spherePosition = transform.position + Vector3.up * (characterController.radius * 0.5f);
        IsGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        IsGrounded |= characterController.isGrounded;
    }

    void HandleMovement()
    {
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = externalVelocity.y > -0.05f && externalVelocity.y < 0.05f ? 0f : -2f;
        }

        Vector3 inputDirection = new Vector3(CurrentInput.x, 0f, CurrentInput.y).normalized;
        IsMoving = inputDirection.magnitude > 0.1f;

        Vector3 moveDirection = Vector3.zero;
        if (IsMoving)
        {
            moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;
        }

        if (jumpRequested && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator?.SetBool("isJumping", true);
            jumpRequested = false;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMovement = (moveDirection * currentSpeed + externalVelocity) * Time.deltaTime;
        finalMovement.y += velocity.y * Time.deltaTime;

        characterController.Move(finalMovement);

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
            Quaternion moveRotation = Quaternion.Euler(0f, yaw, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, rotationSpeed * Time.deltaTime);
            targetRotation = transform.rotation;
            isTurning = false;
            turningLeft = false;
            turningRight = false;
        }
        else if (isTurning)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * 20f * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                isTurning = false;
                turnResetTimer = turnResetDelay;
                turningLeft = false;
                turningRight = false;
            }
        }
    }

    void UpdateAnimator()
    {
        float horizontalParam = 0f;
        float verticalParam = 0f;

        if (isTurning && turningLeft)
            horizontalParam = -0.5f;
        else if (isTurning && turningRight)
            horizontalParam = 0.5f;
        else
            horizontalParam = Mathf.Clamp(CurrentInput.x, -1f, 1f);

        verticalParam = Mathf.Clamp(CurrentInput.y, -1f, 1f);

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        if (isSprinting && verticalParam > 0f)
            verticalParam = 1f;
        else if (verticalParam > 0f)
            verticalParam = 0.5f; // Walk forward
        else if (verticalParam < 0f)
            verticalParam = -0.5f; // Walk backward
        else
            verticalParam = 0f; // Idle

        if (!isTurning)
        {
            if (horizontalParam > 0f) horizontalParam = 0.5f;
            else if (horizontalParam < 0f) horizontalParam = -0.5f;
            else horizontalParam = 0f;
        }

        // Always update Base_Layer parameters to ensure walking animation plays
        animator?.SetFloat("Horizontal", horizontalParam, 0.1f, Time.deltaTime);
        animator?.SetFloat("Vertical", verticalParam, 0.1f, Time.deltaTime);
        animator?.SetBool("IsGrounded", IsGrounded);
        animator?.SetFloat("VerticalSpeed", velocity.y);
        animator?.SetBool("IsInCombatMode", combatController.IsInCombatMode());

        // CombatLayer weight is managed by CombatController, no need to set it here
    }

    public void SetExternalVelocity(Vector3 platformVelocity)
    {
        externalVelocity = platformVelocity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 spherePosition = transform.position + Vector3.up * (characterController ? characterController.radius * 0.5f : 0.5f);
        Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
    }
}

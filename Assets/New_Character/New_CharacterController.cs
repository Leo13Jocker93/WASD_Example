//using Unity.Android.Gradle.Manifest;
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

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

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

        // Manejo de layers según estado de combate
        bool shouldBeInCombat = combatController.IsInCombatMode();
        float targetWeight = shouldBeInCombat ? 1f : 0f;
        float currentWeight = animator.GetLayerWeight(combatLayerIndex);
        animator.SetLayerWeight(combatLayerIndex, Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 5f));
        animator.SetBool("IsInCombatMode", shouldBeInCombat);

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 spherePosition = transform.position + Vector3.up * (characterController ? characterController.radius * 0.5f : 0.5f);
        Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
    }
}

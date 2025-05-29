using UnityEngine;

public class MoveWASD_Claude : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;
    public float rotationSpeed = 720f; // Grados por segundo para rotación suave
    
    [Header("Configuración de Físicas")]
    public float gravity = -9.81f;
    public LayerMask groundMask = 1; // Layer del suelo
    public Transform groundCheck; // Transform vacío como hijo del jugador para detectar suelo
    public float groundDistance = 0.4f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    
    // Variables para rotación suave
    private float targetRotationY;
    private bool needsRotation;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Si no se asignó groundCheck, crear uno automáticamente
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
        }
        
        // Inicializar rotación objetivo
        targetRotationY = transform.eulerAngles.y;
    }
    
    void Update()
    {
        HandleGroundCheck();
        HandleInput();
        HandleMovement();
        HandleRotation();
        HandleGravityAndJump();
    }
    
    void HandleGroundCheck()
    {
        // Verificar si está en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Resetear velocidad vertical si está en el suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeño valor negativo para mantener al jugador pegado al suelo
        }
    }
    
    void HandleInput()
    {
        // Detectar si está corriendo
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        // Manejar rotaciones con WASD
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetTargetRotation(0f); // Hacia adelante (Z positivo)
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetTargetRotation(90f); // Hacia la derecha
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetTargetRotation(180f); // Hacia atrás (Z negativo)
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetTargetRotation(270f); // Hacia la izquierda
        }
        
        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
    
    void SetTargetRotation(float newRotation)
    {
        targetRotationY = newRotation;
        needsRotation = true;
    }
    
    void HandleRotation()
    {
        if (needsRotation)
        {
            float currentY = transform.eulerAngles.y;
            
            // Normalizar ángulos para evitar problemas con 360 grados
            if (currentY > 180f) currentY -= 360f;
            float targetY = targetRotationY;
            if (targetY > 180f) targetY -= 360f;
            
            // Calcular la diferencia más corta entre ángulos
            float angleDifference = Mathf.DeltaAngle(currentY, targetY);
            
            // Rotar suavemente hacia el objetivo
            if (Mathf.Abs(angleDifference) > 1f)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                float newY = currentY + Mathf.Sign(angleDifference) * Mathf.Min(rotationStep, Mathf.Abs(angleDifference));
                transform.rotation = Quaternion.Euler(0, newY, 0);
            }
            else
            {
                // Completar la rotación
                transform.rotation = Quaternion.Euler(0, targetRotationY, 0);
                needsRotation = false;
            }
        }
    }
    
    void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        
        // Solo moverse si se presiona alguna tecla de movimiento
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            // Moverse hacia adelante en la dirección que está mirando el jugador
            move = transform.forward;
            
            // Aplicar velocidad (caminar o correr)
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            move *= currentSpeed;
        }
        
        // Aplicar movimiento horizontal
        controller.Move(move * Time.deltaTime);
    }
    
    void HandleGravityAndJump()
    {
        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        
        // Aplicar velocidad vertical (gravedad y salto)
        controller.Move(velocity * Time.deltaTime);
    }
    
    // Método para visualizar el área de detección de suelo en el editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
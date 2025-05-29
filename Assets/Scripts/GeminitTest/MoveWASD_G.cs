using UnityEngine;

public class MoveWASD_G : MonoBehaviour
{
// --- Configuración de Velocidad ---
    public float walkSpeed = 3.0f; // Velocidad al caminar
    public float runSpeedMultiplier = 1.7f; // Multiplicador para la velocidad al correr (ej. 1.7x la velocidad de caminar)
    private float currentSpeed; // Velocidad actual del jugador

    // --- Configuración de Salto ---
    public float jumpForce = 7.0f; // Fuerza del salto
    public Transform groundCheck; // Objeto para verificar si el jugador está en el suelo (generalmente un GameObject vacío un poco debajo del jugador)
    public LayerMask groundLayer; // Capa que define lo que es "suelo"
    public float groundCheckRadius = 0.3f; // Radio para la detección del suelo

    private Rigidbody rb; // Componente Rigidbody del jugador
    private bool isGrounded; // Variable para saber si el jugador está en el suelo

    // --- Rotación y Orientación ---
    // Usaremos la rotación del propio transform para la orientación del movimiento
    // y aplicaremos rotaciones instantáneas con las teclas A, S, D.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("El Rigidbody no se encontró en el jugador. Asegúrate de añadir uno.");
        }
    }

    void Update()
    {
        // --- Detección de Suelo ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // --- Movimiento y Rotación ---
        HandleMovementAndRotation();

        // --- Salto ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleMovementAndRotation()
    {
        // Determinar la velocidad actual (caminar o correr)
        currentSpeed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= runSpeedMultiplier;
        }

        // Movimiento hacia adelante (W)
        if (Input.GetKey(KeyCode.W))
        {
            // Mueve el jugador en la dirección Z local (adelante) de su transform
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }

        // Rotación y movimiento hacia la derecha (D)
        if (Input.GetKeyDown(KeyCode.D))
        {
            // Rota 90 grados a la derecha instantáneamente
            transform.Rotate(0, 90, 0);
        }
        // Movimiento después de rotar con D (opcional, si quieres que se mueva automáticamente después de la rotación)
        // Puedes descomentar la siguiente línea si quieres que al presionar D, además de girar, avance
        // transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Rotación para ir hacia atrás (S)
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Rota 180 grados instantáneamente
            transform.Rotate(0, 180, 0);
        }
        // Movimiento después de rotar con S (opcional)
        // transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);


        // Rotación y movimiento hacia la izquierda (A)
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Rota 90 grados a la izquierda instantáneamente
            transform.Rotate(0, -90, 0);
        }
        // Movimiento después de rotar con A (opcional)
        // transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    // Opcional: Dibuja el radio de detección de suelo en el editor para depuración
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
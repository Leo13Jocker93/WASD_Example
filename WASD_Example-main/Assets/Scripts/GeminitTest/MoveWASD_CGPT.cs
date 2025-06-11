using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class MoveWASD_CGPT : MonoBehaviour
{
// --- Configuración de Velocidad ---
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float rotationY = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pegarlo al suelo
        }

        // Determine movement direction and rotation
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W))
        {
            rotationY = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            rotationY += 90f;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            rotationY += 180f;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            rotationY -= 90f;
        }

        // Normaliza la rotación (0 - 360)
        rotationY = Mathf.Repeat(rotationY, 360f);
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        // Movimiento hacia adelante en la dirección actual
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            moveDir = transform.forward;
        }

        // Correr con Shift
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        controller.Move(moveDir * speed * Time.deltaTime);

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
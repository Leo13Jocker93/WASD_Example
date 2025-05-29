using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MoveWASD_DeepSeek : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadCaminar = 5f;
    [SerializeField] private float velocidadCorrer = 8f;
    [SerializeField] private float velocidadRotacion = 10f;
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float gravedad = -9.81f;

    [Header("Referencias")]
    [SerializeField] private Transform camaraTransform;

    private CharacterController controller;
    private Vector3 movimiento;
    private float velocidadVertical;
    private bool estaCorriendo;
    private float rotacionObjetivo;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (camaraTransform == null && Camera.main != null)
        {
            camaraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        ProcesarEntrada();
        AplicarGravedad();
        MoverPersonaje();
    }

    private void ProcesarEntrada()
    {
        // Reiniciar movimiento
        movimiento = Vector3.zero;
        estaCorriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Movimiento hacia adelante (W)
        if (Input.GetKey(KeyCode.W))
        {
            movimiento += transform.forward;
            rotacionObjetivo = 0f; // Mirar hacia adelante (respecto al personaje)
        }

        // Movimiento hacia atrás (S) - gira 180 grados
        if (Input.GetKey(KeyCode.S))
        {
            movimiento += transform.forward;
            rotacionObjetivo = 180f; // Mirar hacia atrás
        }

        // Movimiento hacia la derecha (D) - gira 90 grados
        if (Input.GetKey(KeyCode.D))
        {
            movimiento += transform.forward;
            rotacionObjetivo = 90f; // Mirar hacia la derecha
        }

        // Movimiento hacia la izquierda (A) - gira -90 grados
        if (Input.GetKey(KeyCode.A))
        {
            movimiento += transform.forward;
            rotacionObjetivo = -90f; // Mirar hacia la izquierda
        }

        // Normalizar el vector de movimiento si hay entrada múltiple
        if (movimiento.magnitude > 0)
        {
            movimiento.Normalize();
        }

        // Salto (Espacio)
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocidadVertical = fuerzaSalto;
        }
    }

    private void MoverPersonaje()
    {
        // Rotación suave hacia la dirección objetivo
        float rotacionActual = transform.eulerAngles.y;
        float rotacionSuavizada = Mathf.LerpAngle(rotacionActual, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, rotacionSuavizada, 0f);

        // Calcular velocidad actual
        float velocidadActual = estaCorriendo ? velocidadCorrer : velocidadCaminar;

        // Aplicar movimiento
        Vector3 movimientoFinal = movimiento * velocidadActual * Time.deltaTime;
        movimientoFinal.y = velocidadVertical * Time.deltaTime;
        controller.Move(movimientoFinal);
    }

    private void AplicarGravedad()
    {
        if (controller.isGrounded && velocidadVertical < 0)
        {
            velocidadVertical = -0.5f; // Pequeña fuerza hacia abajo para mantener al personaje en el suelo
        }
        else
        {
            velocidadVertical += gravedad * Time.deltaTime;
        }
    }

    // Método para dibujar gizmos y ayudar en el debug
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, movimiento * 2f);
    }
}
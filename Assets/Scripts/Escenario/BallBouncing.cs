using UnityEngine;

public class BallBouncing : MonoBehaviour
{
    private Vector3 posicionInicial;
    private Rigidbody rb;

    void Start()
    {
        // Guardar la posición inicial
        posicionInicial = transform.position;

        // Obtener el Rigidbody
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Reiniciar si cae por debajo de y = -10
        if (transform.position.y < -10f)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = posicionInicial;
        }
      
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && rb.linearVelocity.y < 0.1f)
        {
            Vector3 normal = collision.contacts[0].normal;
            rb.linearVelocity = Vector3.Reflect(rb.linearVelocity, normal) * 0.8f; // 0.8 controla el rebote
        }
    }
}
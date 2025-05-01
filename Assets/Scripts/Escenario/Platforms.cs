using UnityEngine;

public class Platforms : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("Altura mínima en unidades del mundo")]
    [SerializeField] private float minHeight = 0f;

    [Tooltip("Altura máxima en unidades del mundo")]
    [SerializeField] private float maxHeight = 5f;

    [Tooltip("Velocidad de movimiento en unidades por segundo")]
    [SerializeField] private float speed = 2f;

    private bool movingUp = true;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void FixedUpdate() 
    {
        float currentHeight = transform.position.y;
        Vector3 movement = movingUp ? Vector3.up : Vector3.down;
        movement *= speed * Time.deltaTime;

        transform.Translate(movement);

        // Limitar altura
        float maxY = initialPosition.y + maxHeight;
        float minY = initialPosition.y + minHeight;

        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            movingUp = false;
        }
        else if (transform.position.y < minY)
        {
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
            movingUp = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 minPoint = new Vector3(transform.position.x, transform.position.y + minHeight, transform.position.z);
        Vector3 maxPoint = new Vector3(transform.position.x, transform.position.y + maxHeight, transform.position.z);

        Gizmos.DrawLine(minPoint, maxPoint);
        Gizmos.DrawWireSphere(minPoint, 0.2f);
        Gizmos.DrawWireSphere(maxPoint, 0.2f);
    }

    void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}

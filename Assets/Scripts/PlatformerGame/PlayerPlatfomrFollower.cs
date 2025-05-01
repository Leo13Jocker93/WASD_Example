using UnityEngine;

public class PlayerPlatformFollower : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    private Vector3 platformVelocity;
    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;
    private PlayerController playerController;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (currentPlatform != null)
        {
            platformVelocity = (currentPlatform.position - lastPlatformPosition) / Time.deltaTime;

            // Ignorar pequeñas vibraciones
            if (platformVelocity.magnitude < 0.001f)
                platformVelocity = Vector3.zero;

            lastPlatformPosition = currentPlatform.position;
        }
        else
        {
            platformVelocity = Vector3.zero;
        }

        playerController.SetExternalVelocity(platformVelocity);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("MovingPlatform") && Vector3.Dot(hit.normal, Vector3.up) > 0.5f)
        {
            currentPlatform = hit.collider.transform;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    void LateUpdate()
    {
        if (currentPlatform != null)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
            if (!Physics.Raycast(ray, out RaycastHit hit, 1.0f) || hit.collider.transform != currentPlatform)
            {
                currentPlatform = null;
                platformVelocity = Vector3.zero;
            }
        }
    }
}

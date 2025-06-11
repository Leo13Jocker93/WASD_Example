using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatiorStates : MonoBehaviour
{
    [SerializeField] private PlayerController playerController; // Asignar desde Inspector
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController no asignado en PlatformerPlayerAnimator");
        }
    }

    private void Update()
    {
        if (playerController == null) return;

        float speed = new Vector2(playerController.CurrentInput.x, playerController.CurrentInput.y).magnitude;

        animator.SetFloat("Speed", speed);

        animator.SetBool("isJumping", !playerController.IsGrounded);
    }
}
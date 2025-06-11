using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
    private Animator animator;
    private int clickCount = 0;
    private int currentAttackIndex = 1;
    private float comboTimer = 0f;
    private float combatModeTimer = 0f;
    private bool isAttacking = false;
    private bool isInCombatMode = false;

    [Header("Configuración de combate")]
    public float comboWindow = 0.5f;
    public float combatModeDuration = 20f;
    public float attackDuration = 0.4f;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;

            if (!isInCombatMode)
            {
                EnterCombatMode();
            }

            if (!isAttacking)
            {
                StartAttack();
            }

            Debug.Log("Clic registrado, clickCount: " + clickCount);
        }

        if (clickCount > 0 && !isAttacking)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
                Debug.Log("Combo reiniciado por timeout.");
            }
        }

        if (isInCombatMode)
        {
            combatModeTimer -= Time.deltaTime;
            if (combatModeTimer <= 0f && !isAttacking)
            {
                ExitCombatMode();
            }
        }
    }

    void StartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        string trigger = "Attack" + currentAttackIndex;
        animator.SetTrigger(trigger);

        Debug.Log("Iniciando ataque: " + trigger + ", clickCount: " + clickCount);

        currentAttackIndex++;
        if (currentAttackIndex > 3)
        {
            currentAttackIndex = 1;
        }

        comboTimer = comboWindow;
        combatModeTimer = combatModeDuration;

        StartCoroutine(SimulateAttackEnd());
    }

    IEnumerator SimulateAttackEnd()
    {
        yield return new WaitForSeconds(attackDuration);
        OnAttackAnimationEnd();
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;

        if (clickCount > 0)
        {
            clickCount--;

            if (clickCount > 0)
            {
                StartAttack();
            }
            else
            {
                ResetCombo();
            }
        }
        else
        {
            ResetCombo();
        }
    }

    void ResetCombo()
    {
        clickCount = 0;
        currentAttackIndex = 1;
        isAttacking = false;
        animator.SetTrigger("ResetToCombatIdle");
        Debug.Log("Combo reiniciado, regreso a combat idle.");
    }

    void EnterCombatMode()
    {
        isInCombatMode = true;
        combatModeTimer = combatModeDuration;

        if (animator != null)
        {
            StartCoroutine(TransitionLayerWeight(1, 1f, 0.3f)); // Transición suave al modo combate
        }

        Debug.Log("Entrando en modo combate.");
    }

    void ExitCombatMode()
    {
        isInCombatMode = false;

        if (animator != null)
        {
            StartCoroutine(TransitionLayerWeight(1, 0f, 0.3f)); // Transición suave al modo normal
        }

        Debug.Log("Saliendo del modo combate.");
    }

    public bool IsInCombatMode()
    {
        return isInCombatMode;
    }

    IEnumerator TransitionLayerWeight(int layerIndex, float targetWeight, float duration)
    {
        float startWeight = animator.GetLayerWeight(layerIndex);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newWeight = Mathf.Lerp(startWeight, targetWeight, elapsed / duration);
            animator.SetLayerWeight(layerIndex, newWeight);
            yield return null;
        }

        animator.SetLayerWeight(layerIndex, targetWeight);
    }
}
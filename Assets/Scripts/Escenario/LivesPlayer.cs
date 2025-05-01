using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LivesPlayer : MonoBehaviour
{
    public int maxLives = 3;
    private int currentLives;

    public Transform respawnPoint;
    public TextMeshProUGUI livesText;

    void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();
        if (respawnPoint == null)
            respawnPoint = this.transform;
    }

    void Update()
    {
        UpdateLivesUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            currentLives--;
            Debug.Log("Vidas restantes: " + currentLives);

            if (currentLives <= -1)
            {
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                transform.position = respawnPoint.position;
            }
        }
    }

    void UpdateLivesUI()
    {
        if(livesText!= null)
        {
            livesText.text = $"{currentLives} / {maxLives}";
        }
    }
}
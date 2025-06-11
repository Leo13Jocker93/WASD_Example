using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_HolaMundo : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(() => ChangeTextHM ("¡¡Hola Mundo!!"));
        }
        
    }

    public void ChangeTextHM (string text)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }
    }
}

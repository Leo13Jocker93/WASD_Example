using UnityEngine;

public class Scale : MonoBehaviour
{
    
    [SerializeField] public UI_HolaMundo holaMundoScript;
    [SerializeField] public Transform ObjectToScale;
    [SerializeField] private float ScaleSpeed = 0.1f;
    [SerializeField] private float minScale = -0.005f;

    private string ScaleText = "Escalado a: ";
    private string WarningText = "No se puede hacer más pequeño";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        holaMundoScript.ChangeTextHM("Empty");
    }

    public void ScaleUp()
    {
        ObjectToScale.localScale += Vector3.one * ScaleSpeed;
        holaMundoScript.ChangeTextHM(ScaleText + ObjectToScale.localScale.x.ToString("F2"));
        //con la linea de arriba mostramos el valor de escala con dos cifras decimales (F2)
    }
    public void ScaleDown()
    {
        if (ObjectToScale.localScale.x > minScale)
        {
            ObjectToScale.localScale -= Vector3.one * ScaleSpeed;
            holaMundoScript.ChangeTextHM(ScaleText + ObjectToScale.localScale.x.ToString("F2"));
        }
        else
        {
            holaMundoScript.ChangeTextHM(WarningText);
        }
    }
}

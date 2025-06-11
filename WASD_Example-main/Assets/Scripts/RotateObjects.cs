using UnityEngine;

public class RotateObjects : MonoBehaviour
{
    [SerializeField] private Transform objectToRotate;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] public UI_HolaMundo holaMundoScript;

    private bool isRotating = false;

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            float direction = clockwise ? 1f : -1f; //Definimos el sentido de la rotación
            objectToRotate.Rotate(Vector3.up * rotationSpeed * direction * Time.deltaTime);
            Debug.Log("Rotando" + objectToRotate.name);
        }
        
    }

    public void ToggleRotation()
    {
        isRotating = !isRotating; //Activar y desactivar la rotación es como un switch
        if (holaMundoScript != null)
        {
            if (isRotating)
            {
                holaMundoScript.ChangeTextHM("Rotando Objeto");
            }
            else
            {
                holaMundoScript.ChangeTextHM("Rotación Detenida");
            }
        }
    }
}

using UnityEngine;

public class RotateObject : MonoBehaviour
{
    
    public Transform objectToRotate;
    public float rotationSpeed = 100f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //float direction = clockwise ? 1f : -1f; //Definimos el sentido de la rotación
        objectToRotate.Rotate(Vector3.up * rotationSpeed * 1f * Time.deltaTime);
        Debug.Log("Rotando" + objectToRotate.name);
        
    }
}

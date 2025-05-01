using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ColletibleMangerS : MonoBehaviour
{
    [Header("SecuenciaMovimiento")]
    public float amplitud = 0.25f;
    public float speed = 2f;
    public float rotationSpeed = 45f;

    [Header("SistemaRecoleccion")]
    public TextMeshProUGUI itemCounter;
    public int totalItemsScene = 2;
    public string collectibleTad = "Collecible";
    private static int itemsCollected = 0;

    [Header("Lista de objetos")]
    public List<Transform>collectibles = new List<Transform>();
    private Dictionary<Transform, Vector3> startposition = new Dictionary<Transform, Vector3>();    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(var obj in collectibles)
        {
            if(obj!=null)
            {
                startposition[obj] = obj.position;

                Collider col = obj.GetComponent<Collider>();
                if(col==null)
                    col=obj.gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;

                if(obj.GetComponent<PlayerCollecitbleDetector>() == null)
                obj.gameObject.AddComponent<PlayerCollecitbleDetector>().Init(this);
            }
        }

        UpdatedCounterUI();
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var obj in collectibles)
        {
            if(obj == null) continue;
            Vector3 StartPos = startposition[obj];
            float newY = StartPos.y + Mathf.Sin(Time.deltaTime * speed)*amplitud;
            obj.position = new Vector3(StartPos.x, newY, StartPos.z);
            obj.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }        
    }

    public void Collect(Transform obj)
    {
        if(!collectibles.Contains(obj)) return;

        collectibles.Remove(obj);
        itemsCollected++;
        UpdatedCounterUI();
        Destroy(obj.gameObject);
    }
    void UpdatedCounterUI()
    {
        if(itemCounter != null)
        {
            itemCounter.text = $"{itemsCollected} / {totalItemsScene}";
        }
    }
}

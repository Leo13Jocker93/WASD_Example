using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class CollectiblesItems : MonoBehaviour
{
    [Header("Movimiento")]
    public float floatAmplitude = 0.25f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 45f;

    [Header("Recolección")]
    public TextMeshProUGUI itemCounterText;
    public int totalItemsInScene = 5;
    public string collectibleTag = "Collectible";

    private static int itemsCollected = 0;

    [Header("Lista de objetos")]
    public List<Transform> collectibles = new List<Transform>();
    private Dictionary<Transform, Vector3> startPositions = new Dictionary<Transform, Vector3>();

    void Start()
    {
        foreach (var obj in collectibles)
        {
            if (obj != null)
            {
                startPositions[obj] = obj.position;

                // Agrega collider y trigger si no lo tienen
                Collider col = obj.GetComponent<Collider>();
                if (col == null)
                    col = obj.gameObject.AddComponent<BoxCollider>();

                col.isTrigger = true;

                // Agrega un componente de detección
                if (obj.GetComponent<CollectibleDetector>() == null)
                    obj.gameObject.AddComponent<CollectibleDetector>().Init(this);
            }
        }

        UpdateCounterUI();
    }

    void Update()
    {
        foreach (var obj in collectibles)
        {
            if (obj == null) continue;

            Vector3 startPos = startPositions[obj];
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            obj.position = new Vector3(startPos.x, newY, startPos.z);
            obj.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void Collect(Transform obj)
    {
        if (!collectibles.Contains(obj)) return;

        collectibles.Remove(obj);
        itemsCollected++;
        UpdateCounterUI();
        Destroy(obj.gameObject);
    }

    void UpdateCounterUI()
    {
        if (itemCounterText != null)
        {
            itemCounterText.text = $"{itemsCollected} / {totalItemsInScene}";
        }
    }
}
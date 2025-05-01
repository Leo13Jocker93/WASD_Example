using UnityEngine;

public class CollectibleDetector : MonoBehaviour
{
    private CollectiblesItems manager;

    public void Init(CollectiblesItems manager)
    {
        this.manager = manager;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.Collect(transform);
        }
    }
}
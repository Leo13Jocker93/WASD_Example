using UnityEngine;

public class PlayerCollecitbleDetector : MonoBehaviour
{
    
    private ColletibleMangerS  manager;
    public void Init(ColletibleMangerS  manager)
    {
        this.manager = manager;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            manager.Collect(transform);
        }
    }
}

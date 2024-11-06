using UnityEngine;

public class ObjectSuicide : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime;

    private void Update()
    {
        lifeTime -= Time.deltaTime; ;
        if (lifeTime <= 0) Destroy(gameObject);
    }
}

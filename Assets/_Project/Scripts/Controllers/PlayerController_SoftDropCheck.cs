using UnityEngine;
using UnityEngine.Events;

public class PlayerController_SoftDropCheck : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoxCollider2D playerCol;

    [Header("Events")]
    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent OnExit;

    [Header("Debug")]
    [SerializeField] private string onewayTag;
    [SerializeField] private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        onewayTag = GetComponentInParent<PlayerController>().onewayTag;
    }

    private void Update()
    {
        col.size = playerCol.size;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(onewayTag))
        {
            OnEnter.Invoke();
            Debug.Log("Entered!");
        }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(onewayTag))
        {
            OnExit.Invoke();
            Debug.Log("Exited!");
        }
    }
}
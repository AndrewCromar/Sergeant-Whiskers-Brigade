using UnityEngine;
using UnityEngine.Events;

public class PlayerController_SoftDropCheck : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent OnExit;

    [Header("Debug")]
    [SerializeField] private string onewayTag;

    private void Start()
    {
        onewayTag = GetComponentInParent<PlayerController>().onewayTag;
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
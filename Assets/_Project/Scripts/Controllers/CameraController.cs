using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [HideInInspector] public static CameraController instance;

    [SerializeField] private List<GameObject> targets = new List<GameObject>();
    
    [SerializeField] private float trackSpeed = 5f;
    [SerializeField] private bool trackX, trackY;

    void Start() {
        instance = this;
    }

    private void Update(){
        if(targets.Count <= 0) return;

        Vector2 targetPosition = Vector2.zero;
        foreach(GameObject target in targets){
            targetPosition += (Vector2)target.transform.position;
        }
        targetPosition = targetPosition / targets.Count;

        transform.position = Vector2.Lerp(transform.position, new Vector2((trackX ? targetPosition.x : 0), (trackY ? targetPosition.y : 0)), trackSpeed * Time.deltaTime);
    }


    public void AddTarget(GameObject newTarget){ if (!targets.Contains(newTarget)) targets.Add(newTarget); }

    public void RemoveTarget(GameObject removeTarget){ if (targets.Contains(removeTarget)) targets.Remove(removeTarget); }
}

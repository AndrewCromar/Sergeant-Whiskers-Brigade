using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectRepeater : MonoBehaviour {
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    [SerializeField] private float objectWidth;
    [SerializeField] private int numObjects;

    public void RunRepeater() {
        foreach(GameObject obj in objects) {
            DestroyImmediate(obj);
        }
        objects.Clear();

        for(int x = 0; x < numObjects; x++) {
            GameObject newObject = Instantiate(objectPrefab, transform.position + new Vector3(x * objectWidth, 0, 0), Quaternion.identity, transform);
            objects.Add(newObject);
        }
    }
}
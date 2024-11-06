using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectRepeater))]
public class ObjectRepeaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectRepeater repeater = (ObjectRepeater)target;
        if (GUILayout.Button("Run Repeater"))
        {
            repeater.RunRepeater();
        }

        DrawDefaultInspector();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableJumpHeightToggleController : MonoBehaviour
{
    [HideInInspector] public static VariableJumpHeightToggleController instance;

    [Header("Debug")]
    [SerializeField] public bool variableJumpHeight = true;

    private void Awake()
    {
        instance = this;
    }
    
    public void ToggleVariableJumpHeight()
    {
        variableJumpHeight = !variableJumpHeight;
    }
}

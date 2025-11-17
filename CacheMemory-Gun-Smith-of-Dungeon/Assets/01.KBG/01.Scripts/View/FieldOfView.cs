using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)] public float fov = 90f;
    public float viewDistance = 10f;
    public LayerMask layerMask;
    
    
    private void 
}
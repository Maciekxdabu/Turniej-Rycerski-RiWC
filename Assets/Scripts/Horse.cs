using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Horse", menuName = "RIWC/Horse")]
public class Horse : ScriptableObject
{
    [field: SerializeField]
    public float acceleration { get; private set; }
    [field: SerializeField]
    public float maxSpeed { get; private set; }
    [field: SerializeField]
    public float strength { get; private set; }
}

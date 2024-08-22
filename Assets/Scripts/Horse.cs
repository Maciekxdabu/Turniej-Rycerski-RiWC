using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Horse", menuName = "RIWC/Horse")]
public class Horse : ScriptableObject
{
    [field: SerializeField]
    public float acceleration { get; set; }
    [field: SerializeField]
    public float maxSpeed { get; set; }
    [field: SerializeField]
    public float minDamagingSpeed { get; set; }
    [field: SerializeField]
    public float strength { get; set; }

    //!!!
    //WARNING: CHANGE HORSE SETTERS TO PRIVATE WHEN DELETING DEBUG SETUP SCRIPT
    //!!!
}

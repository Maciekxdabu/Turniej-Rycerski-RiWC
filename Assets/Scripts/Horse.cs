using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Horse", menuName = "RIWC/Horse")]
public class Horse : ScriptableObject
{
    [field: SerializeField, Header("Game Data")]
    public float acceleration { get; set; }
    [field: SerializeField]
    public float maxSpeed { get; set; }
    [field: SerializeField]
    public float minDamagingSpeed { get; set; }
    [field: SerializeField]
    public float strength { get; set; }

    [field: SerializeField, Header("Graphical Data")]
    public Sprite previewGraphic { get; set; }

    //!!!
    //WARNING: CHANGE HORSE SETTERS TO PRIVATE WHEN DELETING DEBUG SETUP SCRIPT
    //!!!
}

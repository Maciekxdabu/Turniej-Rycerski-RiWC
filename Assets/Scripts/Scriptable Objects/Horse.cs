using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

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
    public SpriteLibraryAsset spriteLibrary { get; set; }

    //!!!
    //WARNING: CHANGE HORSE SETTERS TO PRIVATE WHEN DELETING DEBUG SETUP SCRIPT
    //!!!
}

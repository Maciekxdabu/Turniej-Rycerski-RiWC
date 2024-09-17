using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Knight", menuName = "RIWC/Knight")]
public class Knight : ScriptableObject
{
    [field: SerializeField, Header("Game Data")]
    public string knightName { get; set; }

    //[field: SerializeField, Header("Graphical Data")]

    [field: SerializeField, Header("Commentator audio clips")]
    public AudioClip nameClip { get; set; }
    [field: SerializeField]
    public AudioClip loseCommentClip { get; set; }
    [field: SerializeField]
    public AudioClip wonoCommentClip { get; set; }
}

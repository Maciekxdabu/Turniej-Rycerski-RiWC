using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [System.Serializable]
    public class PlayerEntry
    {
        public Transform mainObject;
        public Transform icon;
    }

    // ---------- Variables

    [SerializeField] PlayerEntry[] players = null;
    [SerializeField] Transform[] lines = null;
    [Space]
    [SerializeField] Transform mapLowerLeftCorner = null;
    [SerializeField] Transform mapUpperRightCorner = null;
    [SerializeField] Transform miniLowerLeftCorner = null;
    [SerializeField] Transform miniUpperRightCorner = null;

    private float width { get { return mapUpperRightCorner.position.x - mapLowerLeftCorner.position.x; } }
    private float height { get { return mapUpperRightCorner.position.y - mapLowerLeftCorner.position.y; } }
    private float miniWidth { get { return miniUpperRightCorner.position.x - miniLowerLeftCorner.position.x; } }
    private float miniHeight { get { return miniUpperRightCorner.position.y - miniLowerLeftCorner.position.y; } }

    private Vector2 tempPos = Vector2.zero;

    // ---------- Unity methods

    private void Update()
    {
        foreach (PlayerEntry player in players)
        {
            // from map to procent
            tempPos.x = (player.mainObject.position.x - mapLowerLeftCorner.position.x) / width;
            tempPos.y = (player.mainObject.position.y - mapLowerLeftCorner.position.y) / height;
            // from procent to minimap
            tempPos.x = tempPos.x * miniWidth + miniLowerLeftCorner.position.x;
            tempPos.y = tempPos.y * miniHeight + miniLowerLeftCorner.position.y;

            player.icon.position = tempPos;
        }
    }
}

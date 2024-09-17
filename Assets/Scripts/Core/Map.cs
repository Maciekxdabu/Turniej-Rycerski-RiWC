using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [System.Serializable]
    private class BetweenLine
    {
        public Vector2[] passages;
    }

    [System.Serializable]
    public class MapPosition
    {
        [Tooltip("Line number (index)")]
        public int line;
        [Tooltip("Map position (from 0 to 1)")]
        public float position;
        [Tooltip("Player orientation upon warp")]
        public bool right = true;
    }

    // ---------- Variables

    [Tooltip("Must be given from up to down")]
    [SerializeField] Transform[] lines;
    [SerializeField] BetweenLine[] betweenLines;
    [SerializeField] Transform startPosition;
    [SerializeField] Transform endPosition;
    [SerializeField] Transform cameraYPosition;
    [Space]
    [SerializeField] List<Player> players = null;

    [SerializeField, HideInInspector] private float startX, arenaLen;

    //singleton
    private static Map instance = null;

    // ---------- Unity methods

    private void Awake()
    {
        instance = this;

        startX = startPosition.position.x;
        arenaLen = endPosition.position.x - startPosition.position.x;
    }

    private void Update()
    {
        int line;
        float position;
        foreach (Player player in players)
        {
            (line, position) = player.GetPositions();

            Vector2 pos = Vector2.zero;
            if (line >= 0 && line < lines.Length)
                pos.y = lines[line].position.y;
            pos.x = startX + position * arenaLen;

            player.transform.position = pos;
        }
    }

    private void OnValidate()
    {
        startX = startPosition.position.x;
        arenaLen = endPosition.position.x - startPosition.position.x;
    }

    private void OnDrawGizmos()
    {
        if (betweenLines.Length >= lines.Length)
            return;

        for (int i=0; i< betweenLines.Length; i++)
        {
            float y = (lines[i].position.y - lines[i + 1].position.y)/2 + lines[i + 1].position.y;
            Vector3 startPos = new Vector3(0, y, 0);
            Vector3 endPos = new Vector3(0, y, 0);

            foreach (Vector2 passage in betweenLines[i].passages)
            {
                startPos.x = startX + passage.x * arenaLen;
                endPos.x = startX + passage.y * arenaLen;

                Gizmos.DrawLine(startPos, endPos);
            }
        }
    }

    // ---------- private methods

    private bool _CanMoveLane(int currentLane, float currentPosition, bool up = true)
    {
        //check to which lane Player wants to move (and if that lane exists)
        int betweenLineIndex = -1;
        if (up)
        {
            if (currentLane <= 0)
                return false;
            betweenLineIndex = currentLane - 1;
        }
        else
        {
            if (currentLane >= lines.Length - 1)
                return false;
            betweenLineIndex = currentLane;
        }

        //Check if there is a passage in the current Player position
        foreach (Vector2 passage in betweenLines[betweenLineIndex].passages)
        {
            if (passage.x <= currentPosition && currentPosition <= passage.y)
                return true;
        }

        return false;
    }

    private void _AddPlayer(Player player)
    {
        players.Add(player);
    }

    // ---------- public static wrappers

    public static bool CanChangeLine(int currentLane, float currentPosition, bool up = true)
    {
        return instance._CanMoveLane(currentLane, currentPosition, up);
    }

    public static void AddPlayer(Player player)
    {
        instance._AddPlayer(player);
    }

    public static float GetCameraYPosition()
    {
        return instance.cameraYPosition.position.y;
    }
}

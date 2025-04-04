using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [System.Serializable]
    public class BetweenLine
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

    [SerializeField, HideInInspector] private float startX, arenaLen;

    //singleton
    private static MapController instance = null;
    public static MapController Instance { get { return instance; } }

    // ---------- Unity methods

    private void Awake()
    {
        instance = this;

        startX = startPosition.position.x;
        arenaLen = endPosition.position.x - startPosition.position.x;
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
        currentPosition = UnitToLen(currentPosition);

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

    private Vector2 _OnMove(PlayerBrain player, float position, int line)
    {
        //check if line is correct
        Debug.Assert(line >= 0 && line < lines.Length, "ERR: Incorrect Player line", player.gameObject);

        //translate movement to map units
        position = UnitToLen(position);

        //check if out of bounds
        if (position < 0)
        {
            player.OnOutOfBounds(LenToUnit(0));
        }
        else if (position > 1)
        {
            player.OnOutOfBounds(LenToUnit(1));
        }

        //return applied position
        return new Vector2(LenToUnit(position), lines[line].position.y);
    }

    // ---------- public static wrappers

    public static bool CanChangeLine(int currentLane, float currentPosition, bool up = true)
    {
        return instance._CanMoveLane(currentLane, currentPosition, up);
    }

    //method called from Player on each move attempt
    public static Vector2 OnMove(PlayerBrain player, float position, int line)
    {
        return instance._OnMove(player, position, line);
    }

    public static float GetCameraYPosition()
    {
        return instance.cameraYPosition.position.y;
    }

    // ---------- public methods

    /// <summary>
    /// Transforms Unity units to arena length
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public float UnitToLen(float unit)
    {
        return (unit - startX) / arenaLen;
    }

    /// <summary>
    /// Transforms arena length to Unity units
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    public float LenToUnit(float len)
    {
        return startX + len * arenaLen;
    }
}

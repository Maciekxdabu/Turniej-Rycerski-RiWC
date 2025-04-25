using UnityEngine;

/// <summary>
/// A ScriptableObject class responsible for defining the gameplay map structure (normalized values)
/// </summary>
[CreateAssetMenu(fileName = "Map", menuName = "RIWC/Map")]
public class MapDefinition : ScriptableObject
{
    [System.Serializable]
    public class BetweenLine
    {
        public Vector2[] passages;
    }

    //values
    public int linesAmount = 3;
    [Tooltip("Must be the length of (linesAmount - 1)")]
    public BetweenLine[] betweenLines = new BetweenLine[2];

    // ---------- public methods

    /// <summary>
    /// Can the Player Switch lanes under the circumstances?
    /// </summary>
    /// <param name="currentLane">as in index (from 0)</param>
    /// <param name="currentPosition">must be normalized</param>
    /// <param name="up">up or down movement</param>
    /// <returns></returns>
    public bool CanChangeLine(int currentLane, float currentPosition, bool up = true)
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
            if (currentLane >= linesAmount - 1)
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
}

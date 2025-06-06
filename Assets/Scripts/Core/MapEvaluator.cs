using UnityEngine;

/// <summary>
/// A class responsible for converting the Player in-game position to the in-world position <br></br>
/// Can also edit the map definition using the custom Editor tool
/// </summary>
public class MapEvaluator : MonoBehaviour
{
    public enum PositionState
    {
        Normal,
        OutOfBoundsLeft,
        OutOfBoundsRight,
        Invalid
    }

    //TODO - mapData in Evaluator should be internal and in namespace (so GameManager can set it in-Editor)
    [SerializeField] public MapDefinition mapData;
    [Header("Scene Map Data")]
    [Tooltip("Must be given from up to down")]
    [SerializeField] Transform[] lines;
    [SerializeField] Transform startPosition;
    [SerializeField] Transform endPosition;

    //Note: Problem with scaling when placed on a Canvas (RectTransform) - Awake() returns incorrect values
    //[SerializeField, ReadOnly] private float startX, arenaLen;
    private float startX { get { return startPosition != null ? startPosition.position.x : 0f; } }
    private float arenaLen { get { return startPosition != null && endPosition != null ? endPosition.position.x - startPosition.position.x : 1f; } }

    // ---------- Unity messages

    private void Start()
    {
        //startX = startPosition.position.x;
        //arenaLen = endPosition.position.x - startPosition.position.x;
    }

    private void OnValidate()
    {
        //chack if amount of lanes is valid
        if (mapData != null && mapData.linesAmount != lines.Length)
            Debug.LogWarning("WAR: The amount of lanes in Evaluator does not match the amount of lanes in the definition!");
    }

    private void OnDrawGizmos()
    {
        if (mapData.betweenLines.Length >= lines.Length)
            return;

        for (int i = 0; i < mapData.betweenLines.Length; i++)
        {
            float y = GetBetweenLineHeight(i);
            Vector3 startPos = new Vector3(0, y, 0);
            Vector3 endPos = new Vector3(0, y, 0);

            foreach (Vector2 passage in mapData.betweenLines[i].passages)
            {
                startPos.x = startX + passage.x * arenaLen;
                endPos.x = startX + passage.y * arenaLen;

                Gizmos.DrawLine(startPos, endPos);
            }
        }
    }

    // ---------- public methods

    /// <summary>
    /// Translates the mapRepresentation Transform and returns the PositionState
    /// </summary>
    /// <param name="mapRepresentation">object to move on the Map</param>
    /// <param name="normalizedPosition">normalized Player in Map position</param>
    /// <param name="line">lane index</param>
    /// <returns>PositionState = normal, outOfBounds, Invalid, etc.</returns>
    public PositionState OnMove(Transform mapRepresentation, float normalizedPosition, int line)
    {
        //check if line is valid
        if (line < 0 || line >= lines.Length)
            return PositionState.Invalid;

        //Translate map representation
        mapRepresentation.transform.position = new Vector2(NormToUnit(normalizedPosition), lines[line].position.y);

        //check if out of bounds
        if (normalizedPosition < 0)
            return PositionState.OutOfBoundsLeft;
        else if (normalizedPosition > 1)
            return PositionState.OutOfBoundsRight;

        //return Normal PositionState
        return PositionState.Normal;
    }

    // ---------- public conversion methods

    /// <summary>
    /// Transforms Unity units to normalized arena length (based on the current Evaluator, so markers positions)
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public float UnitToNorm(float unit)
    {
        return (unit - startX) / arenaLen;
    }

    /// <summary>
    /// Transforms Unity units to normalized arena length (not based on Evaluator, just pure length)
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public float UnitToNormLength(float unit)
    {
        return unit / arenaLen;
    }

    /// <summary>
    /// Transforms normalized arena length to Unity units
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    public float NormToUnit(float len)
    {
        return startX + len * arenaLen;
    }

    public float GetBetweenLineHeight(int index)
    {
        if (index < 0 || index >= mapData.betweenLines.Length)
            return -1f;

        return (lines[index].position.y - lines[index + 1].position.y) / 2 + lines[index + 1].position.y;
    }
}

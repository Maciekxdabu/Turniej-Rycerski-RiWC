using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [System.Serializable]
    public class PlayerEntry
    {
        public Player player;
        public int line;
    }

    // ---------- Variables

    private static Map instance = null;

    [Tooltip("Must be given from up to down")]
    [SerializeField] Transform[] lines;
    [Space]
    [SerializeField] List<PlayerEntry> players = null;

    // ---------- Unity methods

    private void Awake()
    {
        instance = this;
    }

    // ---------- private methods

    private void _ChangeLine(Player player, bool up)
    {
        PlayerEntry entry = players.Find(x => x.player == player);
        if (entry == null)
        {
            Debug.LogWarning("WAR: Player could not be found on the map", gameObject);
            return;
        }

        Vector3 newPos = player.transform.position;

        if (up && entry.line > 0)//wants to move up
        {
            entry.line--;
        }
        else if(!up && entry.line < lines.Length - 1)//wants to move down
        {
            entry.line++;
        }

        newPos.y = lines[entry.line].position.y;
        player.transform.position = newPos;
    }

    // ---------- public static wrappers

    public static void ChangeLine(Player player, bool up = true)
    {
        instance._ChangeLine(player, up);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Setups the joined players for the coop gameplay
/// </summary>
public class CoopSetup : MonoBehaviour
{
    private class PlayerData
    {
        public PlayerSetup setup;
        public bool ready = false;
    }

    [SerializeField] private PlayerData[] players;

    // ---------- pulic methods

    public void AddPlayer(Player player)
    {

    }

    public void RemovePlayer(Player player)
    {

    }

    // ---------- public methods (used by UnityEvent's)

    public void OnReady(PlayerSetup player, bool ready_state)
    {

    }
}

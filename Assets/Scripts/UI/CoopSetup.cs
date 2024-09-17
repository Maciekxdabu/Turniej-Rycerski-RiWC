using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Setups the joined players for the coop gameplay
/// Basically let's each Player choose their game setup
/// </summary>
public class CoopSetup : MonoBehaviour
{
    [SerializeField] private List<PlayerSetup> players = new List<PlayerSetup>();

    // ---------- pulic methods

    public void AddPlayer(PlayerSetup player)
    {
        player.Init(this);
        players.Add(player);
    }

    // ---------- public methods (used by UnityEvent's)

    public void OnReady(PlayerSetup player)
    {
        if (players.TrueForAll(x => x.IsPlayerReady()))
            OnSetupFinish();
    }

    // ---------- private methods

    private void OnSetupFinish()
    {
        foreach (PlayerSetup p in players)
            p.DisableSetup();

        GameManager.Instance.StartGame();
    }
}

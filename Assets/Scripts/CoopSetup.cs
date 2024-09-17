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

    //event for informing gameplay when the coop setup is ready
    private UnityEvent onSetupFinishEvent = new UnityEvent();

    // ---------- pulic methods

    public void AddPlayer(PlayerSetup player)
    {
        player.Init(this);
        players.Add(player);
    }

    public void Init()
    {

    }

    // ---------- public methods (used by UnityEvent's)

    public void OnReady(PlayerSetup player)
    {
        if (players.TrueForAll(x => x.IsPlayerReady()))
        {
            foreach (PlayerSetup p in players)
                p.DisableSetup();

            GameManager.Instance.StartGameBtn();
        }
    }

    // ---------- private methods

    private void OnSetupFinish()
    {
        //TODO

        //inform about coop setup finishing
        onSetupFinishEvent.Invoke();
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Class responsible for the joining behaviour of Players in the Local Lobby
/// </summary>
[RequireComponent(typeof(PlayerInputManager))]
public class LocalLobbyManager : MonoBehaviour
{
    [SerializeField] private string gameplayScene;
    [SerializeField] private List<LocalLobbyPlayer> playerSetups;

    // ---------- Unity messages

    private void Awake()
    {
        //Initialize Lobby Players
        foreach (LocalLobbyPlayer lobbyPlayer in playerSetups)
        {
            lobbyPlayer.Init(this);
        }

        //Add Players back from Gameplay (if they exist)
        for (int i=0; i<PlayerInput.all.Count; i++)
        {
            PlayerInput player = PlayerInput.all[i];
            playerSetups[player.playerIndex].OnPlayerJoined(player.GetComponent<PlayerInputController>());
        }
    }

    // ---------- Player Input Manager methods

    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        playerSetups[newPlayer.playerIndex].OnPlayerJoined(newPlayer.GetComponent<PlayerInputController>());
    }

    public void OnPlayerLeft(PlayerInput newPlayer)
    {
        playerSetups[newPlayer.playerIndex].OnPlayerLeft();
    }

    // ---------- publicmethods

    public void OnReady(LocalLobbyPlayer player)
    {
        if (playerSetups.TrueForAll(x => x.IsPlayerReady()))
            OnGameStart();
    }

    // ---------- private methods

    private void OnGameStart()
    {
        //save which Lobby was on
        GameManager.lobbyScenebuildIndex = SceneManager.GetActiveScene().buildIndex;
        //TODO - Later store actual Lobby data in an abstract Lobby class
        //       unless this data can be reconstructed from just the Players preset, but probably not...

        //sort PlayerBrain's singleton List by playerIndex

        //send to gameplay scene
        SceneManager.LoadScene(gameplayScene);
    }
}

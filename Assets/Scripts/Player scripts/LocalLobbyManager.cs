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

    private PlayerInputManager playerInputManager;

    // ---------- Unity messages

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        foreach (LocalLobbyPlayer lobbyPlayer in playerSetups)
        {
            lobbyPlayer.Init(this);
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
        //sort PlayerBrain's singleton List by playerIndex

        //send to gameplay scene
        SceneManager.LoadScene(gameplayScene);
    }
}

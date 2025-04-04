using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the gameplay
/// Joins registered players and sends them to the Setup
/// After setup ends, it initializes the gameplay
/// </summary>
[RequireComponent(typeof(PlayerInputManager))]
public class GameManager : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] private bool DEBUG_JOINING = false;
    [Header("Values")]
    [SerializeField] private MapController.MapPosition[] spawnPositions;
    [SerializeField] private CoopSetup coopSetup;
    [SerializeField] private string menuSceneName;

    private PlayerInputManager playerInputManager;

    private List<PlayerBrain> activePlayers = new List<PlayerBrain>();

    //singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // ---------- Unity messages

    private void Awake()
    {
        _instance = this;

        //get needed references
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        playerInputManager.EnableJoining();

        if (PlayerManager.PlayerDataList.Count > 0)
        {
            for (int i = 0; i < PlayerManager.PlayerDataList.Count; i++)
            {
                PlayerManager.PlayerData playerData = PlayerManager.PlayerDataList[i];
                //playerInputManager.JoinPlayer(playerData.playerIndex, i, playerData.controlScheme, playerData.devices);
                playerInputManager.JoinPlayer(controlScheme: playerData.controlScheme, pairWithDevices: playerData.devices);
            }
        }
        else if (DEBUG_JOINING)
        {
            PlayerInput player;
            do
            {
                player = playerInputManager.JoinPlayer();
            }
            while (player != null);
        }

        playerInputManager.DisableJoining();
    }

    // ---------- Player Input Manager messages

    //should make sure the player joined correctly (e.g. correct prefab) (only configurations which need to happen only once)
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerInput.devices.Count == 0)
            return;
        Debug.Log("New player joined in gameplay");

        playerInput.DeactivateInput();
        PlayerBrain player = playerInput.GetComponent<PlayerBrain>();
        if (player == null)
        {
            Debug.LogWarning("ERR: Player attempted to spawn/join without a Player Component... Removing...", gameObject);
            Destroy(playerInput.gameObject);
            return;
        }

        //Init Player
        StartCoroutine(player.Init(coopSetup, playerInput.splitScreenIndex));

        return;
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogWarning("WAR: Player left during gameplay, this should not happen", gameObject);
    }

    // ---------- public methods (UI Buttons)

    public void StartGame()
    {
        if (PlayerInput.all.Count <= 0)
        {
            Debug.LogWarning("WAR: There is not enough Players to start the game, at least 1 Player is required", gameObject);
            return;
        }

        Debug.Log("===Started the game!");

        //setup players
        activePlayers.Clear();
        for (int i = 0; i < PlayerInput.all.Count; i++)
        {
            if (PlayerInput.all[i].TryGetComponent<PlayerBrain>(out PlayerBrain player))
            {
                activePlayers.Add(player);
                player.OnStartGame(spawnPositions[i]);
            }
        }

        Minimap.Instance.ShowMinimap(true);
    }

    // ---------- public methods

    public void OnPlayerDeath(PlayerBrain deadPlayer)
    {
        //remove player from alive list
        activePlayers.Remove(deadPlayer);

        //check if there is a single winner present
        if (activePlayers.Count <= 1)
        {
            OnGameFinished();
        }
    }

    // ---------- private methods

    private void OnGameFinished()
    {
        // --- after game finishes
        //disable all players inputs (not only the winning one, because the match could end in different ways)
        for (int i = 0; i < PlayerInput.all.Count; i++)
        {
            if (PlayerInput.all[i].TryGetComponent<PlayerBrain>(out PlayerBrain player))
            {
                player.ManualPlayerDisable();
            }
        }

        //display score/result
        //TODO

        //load Main Menu Scene
        SceneManager.LoadScene(menuSceneName);
    }
}

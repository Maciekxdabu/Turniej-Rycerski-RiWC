using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform camerasYPosition;
    [SerializeField] private Map.MapPosition[] spawnPositions;
    [Header("Other references --- Remove when redundant")]
    [SerializeField] private GameObject startGameBtn;
    [SerializeField] private GameObject exitGameBtn;

    private PlayerInputManager playerInputManager;

    private List<Player> activePlayers = new List<Player>();

    //singleton
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    // ---------- Unity messages

    private void Awake()
    {
        _instance = this;

        //get needed references
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    // ---------- Player Input Manager messages

    //should make sure the player joined correctly (e.g. correct prefab) (only configurations which need to happen only once)
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.DeactivateInput();
        Player player = playerInput.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogWarning("ERR: Player attempted to spawn/join without a Player Component... Removing...", gameObject);
            Destroy(playerInput.gameObject);
            return;
        }

        //Init Player and add it to the Map (for movement)
        player.Init(camerasYPosition.position.y);
        Map.AddPlayer(player);

        return;
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {

    }

    // ---------- public methods (UI Buttons)

    public void StartGameBtn()
    {
        if (PlayerInput.all.Count <= 0)
        {
            Debug.LogWarning("WAR: There is no enough Players to start the game, at least 1 Player is required", gameObject);
            return;
        }

        //disable the start game button
        startGameBtn.SetActive(false);
        exitGameBtn.SetActive(false);

        //disable joining of new players
        playerInputManager.DisableJoining();

        //setup players
        activePlayers.Clear();
        for (int i = 0; i < PlayerInput.all.Count; i++)
        {
            if (PlayerInput.all[i].TryGetComponent<Player>(out Player player))
            {
                activePlayers.Add(player);
                player.OnStartGame(spawnPositions[i]);
            }
        }
    }

    public void ExitGameBtn()
    {
        Application.Quit();
    }

    // ---------- public methods

    public void OnPlayerDeath(Player deadPlayer)
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
            if (PlayerInput.all[i].TryGetComponent<Player>(out Player player))
            {
                player.ManualPlayerDisable();
            }
        }

        //display score/result


        // --- reenable joining and UI
        //reanable joining
        playerInputManager.EnableJoining();

        //reanable the start game button
        startGameBtn.SetActive(true);
        exitGameBtn.SetActive(true);
    }
}

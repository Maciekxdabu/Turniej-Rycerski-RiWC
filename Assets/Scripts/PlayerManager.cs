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

    private PlayerInputManager playerInputManager;

    // ---------- Unity messages

    private void Awake()
    {
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

        //add Player to Map (for movement)
        Map.AddPlayer(player);

        return;
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {

    }

    // ---------- public methods

    public void StartGame()
    {
        if (PlayerInput.all.Count <= 0)
        {
            Debug.LogWarning("WAR: There is no enough Players to start the game, at least 1 Player is required", gameObject);
            return;
        }

        //disable the start game button
        startGameBtn.SetActive(false);

        //disable joining of new players
        playerInputManager.DisableJoining();

        //setup players
        for (int i = 0; i < PlayerInput.all.Count; i++)
        {
            if (PlayerInput.all[i].TryGetComponent<Player>(out Player player))
            { 
                player.Init(spawnPositions[i], camerasYPosition.position.y);
            }
        }
    }

    public void OnGameFinished()
    {
        //disable all players input (not only the winning one, because the match could end in different ways)

        //display score/result

        //reanable joining
        playerInputManager.EnableJoining();

        //reanable the start game button
        startGameBtn.SetActive(true);
    }
}

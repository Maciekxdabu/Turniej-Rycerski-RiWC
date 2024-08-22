using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform camerasYPosition;
    [SerializeField] private Map.MapPosition[] spawnPositions;

    private bool[] spawnsTaken = { };

    private PlayerInputManager playerInputManager;

    // ---------- Unity messages

    private void Awake()
    {
        //get needed references
        playerInputManager = GetComponent<PlayerInputManager>();

        //configure spawn positions
        spawnsTaken = new bool[spawnPositions.Length];
        for (int i = 0; i < spawnsTaken.Length; i++)
            spawnsTaken[i] = false;
    }

    // ---------- Player Input Manager messages

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Player player = playerInput.GetComponent<Player>();

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (spawnsTaken[i] == false)//configure Player if there is an available spawn position
            {
                player.Init(spawnPositions[i], camerasYPosition.position.y);
                spawnsTaken[i] = true;

                Map.AddPlayer(player);
                return;
            }
        }

        Debug.LogWarning("WAR: A Player attempted to spawn without a free spawn position", gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {

    }
}

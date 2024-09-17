using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public int playerIndex;
        public string controlScheme;
        public InputDevice[] devices;
    }

    private PlayerInputManager inputManager;

    [SerializeField] private List<PlayerData> debugList;

    private static List<PlayerData> _playerDataList = new List<PlayerData>();
    public static List<PlayerData> PlayerDataList { get { return _playerDataList; } }

    // ---------- Unity messages

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
    }

    // ---------- public methods (called by Events)

    public void OnConfigurationStart()
    {
        inputManager.EnableJoining();
    }

    public void OnConfigurationEnd()
    {
        inputManager.DisableJoining();
    }

    // ---------- public methods (called by PlayerInputManager)

    public void OnPlayerJoined(PlayerInput player)
    {
        PlayerData data = new PlayerData();

        data.playerIndex = player.playerIndex;
        data.controlScheme = player.currentControlScheme;
        data.devices = player.devices.ToArray();

        _playerDataList.Add(data);
        debugList.Add(data);

        Debug.Log("Added a new Player");
    }

    public void OnPlayerLeft(PlayerInput player)
    {
        if (inputManager.joiningEnabled)
        {
            _playerDataList.RemoveAll(x => x.playerIndex == player.playerIndex);
            debugList.RemoveAll(x => x.playerIndex == player.playerIndex);

            Debug.Log("Removed a Player");
        }
    }
}

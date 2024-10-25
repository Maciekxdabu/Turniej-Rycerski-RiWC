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

    [SerializeField] private Transform[] spawnPositions;

    [SerializeField, ReadOnly] private List<PlayerData> debugList;

    //private values
    private PlayerInputManager inputManager;

    //static values
    private static List<PlayerData> _playerDataList = new List<PlayerData>();
    public static List<PlayerData> PlayerDataList { get { return _playerDataList; } }

    // ---------- Unity messages

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        if (_playerDataList.Count > 0)
        {
            List<PlayerData> newList = new List<PlayerData>(_playerDataList);
            _playerDataList.Clear();
            foreach (PlayerData data in newList)
            {
                inputManager.JoinPlayer(controlScheme: data.controlScheme, pairWithDevices: data.devices);
            }
        }
    }

    // ---------- public methods (called by Events)

    public void OnConfigurationStart()
    {
        inputManager.EnableJoining();
    }

    public void OnConfigurationEnd()
    {
        //configure split-devices, etc.)
        //TODO

        inputManager.DisableJoining();
    }

    // ---------- public methods (called by PlayerInputManager)

    public void OnPlayerJoined(PlayerInput player)
    {
        //set spawn position
        player.transform.position = spawnPositions[_playerDataList.Count].position;

        //get Player Input data
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

            for (int i = 0; i < PlayerInput.all.Count; i++)
            {
                PlayerInput.all[i].transform.position = spawnPositions[i].position;
            }

            Debug.Log("Removed a Player");
        }
    }
}

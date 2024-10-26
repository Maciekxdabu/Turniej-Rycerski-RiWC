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
        public PlayerDeviceSetup deviceSetup;
    }

    [SerializeField] private Transform[] spawnPositions;

    [SerializeField, ReadOnly] private List<PlayerData> debugList;

    //private values
    private PlayerInputManager inputManager;

    //static values
    private static List<PlayerData> _playerDataList = new List<PlayerData>();
    private static List<PlayerData> _finalPlayerDataList = new List<PlayerData>();
    public static List<PlayerData> PlayerDataList { get { return _finalPlayerDataList; } }

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

        foreach (PlayerInput player in PlayerInput.all)
        {
            player.ActivateInput();
        }
    }

    public void OnConfigurationEnd()
    {
        //configure split-devices, etc.)
        _finalPlayerDataList.Clear();
        foreach (PlayerData data in _playerDataList)
        {
            if (data.deviceSetup.split && data.controlScheme == "Keyboard&Mouse")//add split keyboard (2 Players)
            {
                Debug.Log("Split Players keyboard");

                PlayerData newData1 = new PlayerData();
                PlayerData newData2 = new PlayerData();

                //newData1.playerIndex = data.playerIndex;
                newData1.controlScheme = "Keyboard1";
                newData2.controlScheme = "Keyboard2";
                newData1.devices = data.devices;
                newData2.devices = data.devices;

                _finalPlayerDataList.Add(newData1);
                _finalPlayerDataList.Add(newData2);
            }
            else//add single keyboard/device (1 Player)
            {
                PlayerData newData = new PlayerData();

                //newData.playerIndex = data.playerIndex;
                newData.controlScheme = data.controlScheme;
                newData.devices = data.devices;

                _finalPlayerDataList.Add(newData);
            }
        }

        inputManager.DisableJoining();

        foreach (PlayerInput player in PlayerInput.all)
        {
            player.DeactivateInput();
        }
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
        data.deviceSetup = player.GetComponent<PlayerDeviceSetup>();

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

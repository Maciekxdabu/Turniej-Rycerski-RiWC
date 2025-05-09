using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the gameplay
/// It checks the existing PlayerBrain's instances from its static field and initializes them for gameplay
/// After initialization ends, it starts the gameplay
/// </summary>
public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class DebugPlayerSpawn
    {
        public string controlScheme = "Keyboard1";
        public Horse horse;
        public Knight knight;
    }

    [Header("DEBUG")]
    [SerializeField] private bool DEBUG_JOINING = false;
    [SerializeField] private GameObject playerBrainPrefab = null;
    [Tooltip("WAR: Only keyboards for now")]
    [SerializeField] private DebugPlayerSpawn[] debugPlayerSpawnData = null;
    [Header("Values")]
    [SerializeField] private GamePlayer[] gamePlayers;
    [Tooltip("Setting this reference here, will also set it for all connected MapEvaluators")]
    [field: SerializeField] public MapDefinition mapData { get; private set; }
    [field: SerializeField] public MapEvaluator mapEvaluator { get; private set; }
    [field: SerializeField] public MapEvaluator minimapEvaluator { get; private set; }

    private List<PlayerBrain> activePlayers = new List<PlayerBrain>();
    private List<PlayerBrain> deadPlayers = new List<PlayerBrain>();

    public static int lobbyScenebuildIndex { get; set; }
    public static PlayerBrain lastWinner { get; private set; }

    //singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (Application.isPlaying)
                return _instance;
            else
                return FindAnyObjectByType<GameManager>();
        }
    }

    // ---------- Unity messages

    private void Awake()
    {
        _instance = this;

        ClearLastWinner();
    }

    private void Start()
    {
        //Add "automatic" Players if Debug joining is enabled
        if (DEBUG_JOINING && PlayerBrain.all.Count == 0)
        {
            for (int i=0; i<debugPlayerSpawnData.Length; i++)
            {
                DebugPlayerSpawn data = debugPlayerSpawnData[i];
                PlayerInput.Instantiate(playerBrainPrefab, controlScheme: data.controlScheme, pairWithDevice: Keyboard.current);
                PlayerBrain.all[i].ApplyConfiguration(data.horse, data.knight);
            }

            //Set Lobby scene to LocalLobby scene
            lobbyScenebuildIndex = 1;
        }

        //Check if the game can be started (correct number of Players, etc.)
        //TODO

        //initialize the Players present
        activePlayers.Clear();
        deadPlayers.Clear();
        for (int i=0; i<PlayerBrain.all.Count; i++)
        {
            InitializePlayer(PlayerBrain.all[i]);
        }

        //Cleanup/do something with the rest of the GamePlayers
        for (int i=activePlayers.Count; i<gamePlayers.Length; i++)
        {
            gamePlayers[i].InitDisable();
        }

        //once all Players are initialized, start the gameplay
        StartGame();
    }

    private void OnValidate()
    {
        if (mapEvaluator != null)
            mapEvaluator.mapData = mapData;
        if (minimapEvaluator != null)
            minimapEvaluator.mapData = mapData;
    }

    // ---------- public methods

    //event for GameManager to react to Player death (Players should handle their death themselves)
    public void OnPlayerDeath(PlayerBrain deadPlayer)
    {
        //remove player from alive list and move to dead list
        activePlayers.Remove(deadPlayer);
        deadPlayers.Add(deadPlayer);

        //check if there is a single winner present
        if (activePlayers.Count <= 1)
        {
            OnGameFinished();
        }
    }

    public void ClearLastWinner()
    {
        lastWinner = null;
    }

    // ---------- private methods

    //Initializes the Player
    private void InitializePlayer(PlayerBrain brain)
    {
        //check Player type (player, observer, etc.)
        //TODO - for later

        //Assign PlayerBrain to a GamePlayer and initialize it
        GamePlayer gamePlayer = gamePlayers[activePlayers.Count];
        brain.InitForGameplay(gamePlayer);

        //Add player to activePlayers list
        activePlayers.Add(brain);
    }

    private void StartGame()
    {
        Debug.Log("===Started the game!===");

        //inform players
        foreach (PlayerBrain brain in activePlayers)
        {
            brain.OnStartGame();
        }
    }

    private void OnGameFinished()
    {
        // --- after game finishes

        //get winner (last active player)
        lastWinner = activePlayers[0];

        //inform all players about finished game
        foreach (PlayerBrain brain in activePlayers)
        {
            brain.OnGameFinished();
        }
        foreach (PlayerBrain brain in deadPlayers)
        {
            brain.OnGameFinished();
        }
        //TODO - maybe use an event for this to not do two foreach'es???

        //display score/result (time it out like the commentator would later) (in the future, just call commentator)
        //TODO

        //load Main Menu Scene
        SceneManager.LoadScene(lobbyScenebuildIndex);
    }
}

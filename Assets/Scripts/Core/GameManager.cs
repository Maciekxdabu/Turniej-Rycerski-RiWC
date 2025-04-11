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
    [Header("DEBUG")]
    [SerializeField] private bool DEBUG_JOINING = false;
    [Header("Values")]
    [SerializeField] private GamePlayer[] gamePlayers;
    [SerializeField] private string menuSceneName;

    private List<PlayerBrain> activePlayers = new List<PlayerBrain>();
    private List<PlayerBrain> deadPlayers = new List<PlayerBrain>();

    //singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // ---------- Unity messages

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        //Add "automatic" Players if Debug joining is enabled
        if (DEBUG_JOINING)
        {
            //TODO
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
        for (int i=activePlayers.Count-1; i<gamePlayers.Length; i++)
        {
            //TODO
        }

        //once all Players are initialized, start the gameplay
        StartGame();
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
        //inform players
        foreach (PlayerBrain brain in activePlayers)
        {
            brain.OnGameFinished();
        }

        //display score/result
        //TODO

        //load Main Menu Scene
        SceneManager.LoadScene(menuSceneName);
    }
}

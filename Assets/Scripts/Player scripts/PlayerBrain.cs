using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Class meant to represent the Player in the game. It's identity (later server-owned)
/// </summary>
public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private PlayerInputController inputController;
    [Header("Player Data")]
    [field: SerializeField] public Horse horseData { get; private set; }
    [field: SerializeField] public Knight knightData { get; private set; }
    [Header("Auto References")]
    [SerializeField, ReadOnly] private GamePlayer gamePlayer;

    //input
    private bool canBeControlled = false;

    //singleton list
    private static List<PlayerBrain> _all = new List<PlayerBrain>();
    public static ReadOnlyArray<PlayerBrain> all { get { return new ReadOnlyArray<PlayerBrain>(_all.ToArray()); } }

    // ---------- Unity methods

    private void Awake()
    {
        _all.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        _all.Remove(this);
    }

    // ---------- public methods

    public void InitForGameplay(GamePlayer _gamePlayer)
    {
        gamePlayer = _gamePlayer;
        gamePlayer.Init(horseData, knightData, this);
    }

    //[Server]
    public void OnStartGame()
    {
        gamePlayer.OnStartGame();
    }

    //[Server]
    public void OnGameFinished()
    {
        //disable input
        DeactivateInput();

        //stop animations and other logic
        //TODO

        //Clear no-longer needed references
        //TODO
    }

    // ---------- Command methods (called by assigned PlayerController) (will work only if input is activated)

    public void CmdMove(float moveDir)
    {
        if (!CanBeControlled())
            return;

        gamePlayer.CmdMove(moveDir);
    }

    public void CmdRaiseLine()
    {
        if (!CanBeControlled())
            return;

        gamePlayer.CmdRaiseLine();
    }

    public void CmdLowerLine()
    {
        if (!CanBeControlled())
            return;

        gamePlayer.CmdLowerLine();
    }

    public void CmdRaiseLance()
    {
        if (!CanBeControlled())
            return;

        gamePlayer.CmdRaiseLance();
    }

    public void ActivateInput()
    {
        canBeControlled = true;

        inputController.SwitchToGameplay();
    }

    public void DeactivateInput()
    {
        canBeControlled = false;
    }

    private bool CanBeControlled()
    {
        return canBeControlled;
    }

    // ---------- Configure methods

    public void ApplyConfiguration(Horse horse, Knight knight)
    {
        horseData = horse;
        knightData = knight;
    }
}

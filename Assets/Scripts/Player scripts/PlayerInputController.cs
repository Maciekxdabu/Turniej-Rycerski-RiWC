using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Class responsible for sending input from devices to the PlayerBrain
/// </summary>
[RequireComponent(typeof(PlayerBrain), typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private MultiplayerEventSystem eventSystem;

    private PlayerBrain brain;
    private PlayerInput playerInput;

    private void Awake()
    {
        brain = GetComponent<PlayerBrain>();
        playerInput = GetComponent<PlayerInput>();
    }

    // ---------- Configure methods

    /// <summary>
    /// Called when spawned by LocalPlayerSetup
    /// </summary>
    public void Init(GameObject uiRoot, GameObject firstSelected)
    {
        //get references
        brain = GetComponent<PlayerBrain>();
        playerInput = GetComponent<PlayerInput>();

        //switch player to the Setup UI
        SwitchToUI(uiRoot, firstSelected);
    }

    public void SwitchToUI(GameObject uiRoot=null, GameObject firstSelected=null)
    {
        playerInput.SwitchCurrentActionMap("UI");

        //assign new UI/EventSystem if provided
        if (uiRoot != null)
            eventSystem.playerRoot = uiRoot;
        if (firstSelected != null)
            eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void SwitchToGameplay()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    // ---------- Input methods

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            brain.CmdMove(ctx.ReadValue<float>());
        else if (ctx.canceled)
            brain.CmdMove(0f);
    }

    public void OnRaiseLine(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            brain.CmdRaiseLine();
    }

    public void OnLowerLine(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            brain.CmdLowerLine();
    }

    public void OnRaiseLance(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            brain.CmdRaiseLance();
    }
}

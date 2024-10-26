using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(PlayerInput))]
public class PlayerDeviceSetup : MonoBehaviour
{
    [SerializeField] private bool _split = false;
    public bool split { get { return _split; } }
    [SerializeField] private SpriteRenderer splitDeviceSprite;

    private PlayerInput playerInput;

    // ---------- Unity methods

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        splitDeviceSprite.enabled = _split;
    }

    // ---------- public methods

    public void OnSplit(InputAction.CallbackContext ctx)
    {
        if (ctx.started && playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            _split = !_split;
            splitDeviceSprite.enabled = _split;
        }
    }
}

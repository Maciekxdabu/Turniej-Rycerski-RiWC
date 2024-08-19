using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField] private Horse horse = null;
    [Space]
    [SerializeField] private Animator lanceAnimator = null;
    [Space]
    [SerializeField] private Camera playerCamera;

    [Header("Debug Values")]
    [SerializeField] private int line;
    [SerializeField] private float position;

    private bool right = true;
    private bool canLance = true;

    private float velocity = 0f;

    //inputs
    private float moveValue = 0f;

    // ---------- Unity methods

    private void Start()
    {
        playerCamera.transform.parent = null;
    }

    private void Update()
    {
        if (moveValue != 0)
        {
            velocity += Mathf.Clamp(moveValue * horse.acceleration * Time.deltaTime, -horse.maxSpeed, horse.maxSpeed);

            if (velocity < 0f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                right = false;
            }
            else if (velocity > 0f)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                right = true;
            }
        }

        position += velocity * Time.deltaTime;

        if (position < 0)
        {
            velocity = 0;
            position = 0;
        }
        else if (position > 1)
        {
            velocity = 0;
            position = 1;
        }
    }

    // ---------- public methods

    public void Warp(Map.MapPosition newPosition)
    {
        line = newPosition.line;
        position = newPosition.position;
    }

    // ---------- Input methods (assigned in Inspector)

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            moveValue = ctx.ReadValue<float>();
        else if (ctx.canceled)
            moveValue = 0f;
    }

    public void OnRaiseLine(InputAction.CallbackContext ctx)
    {
        if (ctx.started && Map.CanChangeLine(line, position, true))
        {
            line--;
        }
    }

    public void OnLowerLine(InputAction.CallbackContext ctx)
    {
        if (ctx.started && Map.CanChangeLine(line, position, false))
        {
            line++;
        }
    }

    public void OnRaiseLance(InputAction.CallbackContext ctx)
    {
        if (canLance && ctx.started)
        {
            canLance = false;
            lanceAnimator.SetTrigger("MoveLanceUp");
        }
    }

    // ---------- public getters

    public (int, float) GetPositions()
    {
        return (line, position);
    }

    // ---------- Called in Unity (by Events)

    public void ResetLance()
    {
        canLance = true;
    }
}

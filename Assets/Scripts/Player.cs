using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Horse horse = null;
    [Space]
    [SerializeField] private Transform Lance = null;

    private Rigidbody2D rigidb = null;
    private bool right = true;

    //inputs
    private float moveValue = 0f;

    // ---------- Unity methods

    private void Awake()
    {
        rigidb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (moveValue != 0)
        {
            rigidb.AddForce(Vector2.right * moveValue * horse.acceleration);
            ConstrainSpeed();

            if (right && rigidb.velocity.x < 0)//check if moves left now
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                right = false;
            }
            else if (!right && rigidb.velocity.x > 0)//check if move right now
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                right = true;
            }
        }
    }

    // ---------- Input methods

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            moveValue = ctx.ReadValue<float>();
        else if (ctx.canceled)
            moveValue = 0f;
    }

    public void OnRaiseLine(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Map.ChangeLine(this, true);
        }
    }

    public void OnLowerLine(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Map.ChangeLine(this, false);
        }
    }

    public void OnRaiseLance(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            throw new System.NotImplementedException();
        }
    }

    // ---------- private methods

    private void ConstrainSpeed()
    {
        if (rigidb.velocity.x > horse.maxSpeed)
        {
            Vector2 newPos = rigidb.velocity;
            newPos.x = horse.maxSpeed;
            rigidb.velocity = newPos;
        }
        else if (rigidb.velocity.x < -horse.maxSpeed)
        {
            Vector2 newPos = rigidb.velocity;
            newPos.x = -horse.maxSpeed;
            rigidb.velocity = newPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private static string upTag = "MoveUp";
    private static string downTag = "MoveDown";

    [SerializeField] private Horse horse = null;
    [Space]
    [SerializeField] private Animator lanceAnimator = null;

    private Rigidbody2D rigidb = null;
    private bool right = true;
    private bool canUp = false;
    private bool canDown = false;
    private bool canLance = true;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == upTag)
            canUp = true;
        if (collision.tag == downTag)
            canDown = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == upTag)
            canUp = false;
        if (collision.tag == downTag)
            canDown = false;
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
        if (canUp && ctx.started)
        {
            Map.ChangeLine(this, true);
        }
    }

    public void OnLowerLine(InputAction.CallbackContext ctx)
    {
        if (canDown && ctx.started)
        {
            Map.ChangeLine(this, false);
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

    // ---------- Called in Unity

    public void ResetLance()
    {
        canLance = true;
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

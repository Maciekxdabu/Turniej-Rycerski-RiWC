using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Horse horseData = null;
    [SerializeField] private Knight knightData = null;
    [SerializeField] private float invincibilityTime = 1.5f;
    [Space]
    [SerializeField] private Animator lanceAnimator = null;
    [Space]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private PlayerSetup playerSetup;
    [Header("REMOVE WHEN REDUNDANT")]
    [SerializeField] private SpriteRenderer knightSprite;
    [SerializeField] private Slider healthSlider;

    [Header("Debug Values")]
    [SerializeField] private int line;
    [SerializeField] private float position;

    private bool right = true;
    private bool canLance = true;
    private bool lunging = false;
    private bool canBeDamaged = true;

    private float health = 100f;
    private float velocity = 0f;

    //inputs
    private PlayerInput input;
    private float moveValue = 0f;

    // ---------- Unity methods

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        //change velocity on input
        if (moveValue != 0)
        {
            velocity += Mathf.Clamp(moveValue * horseData.acceleration * Time.deltaTime, -horseData.maxSpeed, horseData.maxSpeed);

            if (velocity < 0f)
            {
                right = false;
                Orient();
            }
            else if (velocity > 0f)
            {
                right = true;
                Orient();
            }
        }

        //change position based on current velocity
        position += velocity * Time.deltaTime;

        //validate position with Map
        transform.position = Map.OnMove(this, position, line);
    }

    // ---------- public methods

    public void Init(CoopSetup coopSetup)
    {
        //deparent Canvas and Camera
        playerCanvas.transform.SetParent(null);
        playerCamera.transform.SetParent(null);
        if (UISpaces.Instance != null)
            RetargetCamera(UISpaces.Instance.UItransforms[PlayerInput.all.Count - 1]);

        //Add Player to CoopSetup system
        coopSetup.AddPlayer(playerSetup);
    }

    public void OnStartGame(Map.MapPosition newPosition)
    {
        //enable all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        //get Coop Setup values
        horseData = playerSetup.GetHorse();
        knightData = playerSetup.GetKnight();

        //setup up Player values
        health = 100f;
        healthSlider.value = 1f;

        velocity = 0f;
        line = newPosition.line;
        position = Map.Instance.LenToUnit(newPosition.position);
        right = newPosition.right;
        Orient();

        canBeDamaged = true;
        canLance = true;
        lunging = false;

        lanceAnimator.Rebind();
        lanceAnimator.speed = 1f;

        //setup camera (Y position in the middle of the map)
        RetargetCamera(transform);
        Vector3 pos = playerCamera.transform.position;
        pos.y = Map.GetCameraYPosition();
        playerCamera.transform.position = pos;

        //activate input
        input.ActivateInput();
        //input.SwitchCurrentActionMap("Player");//Will be needed if Player will start with another default Action Map (UI technically does not matter)
    }

    //called when a player hit another player with a lance (checks if the player can receive damage)
    public void ReceiveHit(PlayerController damagingPlayer)
    {
        //check if damage applicable (lunging and speed requirement)
        if (canBeDamaged && damagingPlayer.lunging && Mathf.Abs(damagingPlayer.velocity) > damagingPlayer.horseData.minDamagingSpeed)
            StartCoroutine(DamageSelf(damagingPlayer));
    }

    public void ManualPlayerDisable()
    {
        input.DeactivateInput();
        velocity = 0f;
        lanceAnimator.speed = 0f;
    }

    public void OnOutOfBounds()
    {
        velocity = 0f;

        //apply out of bounds animation based on current speed
        //TODO
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

    public void OnLungeStart()
    {
        lunging = true;
    }

    public void OnLungeEnd()
    {
        lunging = false;
    }

    public void ResetLance()
    {
        canLance = true;
    }

    // ---------- private methods

    private void Orient()
    {
        if (right)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        else
            transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    //applies damage to player, plays animation, etc
    private IEnumerator DamageSelf(PlayerController damagingPlayer)
    {
        if (canBeDamaged == false)
            yield break;
        canBeDamaged = false;

        //disable input
        input.DeactivateInput();
        velocity = 0;
        //play animation (which applies cooldown) (maybe a Corutine)
        Color color = knightSprite.color;
        color.a = 0.5f;
        knightSprite.color = color;

        // --- remove damaged health
        health -= damagingPlayer.horseData.strength;
        healthSlider.value = health / 100f;
        Debug.Log("Player received damage", gameObject);

        // --- Invincibility time
        //wait for invincibility time (later animation length)
        yield return new WaitForSeconds(invincibilityTime);
        //reset player
        color.a = 1f;
        knightSprite.color = color;

        // --- call death after animation ended and health <= 0
        if (health <= 0)
            OnPlayerDeath();
        else//reactivate input if player is still alive
        {
            input.ActivateInput();
            canBeDamaged = true;
        }
    }

    private void OnPlayerDeath()
    {
        input.DeactivateInput();
        //disable all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //tell the Player Manager that this Player died
        GameManager.Instance.OnPlayerDeath(this);
    }

    private void RetargetCamera(Transform newTarget)
    {
        PositionConstraint constraint = playerCamera.GetComponent<PositionConstraint>();

        //clear constraint sources
        while (constraint.sourceCount > 0)
            constraint.RemoveSource(0);

        //add new target to constraint
        ConstraintSource newSource = new ConstraintSource();
        newSource.weight = 1;
        newSource.sourceTransform = newTarget;
        constraint.AddSource(newSource);
    }
}

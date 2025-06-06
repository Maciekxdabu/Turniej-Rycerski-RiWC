using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using DG.Tweening;

/// <summary>
/// Class meant to control the in-game Player (later server-owned)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class GamePlayer : MonoBehaviour
{
    [SerializeField] private float invincibilityTime = 1.5f;
    [SerializeField] private float outOfBoundsDamage = 20f;
    [Space]
    [SerializeField] private SpriteLibrary horseSpriteLibrary;
    [SerializeField] private SpriteLibrary knightSpriteLibrary;
    [SerializeField] private Animator spriteAnimator;
    [SerializeField] private Animator lanceAnimator = null;
    [Space]
    [SerializeField] private SortingGroup playerSortingGroup;
    [SerializeField] private CanvasGroup disabledPlayerGroup;
    [Space]
    [SerializeField] private MinimapPlayer minimapRepresentation;
    [Header("REMOVE WHEN REDUNDANT")]
    [SerializeField] private SpriteRenderer knightSprite;

    [Header("Debug Values")]
    [SerializeField] private int line;
    [SerializeField] private float position;
    [SerializeField] private bool right = true;

    //player data (gotten from PlayerBrain)
    [SerializeField, ReadOnly] private Horse horseData = null;
    [SerializeField, ReadOnly] private Knight knightData = null;

    PlayerBrain brain;

    private bool canLance = true;
    private bool lunging = false;
    private bool canBeDamaged = true;
    private bool outOfBoundsPlaying = false;

    private float maxHealth = 100f;
    private float health = 100f;
    [SerializeField, ReadOnly] private float velocity = 0f;

    //input
    private float moveValue = 0f;

    // ---------- Unity messages

    private void Update()
    {
        if (outOfBoundsPlaying == false)
        {
            //change velocity on input
            if (moveValue != 0)
            {
                velocity += moveValue * horseData.acceleration * Time.deltaTime;
                velocity = Mathf.Clamp(velocity, -horseData.maxSpeed, horseData.maxSpeed);
                spriteAnimator.SetFloat("Speed", Mathf.Abs(velocity));

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
        }

        //(transform.position, outOfBounds) = MapController.OnMove(this, position, line);

        MapEvaluator.PositionState outOfBounds;

        //Update Map and Minimap position (normalize by mapEvaluator)
        float normalizedPosition = GameManager.Instance.mapEvaluator.UnitToNorm(position);
        outOfBounds = GameManager.Instance.mapEvaluator.OnMove(transform, normalizedPosition, line);
        minimapRepresentation.OnMove(normalizedPosition, line);

        //Process out of bounds event
        ProcessOutOfBounds(outOfBounds);
    }

    private void OnValidate()
    {
        if (GameManager.Instance == null)
            return;

        //Update transform and visuals based on Map position
        if (GameManager.Instance.mapEvaluator != null)
        {
            float normalizedPosition = GameManager.Instance.mapEvaluator.UnitToNorm(position);
            GameManager.Instance.mapEvaluator.OnMove(transform, normalizedPosition, line);
            if (minimapRepresentation != null)
                minimapRepresentation.OnMove(normalizedPosition, line);

            playerSortingGroup.sortingOrder = line * 2;
            Orient();
        }
    }

    // ---------- public methods

    //Called to initialize the Player for gameplay
    public void Init(Horse _horseData, Knight _knightData, PlayerBrain _brain)
    {
        //assign values
        brain = _brain;
        horseData = _horseData;
        knightData = _knightData;

        //apply horse and knight data
        horseSpriteLibrary.spriteLibraryAsset = horseData?.spriteLibrary;
        knightSpriteLibrary.spriteLibraryAsset = knightData?.spriteLibrary;

        //fix visuals
        playerSortingGroup.sortingOrder = line * 2;
        Orient();

        //initialize Minimap Player (update visuals)
        //TODO
    }

    //Called when GamePlayer is initialized as "disabled" (not taking part i the gameplay)
    public void InitDisable()
    {
        //disable all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //disable Minimap representation
        minimapRepresentation.Hide();

        //Hide the Player Canvas
        disabledPlayerGroup.alpha = 1f;

        //Check other things to potentialy disable
        //TODO
    }

    public void OnStartGame()
    {
        //setup up Player values
        health = maxHealth;

        //initialize movement
        velocity = 0f;

        //reset flags
        canBeDamaged = true;
        canLance = true;
        lunging = false;

        lanceAnimator.Rebind();
        lanceAnimator.speed = 1f;

        //activate input
        brain.ActivateInput();
    }

    //called when a player hit another player with a lance (checks if the player can receive damage)
    public void ReceiveHit(GamePlayer damagingPlayer)
    {
        //check if damage applicable (lunging and speed requirement)
        if (canBeDamaged && damagingPlayer.lunging && Mathf.Abs(damagingPlayer.velocity) > damagingPlayer.horseData.minDamagingSpeed)
            StartCoroutine(DamageSelf(damagingPlayer));
    }

    public void ManualPlayerDisable()
    {
        brain.DeactivateInput();
        velocity = 0f;
        lanceAnimator.speed = 0f;
    }

    public void ProcessOutOfBounds(MapEvaluator.PositionState outOfBoundsState)
    {
        if (outOfBoundsState == MapEvaluator.PositionState.Normal)
            return;

        switch (outOfBoundsState)
        {
            case MapEvaluator.PositionState.Normal:
                break;
            case MapEvaluator.PositionState.OutOfBoundsLeft:
            case MapEvaluator.PositionState.OutOfBoundsRight:
                StartCoroutine(OnPlayerOutOfBounds());
                break;
            case MapEvaluator.PositionState.Invalid:
                Debug.LogWarning("WAR: The position on the Map is invalid", gameObject);
                break;
        }
    }

    // ---------- Command methods (called by assigned PlayerBrain when canBeControlled==true)

    public void CmdMove(float moveDir)
    {
        moveValue = moveDir;
    }

    public void CmdRaiseLine()
    {
        float normalizedPosition = GameManager.Instance.mapEvaluator.UnitToNorm(position);
        if (GameManager.Instance.mapData.CanChangeLine(line, normalizedPosition, true))
        {
            line--;
            playerSortingGroup.sortingOrder = line * 2;
        }
    }

    public void CmdLowerLine()
    {
        float normalizedPosition = GameManager.Instance.mapEvaluator.UnitToNorm(position);
        if (GameManager.Instance.mapData.CanChangeLine(line, normalizedPosition, false))
        {
            line++;
            playerSortingGroup.sortingOrder = line * 2;
        }
    }

    public void CmdRaiseLance()
    {
        if (canLance)
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

    //Subtracts Player health and updates its display values
    private void SubtractHealth(float value)
    {
        health -= value;
        minimapRepresentation.UpdateHealth(health / maxHealth);
        Debug.Log("Player received damage: " + value.ToString("00.00"), gameObject);
    }

    private void Orient()
    {
        if (right)
            playerSortingGroup.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else
            playerSortingGroup.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    //applies damage to the player, plays animation, etc
    private IEnumerator DamageSelf(GamePlayer damagingPlayer)
    {
        //ensure only one is playing at a time
        if (canBeDamaged == false)
            yield break;
        canBeDamaged = false;

        //disable input
        brain.DeactivateInput();
        velocity = 0;
        //play animation (which applies cooldown) (maybe a Corutine)
        Color color = knightSprite.color;
        color.a = 0.5f;
        knightSprite.color = color;

        // --- remove damaged health
        SubtractHealth(damagingPlayer.horseData.strength);

        // --- Invincibility time
        //wait for invincibility time (later animation length present above)
        yield return new WaitForSeconds(invincibilityTime);
        //reset player (remove when proper animation exists)
        color.a = 1f;
        knightSprite.color = color;

        // --- call death after animation ended and health <= 0
        if (health <= 0)
            OnPlayerDeath();
        else//reactivate input if player is still alive
        {
            brain.ActivateInput();
            canBeDamaged = true;
        }
    }

    //used by Virtual Tweeners
    public void CallbackPosition(float value)
    {
        position = value;
    }

    private IEnumerator OnPlayerOutOfBounds()
    {
        //ensure only one is playing at a time
        if (outOfBoundsPlaying)
            yield break;
        outOfBoundsPlaying = true;

        //disable input
        brain.DeactivateInput();
        moveValue = 0f;
        velocity = 0;
        canBeDamaged = false;

        //play animation
        Tween tween = DOVirtual.Float(position,
            position + (right ? 5f : -5f),
            5f,
            CallbackPosition).SetEase(Ease.Linear);
        yield return tween.WaitForCompletion();
        tween.Kill();

        //set player orientation and position
        right = !right;
        float arenaNormPadding = GameManager.Instance.mapEvaluator.UnitToNormLength(1f);
        position = GameManager.Instance.mapEvaluator.NormToUnit(right ? arenaNormPadding : 1f - arenaNormPadding);
        velocity = right ? horseData.maxSpeed * 0.2f : horseData.maxSpeed * -0.2f;
        Orient();

        //apply out of bounds damage
        SubtractHealth(outOfBoundsDamage);

        // --- call death after animation ended and health <= 0
        if (health <= 0)
            OnPlayerDeath();
        else//reactivate input if player is still alive
        {
            brain.ActivateInput();
            canBeDamaged = true;
            outOfBoundsPlaying = false;
        }
    }

    private void OnPlayerDeath()
    {
        brain.DeactivateInput();
        //disable all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //tell the Player Manager that this Player died
        GameManager.Instance.OnPlayerDeath(brain);
    }
}

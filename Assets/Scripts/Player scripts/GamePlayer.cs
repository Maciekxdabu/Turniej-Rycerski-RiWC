using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;

/// <summary>
/// Class meant to control the in-game Player (later server-owned)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class GamePlayer : MonoBehaviour
{
    [SerializeField] private float invincibilityTime = 1.5f;
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

    private float maxHealth = 100f;
    private float health = 100f;
    private float velocity = 0f;

    //input
    private float moveValue = 0f;

    // ---------- Unity messages

    private void Update()
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

        //initialize Minimap Player
        //TODO
    }

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

        Debug.Log("Currently out of bounds: " + outOfBoundsState.ToString());
        velocity = 0f;

        //disable input
        //TODO

        //apply out of bounds animation based on out of bounds state
        //TODO
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

    private void Orient()
    {
        if (right)
            playerSortingGroup.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else
            playerSortingGroup.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    //applies damage to player, plays animation, etc
    private IEnumerator DamageSelf(GamePlayer damagingPlayer)
    {
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
        health -= damagingPlayer.horseData.strength;
        minimapRepresentation.UpdateHealth(health / maxHealth);
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
            brain.ActivateInput();
            canBeDamaged = true;
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

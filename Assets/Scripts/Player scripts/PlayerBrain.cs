using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private float invincibilityTime = 1.5f;
    [Space]
    [SerializeField] private SpriteLibrary horseSpriteLibrary;
    [SerializeField] private SpriteLibrary knightSpriteLibrary;
    [SerializeField] private Animator spriteAnimator;
    [SerializeField] private Animator lanceAnimator = null;
    [Space]
    [SerializeField] private SortingGroup playerSortingGroup;
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private PlayerSetup playerSetup;
    [SerializeField] private Horse horseData = null;
    [SerializeField] private Knight knightData = null;
    [Header("REMOVE WHEN REDUNDANT")]
    [SerializeField] private SpriteRenderer knightSprite;

    [Header("Debug Values")]
    [SerializeField] private int line;
    [SerializeField] private float position;

    private bool right = true;
    private bool canLance = true;
    private bool lunging = false;
    private bool canBeDamaged = true;

    private float maxHealth = 100f;
    private float health = 100f;
    private float velocity = 0f;

    //inputs
    private bool inputActive = false;
    private float moveValue = 0f;

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

    private void Update()
    {
        //change velocity on input
        if (false)//TODO - DEBUG - REMOVE WHEN NECESSARY
        {
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

            //validate position with Map
            transform.position = MapController.OnMove(this, position, line);
        }
    }

    // ---------- public methods

    public IEnumerator Init(CoopSetup coopSetup, int splitScreenIndex)
    {
        //deparent Canvas and Camera
        playerCanvas.transform.SetParent(null);
        //if (UISpaces.Instance != null)
        //    RetargetCamera(UISpaces.Instance.UItransforms[PlayerInput.all.Count - 1]);

        //Add Player to CoopSetup system
        coopSetup.AddPlayer(playerSetup);

        yield return new WaitForEndOfFrame();

        //set the bottom Cameras correctly on the screen
        //Rect rect = playerCamera.rect;
        //if (splitScreenIndex > 1)
        //{
        //    rect.y = 0;
        //    playerCamera.rect = rect;
        //}
    }

    public void OnStartGame(MapController.MapPosition newPosition)
    {
        //enable all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        //get Coop Setup values
        horseData = playerSetup.GetHorse();
        knightData = playerSetup.GetKnight();
        horseSpriteLibrary.spriteLibraryAsset = horseData?.spriteLibrary;
        knightSpriteLibrary.spriteLibraryAsset = knightData?.spriteLibrary;

        //setup up Player values
        health = maxHealth;

        velocity = 0f;
        line = newPosition.line;
        playerSortingGroup.sortingOrder = line * 2;
        position = MapController.Instance.LenToUnit(newPosition.position);
        right = newPosition.right;
        Orient();

        canBeDamaged = true;
        canLance = true;
        lunging = false;

        lanceAnimator.Rebind();
        lanceAnimator.speed = 1f;

        //setup camera (Y position in the middle of the map)
        Minimap.Instance.AddPlayer(this);

        //activate input
        ActivateInput();
        //input.SwitchCurrentActionMap("Player");//Will be needed if Player will start with another default Action Map (UI technically does not matter)
    }

    //called when a player hit another player with a lance (checks if the player can receive damage)
    public void ReceiveHit(PlayerBrain damagingPlayer)
    {
        //check if damage applicable (lunging and speed requirement)
        if (canBeDamaged && damagingPlayer.lunging && Mathf.Abs(damagingPlayer.velocity) > damagingPlayer.horseData.minDamagingSpeed)
            StartCoroutine(DamageSelf(damagingPlayer));
    }

    public void ManualPlayerDisable()
    {
        DeactivateInput();
        velocity = 0f;
        lanceAnimator.speed = 0f;
    }

    public void OnOutOfBounds(float newPosition)
    {
        velocity = 0f;
        position = newPosition;

        //apply out of bounds animation based on current speed
        //TODO
    }

    // ---------- Command methods (called by assigned PlayerController) (will work only if input is activated)

    public void CmdMove(float moveDir)
    {
        if (!CanBeControlled())
            return;

        moveValue = moveDir;
    }

    public void CmdRaiseLine()
    {
        if (!CanBeControlled())
            return;

        if (MapController.CanChangeLine(line, position, true))
        {
            line--;
            playerSortingGroup.sortingOrder = line * 2;
        }
    }

    public void CmdLowerLine()
    {
        if (!CanBeControlled())
            return;

        if (MapController.CanChangeLine(line, position, false))
        {
            line++;
            playerSortingGroup.sortingOrder = line * 2;
        }
    }

    public void CmdRaiseLance()
    {
        if (!CanBeControlled())
            return;

        if (canLance)
        {
            canLance = false;
            lanceAnimator.SetTrigger("MoveLanceUp");
        }
    }

    public void ActivateInput()
    {
        inputActive = true;
    }

    public void DeactivateInput()
    {
        inputActive = false;
    }

    private bool CanBeControlled()
    {
        return inputActive;
    }

    // ---------- Configure methods

    public void ApplyConfiguration(Horse horse, Knight knight)
    {
        horseData = horse;
        knightData = knight;
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
    private IEnumerator DamageSelf(PlayerBrain damagingPlayer)
    {
        if (canBeDamaged == false)
            yield break;
        canBeDamaged = false;

        //disable input
        DeactivateInput();
        velocity = 0;
        //play animation (which applies cooldown) (maybe a Corutine)
        Color color = knightSprite.color;
        color.a = 0.5f;
        knightSprite.color = color;

        // --- remove damaged health
        health -= damagingPlayer.horseData.strength;
        Minimap.Instance.UpdateHealth(this, health / maxHealth);
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
            ActivateInput();
            canBeDamaged = true;
        }
    }

    private void OnPlayerDeath()
    {
        DeactivateInput();
        //disable all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //tell the Player Manager that this Player died
        GameManager.Instance.OnPlayerDeath(this);
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for the assigned Player Setup in the Local game
/// </summary>
public class LocalLobbyPlayer : MonoBehaviour
{
    [Header("Configuration variables")]
    [SerializeField] private HorseToggleGroup horseToggleGroup;
    [SerializeField] private KnightToggleGroup knightToggleGroup;
    [SerializeField] private CanvasGroup configurationGroup;
    [SerializeField] private Toggle readyToggle;
    [Header("Preview Graphic")]
    [SerializeField] private Image horseImage;
    [SerializeField] private Image knightImage;
    [Header("References")]
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private CanvasGroup notAssignedGroup;

    private LocalLobbyManager lobbyManager;
    private PlayerBrain brain;

    // ---------- Unity messages

    private void Start()
    {
        readyToggle.SetIsOnWithoutNotify(false);
    }

    // ---------- public methods

    public void Init(LocalLobbyManager manager)
    {
        lobbyManager = manager;
    }

    public void OnChangeHorseClicked()
    {
        OnUpdateSetup();
    }

    public void OnChangeKnightClicked()
    {
        OnUpdateSetup();
    }

    public void OnReady(bool readyState)
    {
        if (readyState)//when player clicked ready
        {
            //do nothing if configuration is not correct (reset ready toggle)
            if (IsSetupValid() == false)
            {
                readyToggle.SetIsOnWithoutNotify(false);
                return;
            }

            //disable configuration interaction
            configurationGroup.interactable = false;

            //send configuration to PlayerBrain
            brain.ApplyConfiguration(GetHorse(), GetKnight());
        }
        else//when 
        {
            //reenable configuration interaction
            configurationGroup.interactable = true;
        }

        //inform LocalLobbyManager
        lobbyManager.OnReady(this);
    }

    public bool IsPlayerReady()
    {
        return readyToggle.isOn || brain == null;
    }

    // ---- data getter methods
    public Horse GetHorse()
    {
        return horseToggleGroup.ChosenData;
    }

    public Knight GetKnight()
    {
        return knightToggleGroup.ChosenData;
    }

    // ---------- for Manager methods

    public void OnPlayerJoined(PlayerInputController player)
    {
        //assign player
        brain = player.GetComponent<PlayerBrain>();

        player.Init(uiRoot, firstSelected);

        //hide unassigned UI
        notAssignedGroup.alpha = 0f;
        notAssignedGroup.interactable = false;
        notAssignedGroup.blocksRaycasts = false;

        //Initialize Toggle groups
        horseToggleGroup.Init(brain.horseData);
        knightToggleGroup.Init(brain.knightData);

        OnUpdateSetup();
    }

    public void OnPlayerLeft()
    {
        //reset player reference
        brain = null;

        //show unassigned UI
        notAssignedGroup.alpha = 1f;
        notAssignedGroup.interactable = true;
        notAssignedGroup.blocksRaycasts = true;
    }

    // ---------- private methods

    //called when a Horse has been changed
    private void OnUpdateSetup()
    {
        Debug.LogFormat(gameObject, "Current setup:\n" +
            "Horse: {0}\n" +
            "Knight: {1}\n", GetHorse(), GetKnight());

        //update graphics (make invisible if null)
        horseImage.sprite = GetHorse()?.spriteLibrary.GetSprite("Preview", "Preview");
        if (horseImage.sprite == null) horseImage.color = new Color(1, 1, 1, 0); else horseImage.color = Color.white;
        knightImage.sprite = GetKnight()?.spriteLibrary.GetSprite("Preview", "Preview");
        if (knightImage.sprite == null) knightImage.color = new Color(1, 1, 1, 0); else knightImage.color = Color.white;

        //display if Player can be ready (change some Text or something...)
        //TODO
    }

    private bool IsSetupValid()
    {
        return GetHorse() != null && GetKnight() != null;
    }
}

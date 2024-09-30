using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Setups a single Player data for gameplay
/// </summary>
public class PlayerSetup : MonoBehaviour
{
    //variables
    [SerializeField] private HorseToggleGroup horseToggleGroup;
    [SerializeField] private KnightToggleGroup knightToggleGroup;
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private CanvasGroup setupGroup;
    [SerializeField] private Toggle readyToggle;
    [Header("Preview Graphic")]
    [SerializeField] private Image horseImage;
    [SerializeField] private Image knightImage;

    private CoopSetup coopSetup;

    //private Horse currentHorse;

    // ---------- Unity messages

    private void Start()
    {
        readyToggle.SetIsOnWithoutNotify(false);

        horseToggleGroup.Init();
        knightToggleGroup.Init();

        OnUpdateSetup();
    }

    // ---------- public methods (called by Events)

    public void OnChangeHorseClicked()
    {
        OnUpdateSetup();
    }

    public void OnReadyClicked()
    {
        if (readyToggle.isOn)//on ready
        {
            //do nothing if setup is not correct (reset ready toggle)
            if (IsSetupValid() == false)
            {
                readyToggle.SetIsOnWithoutNotify(false);
                return;
            }

            //disable setup interaction
            setupGroup.interactable = false;

            //disable navigation on ready toggle
            Navigation navigationData = readyToggle.navigation;
            navigationData.mode = Navigation.Mode.None;
            readyToggle.navigation = navigationData;
        }
        else//on unready
        {
            //enable setup interaction
            setupGroup.interactable = true;

            //enable navigation on ready toggle
            Navigation navigationData = readyToggle.navigation;
            navigationData.mode = Navigation.Mode.Explicit;
            readyToggle.navigation = navigationData;
        }

        coopSetup.OnReady(this);
    }

    //Coop setup methods
    public void Init(CoopSetup _coopSetup)
    {
        coopSetup = _coopSetup;

        mainGroup.alpha = 1.0f;

        OnUpdateSetup();
    }

    public bool IsPlayerReady()
    {
        return readyToggle.isOn;
    }

    public void DisableSetup()
    {
        mainGroup.interactable = false;
        mainGroup.alpha = 0.0f;
        mainGroup.blocksRaycasts = false;
    }

    //data getter methods
    public Horse GetHorse()
    {
        return horseToggleGroup.ChosenData;
    }

    public Knight GetKnight()
    {
        return knightToggleGroup.ChosenData;
    }

    // ---------- private methods

    //called when a Horse has been changed
    private void OnUpdateSetup()
    {
        Debug.LogFormat(gameObject,"Current setup:\n" +
            "Horse: {0}\n" +
            "Knight: {1}\n", horseToggleGroup.ChosenData, knightToggleGroup.ChosenData);

        //update graphics (make invisible if null)
        horseImage.sprite = horseToggleGroup.ChosenData?.spriteLibrary.GetSprite("Preview", "Preview");
        if (horseImage.sprite == null) horseImage.color = new Color(1, 1, 1, 0); else horseImage.color = Color.white;
        knightImage.sprite = knightToggleGroup.ChosenData?.spriteLibrary.GetSprite("Preview", "Preview");
        if (knightImage.sprite == null) knightImage.color = new Color(1, 1, 1, 0); else knightImage.color = Color.white;

        //display if Player can be ready (change some Text or something...)
        //TODO
    }

    private bool IsSetupValid()
    {
        return horseToggleGroup.ChosenData != null && knightToggleGroup.ChosenData != null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class GenericToggleGroup<T, K> : MonoBehaviour where T : ScriptableObject where K : GenericToggleData<T>
{
    [System.Serializable]
    public class SOUnityEvent : UnityEvent<T> { }

    [SerializeField] protected SOUnityEvent onValueChanged;

    [SerializeField, ReadOnly] protected T _chosenData = null;
    public T ChosenData { get { return _chosenData; } protected set { _chosenData = value; onValueChanged.Invoke(_chosenData); } }

    protected ToggleGroup group;
    //private List<HorseToggleData> toggles = new List<HorseToggleData>();

    // ---------- Unity messages

    protected virtual void Awake()
    {
        group = GetComponent<ToggleGroup>();

        //TODO --- for reservations later
        /*toggles.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent<HorseToggleData>(out HorseToggleData data))
                toggles.Add(data);
        }*/
    }

    // ---------- public methods

    //TODO - It currently sets the selection "at random" if the selectedData==null... It should not...
    public virtual void Init(T selectedData=null)
    {
        //Find the first toggle to select
        Toggle toggle = null;
        if (selectedData == null)//set data to the first ON toggle
        {
            toggle = group.GetFirstActiveToggle();
            Debug.Assert(toggle != null, "ASSERTION FAILED: The FirstActiveToggle is null", gameObject);
        }
        else//try to get the toggle with the requested data
        {
            //disable all toggles
            group.SetAllTogglesOff(false);
            //search through the toggles in a group
            //TODO/WAR: This does not check if the toggles found belong to the current group, should fix in the future...
            K[] toggles = GetComponentsInChildren<K>();
            foreach (K togData in toggles)
            {
                if (togData.data == selectedData)//found a toggle with matching data
                {
                    toggle = togData.toggle;
                    toggle.SetIsOnWithoutNotify(true);
                    break;
                }
            }
            Debug.Assert(toggle != null, "ASSERTION FAILED: The requested data has not been found in the ToggleGroup", gameObject);
        }

        if (toggle.isOn)
            _chosenData = toggle.GetComponent<K>().data;
        else
            Debug.LogWarning("WAR: The first active toggle is not on", gameObject);

        //TODO --- for reservations later
        /*//enable the first available Horse
        group.SetAllTogglesOff();
        if (toggles.Count > 0)
        {
            if (toggles[0].toggle.isOn)
                toggles[0].toggle.SetIsOnWithoutNotify(false);//first set to false to force Toggle callback if true in the first place

            toggles[0].toggle.isOn = true;
        }

        onValueChanged.Invoke(_chosenData);*/
    }

    //received from our own toggles (no networking here, just setting the SO data and invoking an event)
    public virtual void OnToggleValueChanged(K toggleClicked)
    {
        if (toggleClicked.toggle.isOn)//we only care about on messages, we ignore all off messages
        {
            if (ChosenData != toggleClicked.data)
            {
                ChosenData = toggleClicked.data;//assign new data
            }
            else//if a toggle with the same data is clicked, we set it to null and switch all toggles off
            {
                ChosenData = null;
                group.SetAllTogglesOff(false);
            }
        }
    }
}

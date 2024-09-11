using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HorseToggleData : MonoBehaviour
{
    [SerializeField] private Horse _data;
    public Horse data { get { return _data; } }

    private Toggle _toggle;
    public Toggle toggle { get { return _toggle; } }

    // ---------- Unity messages

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }
}

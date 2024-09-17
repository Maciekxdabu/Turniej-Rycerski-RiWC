using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class GenericToggleData<T> : MonoBehaviour where T : ScriptableObject
{
    [SerializeField] protected T _data;
    public T data { get { return _data; } }

    protected Toggle _toggle;
    public Toggle toggle { get { return _toggle; } }

    // ---------- Unity messages

    protected virtual void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }
}

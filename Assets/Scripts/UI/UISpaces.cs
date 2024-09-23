using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpaces : MonoBehaviour
{
    [SerializeField] private Transform[] _UItransforms;
    public Transform[] UItransforms { get { return _UItransforms; } }

    //singleton
    private static UISpaces instance;
    public static UISpaces Instance { get { return instance; } }

    // ---------- Unity messages

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }
}

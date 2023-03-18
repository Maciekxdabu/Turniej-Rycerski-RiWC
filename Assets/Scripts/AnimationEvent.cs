using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField] UnityEvent[] eventsToRun;

    public void RunEvent(int eventIndex)
    {
        if (eventIndex >= 0 && eventIndex < eventsToRun.Length)
            eventsToRun[eventIndex].Invoke();
        else
            Debug.LogWarning("WAR: Incorrect event index in animation", gameObject);
    }
}

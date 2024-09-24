using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayer : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    // ---------- public methods

    public void UpdateHealth(float value)
    {
        healthSlider.value = value;
    }
}

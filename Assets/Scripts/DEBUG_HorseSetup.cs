using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DEBUG_HorseSetup : MonoBehaviour
{
    [SerializeField] private TMP_InputField accelField;
    [SerializeField] private TMP_InputField maxSpeedField;
    [SerializeField] private TMP_InputField damagingSpeedField;
    [SerializeField] private TMP_InputField strengthField;
    [SerializeField] private Horse horseData;

    // ---------- Unity messages

    private void Start()
    {
        accelField.text = horseData.acceleration.ToString();
        maxSpeedField.text = horseData.maxSpeed.ToString();
        damagingSpeedField.text = horseData.minDamagingSpeed.ToString();
        strengthField.text = horseData.strength.ToString();
    }

    // ---------- public methods

    //!!!
    //WARNING: CHANGE HORSE SETTERS TO PRIVATE WHEN DELETING THIS
    //!!!

    public void UpdateHorseValues()
    {
        if (float.TryParse(accelField.text, out float value1))
            horseData.acceleration = value1;
        if (float.TryParse(maxSpeedField.text, out float value2))
            horseData.maxSpeed = value2;
        if (float.TryParse(damagingSpeedField.text, out float value3))
            horseData.minDamagingSpeed = value3;
        if (float.TryParse(strengthField.text, out float value4))
            horseData.strength = value4;
    }
}

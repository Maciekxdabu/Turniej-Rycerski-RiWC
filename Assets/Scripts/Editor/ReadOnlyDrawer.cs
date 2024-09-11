using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    private bool guiState = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        guiState = GUI.enabled;
        GUI.enabled = false;

        EditorGUI.PropertyField(position, property, label);

        GUI.enabled = guiState;
    }
}

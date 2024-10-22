using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapController))]
public class MapEditor : Editor
{
    MapController mapRef = null;

    private void OnEnable()
    {
        mapRef = target as MapController;
    }

    private void OnDisable()
    {
        mapRef = null;
    }

    private void OnSceneGUI()
    {
        SerializedProperty prop = serializedObject.FindProperty("betweenLines");
        for (int i=0; i<prop.arraySize; i++)//one between Linein a map
        {
            MapController.BetweenLine bLine = prop.GetArrayElementAtIndex(i).managedReferenceValue as MapController.BetweenLine;
            foreach (Vector2 val in bLine.passages)// on passage in a betweenLine
            {

            }
        }
    }
}

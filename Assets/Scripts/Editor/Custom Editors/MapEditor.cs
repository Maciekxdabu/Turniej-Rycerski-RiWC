using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Allows to edit the Map Data (MapDefinition) in-Scene by selecting a MapEvaluator <br></br>
/// Warning! It directly changes tha associated ScriptableObject!
/// </summary>
[CustomEditor(typeof(MapEvaluator))]
public class MapEditor : Editor
{
    MapEvaluator mapRef = null;

    private void OnEnable()
    {
        mapRef = target as MapEvaluator;
    }

    private void OnDisable()
    {
        mapRef = null;
    }

    private void OnSceneGUI()
    {
        MapDefinition mapData = mapRef.mapData;
        if (mapData == null)
            return;

        SerializedObject mapObject = new SerializedObject(mapData);

        Handles.color = Color.red;

        SerializedProperty prop = mapObject.FindProperty("betweenLines");
        for (int i=0; i<prop.arraySize; i++)// one betweenLine in a map
        {
            SerializedProperty passages = prop.GetArrayElementAtIndex(i).FindPropertyRelative("passages");
            float y = mapRef.GetBetweenLineHeight(i);

            for (int j=0; j<passages.arraySize; j++)// one passage in a betweenLine
            {
                Vector2 val = passages.GetArrayElementAtIndex(j).vector2Value;

                Vector3 p1 = new Vector3(mapRef.NormToUnit(val.x), y, 0);
                Vector3 p2 = new Vector3(mapRef.NormToUnit(val.y), y, 0);

                EditorGUI.BeginChangeCheck();
                Vector3 p11 = Handles.Slider(p1, Vector3.right, HandleUtility.GetHandleSize(p1) * 0.2f, Handles.SphereHandleCap, 0.1f);
                Vector3 p22 = Handles.Slider(p2, Vector3.right, HandleUtility.GetHandleSize(p1) * 0.2f, Handles.SphereHandleCap, 0.1f);
                if (EditorGUI.EndChangeCheck())
                {
                    val.x = mapRef.UnitToNorm(p11.x);
                    val.y = mapRef.UnitToNorm(p22.x);

                    passages.GetArrayElementAtIndex(j).vector2Value = val;
                }

                Handles.DrawDottedLine(p1, p2, 10);
            }
        }

        mapObject.ApplyModifiedProperties();
    }
}

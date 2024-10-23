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
        Handles.color = Color.red;

        SerializedProperty prop = serializedObject.FindProperty("betweenLines");
        for (int i=0; i<prop.arraySize; i++)// one between Linein a map
        {
            SerializedProperty passages = prop.GetArrayElementAtIndex(i).FindPropertyRelative("passages");
            for (int j=0; j<passages.arraySize; j++)// one passage in a betweenLine
            {
                Vector2 val = passages.GetArrayElementAtIndex(j).vector2Value;

                Vector3 p1 = new Vector3(mapRef.LenToUnit(val.x), -i, 0);
                Vector3 p2 = new Vector3(mapRef.LenToUnit(val.y), -i, 0);

                EditorGUI.BeginChangeCheck();
                Vector3 p11 = Handles.Slider(p1, Vector3.right, HandleUtility.GetHandleSize(p1) * 0.2f, Handles.SphereHandleCap, 0.1f);
                Vector3 p22 = Handles.Slider(p2, Vector3.right, HandleUtility.GetHandleSize(p1) * 0.2f, Handles.SphereHandleCap, 0.1f);
                if (EditorGUI.EndChangeCheck())
                {
                    val.x = mapRef.UnitToLen(p11.x);
                    val.y = mapRef.UnitToLen(p22.x);

                    passages.GetArrayElementAtIndex(j).vector2Value = val;
                }

                Handles.DrawDottedLine(p1, p2, 10);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

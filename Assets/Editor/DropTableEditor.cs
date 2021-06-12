using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(DropTable))]
public class DropTableEditor : Editor
{
    private SerializedProperty table;
    private SerializedProperty max;


    private void OnEnable()
    {
        table = serializedObject.FindProperty("dropTable");
        max = serializedObject.FindProperty("totalDropChance");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Total drop chance: " + max.intValue);

        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        DrawTableData();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Element", GUILayout.Width(100)))
        {
            AddNewElement();
        }
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            CalculateMaxDropChances();
        }


        serializedObject.ApplyModifiedProperties();
    }

    private void AddNewElement()
    {
        int index = table.arraySize;
        table.InsertArrayElementAtIndex(index);
        table.GetArrayElementAtIndex(index).FindPropertyRelative("powerUp").objectReferenceValue = null;
        table.GetArrayElementAtIndex(index).FindPropertyRelative("dropChance").intValue = 0;
    }

    private void DrawTableData()
    {
        for (int i = 0; i < table.arraySize; i++)
        {
            SerializedProperty prop = table.GetArrayElementAtIndex(i);
            EditorGUILayout.LabelField("Power Up " + i);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("powerUp"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("dropChance"));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Drop Percentage: " + ((prop.FindPropertyRelative("dropChance").intValue / (float)max.intValue) * 100) + "%");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(50)))
            {
                table.DeleteArrayElementAtIndex(i);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                break;
            }

            EditorGUILayout.EndHorizontal();


            EditorGUI.indentLevel--;
        }
    }

    private void CalculateMaxDropChances()
    {
        int maxDrop = 0;
        for (int i = 0; i < table.arraySize; i++)
        {
            maxDrop += GetDropChance(i);
        }
        max.intValue = maxDrop;
    }

    private int GetDropChance(int i)
    {
        return table.GetArrayElementAtIndex(i).FindPropertyRelative("dropChance").intValue;
    }
}

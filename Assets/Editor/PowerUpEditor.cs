using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PowerUp_Scriptable))]
public class PowerUpEditor : Editor
{
    SerializedProperty power;
    SerializedProperty iterator;
    private void OnEnable()
    {
        power = serializedObject.FindProperty("power");
        if ((target as PowerUp_Scriptable).power == null)
        {
            CreatePower();
        }
    }

    private void CreatePower()
    {
        switch ((PowerUpType)serializedObject.FindProperty("type").intValue)
        {
            case PowerUpType.EXTRA_SHOT:
                power.managedReferenceValue = new PowerUp_ExtraShot();
                break;
            case PowerUpType.SHOT_MODIFIER:
                break;
            case PowerUpType.STAT_MODIFIER:
                break;
            case PowerUpType.ELEMENTAL_TYPING:
                break;
            case PowerUpType.FAMILIAR:
                break;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        int oldValue = serializedObject.FindProperty("type").intValue;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        if (EditorGUI.EndChangeCheck())
        {
            if (EditorUtility.DisplayDialog("Warning", "Changing the type will delete all values!\nContinue?", "Yes", "No :("))
            {
                CreatePower();
            }
            else
            {
                serializedObject.FindProperty("type").intValue = oldValue;
            }
        }
        iterator = serializedObject.FindProperty("power");
        iterator.Next(true);
        do
        {
            EditorGUILayout.PropertyField(iterator);
        } while (iterator.Next(false));

        serializedObject.ApplyModifiedProperties();
    }
}

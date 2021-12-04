using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DreamStatueCombat))]
public class DreamStatueCombatEditor : EnemyCombatEditor
{
    //This script builds on top of EnemyCombatEditors functionality
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //The separators just add some space between attributes in the inspector.
        EditorGUILayout.Separator();

        //Label width changes the space between the label and the property. Applies to every further label until it's changed again.
        EditorGUIUtility.labelWidth = 100f;
        
        //This looks for a certain property in DreamStatueCombat, puts it in the inspector and gives it a label.
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lightning"), new GUIContent("Lightning Prefab: "));

        EditorGUILayout.Separator();

        //This creates a bold styled label in it's own row.
        EditorGUILayout.LabelField("Phase 1 Lightnings", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("telegraphDuraionPhaseOne"), new GUIContent("Telegraph Duration: "));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("noOfLightningsInPhaseOne"), new GUIContent("Number: "));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenLightningsPhaseOne"), new GUIContent("Time Between: "));
        
        EditorGUILayout.LabelField("Phase 2 Lightnings", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("telegraphDuraionPhaseTwo"), new GUIContent("Telegraph Duration: "));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("noOfLightningsInPhaseTwo"), new GUIContent("Number: "));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenLightningsPhaseTwo"), new GUIContent("Time Between: "));

		serializedObject.ApplyModifiedProperties();
    }
}

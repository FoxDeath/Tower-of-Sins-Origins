using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DreamStatueMovement))]
public class DreamStatueMovementEditor : Editor
{
    #region Attributes
    private bool generalFoldout = false;
    private bool phaseOneChargeFoldout = false;
    private bool phaseTwoChargeFoldout = false;
    private bool pathUpdateFoldout = false;
    #endregion

    #region MonoBehaviour Methods
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Label width changes the space between the label and the property. Applies to every further label until it's changed again.
        EditorGUIUtility.labelWidth = 140f;

        //This is where the expandable list is created for the attack elements.
        generalFoldout = EditorGUILayout.Foldout(generalFoldout, new GUIContent("General"), EditorStyles.boldFont);

        if(generalFoldout)
        {
            //This looks for a certain property in DreamStatueMovement, puts it in the inspector and gives it a label.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"), new GUIContent("Speed: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionDamage"), new GUIContent("Collision Damage: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeDuration"), new GUIContent("Charge  Duration: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("cheeseFromAboveMaxTime"), new GUIContent("Cheese Timer Max: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("teleportBuildUpTime"), new GUIContent("Teleport Build Up Duration: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("aoeAttack"), new GUIContent("AOE Prefab: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("constantRotationDefault"), new GUIContent("Default Rotation: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isSpriteFacingRight"), new GUIContent("Facing Right: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("pathUpdateDistance"), new GUIContent("Path Update Distance: "));
        }
        
        //The separators just add some space between attributes in the inspector.
        EditorGUILayout.Separator();

        phaseOneChargeFoldout = EditorGUILayout.Foldout(phaseOneChargeFoldout, new GUIContent("Phase 1 Charge"), EditorStyles.boldFont);
        
        if(phaseOneChargeFoldout)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseOneMaxCharges"), new GUIContent("Max Charges: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseOneChargeFrequency"), new GUIContent("Frequency: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseOneTimeBetweenCharges"), new GUIContent("Time Between: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseOneChargeTelegraphDuration"), new GUIContent("Telegraph Duration: "));
        }
        
        EditorGUILayout.Separator();

        phaseTwoChargeFoldout = EditorGUILayout.Foldout(phaseTwoChargeFoldout, new GUIContent("Phase 2 Charge"), EditorStyles.boldFont);

        if(phaseTwoChargeFoldout)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseTwoMaxCharges"), new GUIContent("Max Charges: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseTwoChargeFrequency"), new GUIContent("Frequency: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseTwoTimeBetweenCharges"), new GUIContent("Time Between: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("phaseTwoChargeTelegraphDuration"), new GUIContent("Telegraph Duration: "));
        }

        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
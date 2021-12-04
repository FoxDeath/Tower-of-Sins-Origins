using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AirEnemyMovement))]
public class AirEnemyMovementEditor : Editor
{
    #region Attributes
    private bool moveFoldout = false;
    private bool idleFoldout = false;
    private bool generalFoldout = false;
    private bool pathUpdateFoldout = false;
    #endregion

    #region MonoBehaviour Methods
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Label width changes the space between the label and the property. Applies to every further label until it's changed again.
        EditorGUIUtility.labelWidth = 140f;

        AirEnemyMovement.MoveOptions currentMoveOption = (AirEnemyMovement.MoveOptions)serializedObject.FindProperty("currentMoveOption").enumValueIndex;
                
        AirEnemyMovement.IdleOptions currentIdleOption = (AirEnemyMovement.IdleOptions)serializedObject.FindProperty("currentIdleOption").enumValueIndex;

        //This is where the expandable list is created for the attack elements.
        moveFoldout = EditorGUILayout.Foldout(moveFoldout, new GUIContent("Move"), EditorStyles.boldFont);

        if(moveFoldout)
        {
            //This looks for a certain property in AirEnemyMovement, puts it in the inspector and gives it a label.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentMoveOption"), new GUIContent("Current Move Option: "));

            //These switches determine what attributes should be seeable and when.
            switch(currentMoveOption)
            {
                case AirEnemyMovement.MoveOptions.Follow:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAngleOfAttack"), new GUIContent("Current Angle of Attack: "));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("isKeepingDistance"), new GUIContent("Keeping Distance: "));

                    bool isKeepingDistance = serializedObject.FindProperty("isKeepingDistance").boolValue;

                    bool isRetreatingWhenTooClose = serializedObject.FindProperty("isRetreatingWhenTooClose").boolValue;

                    if(isKeepingDistance)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToKeep"), new GUIContent("Distance  to Keep: "));
                        
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("isRetreatingWhenTooClose"), new GUIContent("Retreats When Too Close: "));

                        if(isRetreatingWhenTooClose)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("backwardsSpeedModifier"), new GUIContent("Backwards Speed Modifier: "));
                        }
                    }
                break;

                case AirEnemyMovement.MoveOptions.Charge:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAngleOfAttack"), new GUIContent("Current Angle of Attack: "));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeDuration"), new GUIContent("Charge Duration: "));
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeFrequency"), new GUIContent("Charge Frequency: "));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeTelegraphDuration"), new GUIContent("Charge Telegraph Duration: "));
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("isChargeParriable"), new GUIContent("Charge Parriable: "));
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("isClearingWayForCharge"), new GUIContent("Clear Way Before Charge: "));

                    bool isClearingWayForCharge = serializedObject.FindProperty("isClearingWayForCharge").boolValue;

                    if(isClearingWayForCharge)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToClear"), new GUIContent("Distance to Clear: "));
                    }
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("isFollowingBetweenMoves"), new GUIContent("Follow Between Moves: "));

                    bool isFollowingBetweenMoves = serializedObject.FindProperty("isFollowingBetweenMoves").boolValue;

                    isKeepingDistance = serializedObject.FindProperty("isKeepingDistance").boolValue;

                    if(isFollowingBetweenMoves)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("isKeepingDistance"), new GUIContent("Keeping Distance: "));

                        if(isKeepingDistance)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToKeep"), new GUIContent("Distance  to Keep: "));
                            
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("isRetreatingWhenTooClose"), new GUIContent("Retreats When Too Close: "));
                        }
                    }

                    isRetreatingWhenTooClose = serializedObject.FindProperty("isRetreatingWhenTooClose").boolValue;

                    if(isClearingWayForCharge || (isFollowingBetweenMoves && isRetreatingWhenTooClose))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("backwardsSpeedModifier"), new GUIContent("Backwards Speed Modifier: "));
                    }
                break;
            }
        }
        
        //The separators just add some space between attributes in the inspector.
        EditorGUILayout.Separator();

        idleFoldout = EditorGUILayout.Foldout(idleFoldout, new GUIContent("Idle"), EditorStyles.boldFont);

        if(idleFoldout)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentIdleOption"), new GUIContent("Current Idle Option: "));

            switch(currentIdleOption)
            {
                case AirEnemyMovement.IdleOptions.Patrol:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("patrolSpeed"), new GUIContent("Patrol Speed: "));
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("isRestrictedToArea"), new GUIContent("Restricted to Area: "));

                    bool isRestrictedToArea = serializedObject.FindProperty("isRestrictedToArea").boolValue;

                    if(isRestrictedToArea)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("patrolOffset"), new GUIContent("Patrol Offset: "));
                    }
                break;
                
                case AirEnemyMovement.IdleOptions.Asleep:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("sleepTimer"), new GUIContent("Sleep Timer: "));
                break;
            }
        }

        EditorGUILayout.Separator();

        generalFoldout = EditorGUILayout.Foldout(generalFoldout, new GUIContent("General"), EditorStyles.boldFont);

        if(generalFoldout)
        {
            if(currentMoveOption == AirEnemyMovement.MoveOptions.Charge || currentMoveOption == AirEnemyMovement.MoveOptions.Follow
            || currentIdleOption == AirEnemyMovement.IdleOptions.Patrol)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"), new GUIContent("Speed: "));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("pathUpdateDistance"), new GUIContent("Path Update Distance: "));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionDamage"), new GUIContent("Collision Damage: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isBiggerThanPlayer"), new GUIContent("Bigger Than Player: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isSpriteFacingRight"), new GUIContent("Facing Right: "));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isRotatingConstantly"), new GUIContent("Always Rotating: "));

            bool isRotatingConstantly = serializedObject.FindProperty("isRotatingConstantly").boolValue;

            if(isRotatingConstantly)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("constantRotationDefault"), new GUIContent("Default Rotation: "));
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}

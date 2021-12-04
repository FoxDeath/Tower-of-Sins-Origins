using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroundEnemyMovement))]
public class GroundEnemyMovementEditor : Editor
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

        GroundEnemyMovement.MoveOptions currentMoveOption = (GroundEnemyMovement.MoveOptions)serializedObject.FindProperty("currentMoveOption").enumValueIndex;
        
        GroundEnemyMovement.IdleOptions currentIdleOption = (GroundEnemyMovement.IdleOptions)serializedObject.FindProperty("currentIdleOption").enumValueIndex;
        
        if(currentIdleOption != GroundEnemyMovement.IdleOptions.CirclePlatform)
        {
            //This is where the expandable list is created for the attack elements.
            moveFoldout = EditorGUILayout.Foldout(moveFoldout, new GUIContent("Move"), EditorStyles.boldFont);

            if(moveFoldout)
            {
                //This looks for a certain property in GroundEnemyMovement, puts it in the inspector and gives it a label.
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentMoveOption"), new GUIContent("Current Move Option: "));
                
                //These switches determine what attributes should be seeable and when.
                switch(currentMoveOption)
                {
                    case GroundEnemyMovement.MoveOptions.Follow:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("isKeepingDistance"), new GUIContent("Keeping Distance: "));

                            bool isKeepingDistance = serializedObject.FindProperty("isKeepingDistance").boolValue;

                            if(isKeepingDistance)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToKeep"), new GUIContent("Distance  to Keep: "));
                                
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("isRetreatingWhenTooClose"), new GUIContent("Retreats When Too Close: "));

                                bool isRetreatingWhenTooClose = serializedObject.FindProperty("isRetreatingWhenTooClose").boolValue;

                                if(isRetreatingWhenTooClose)
                                {
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("backwardsSpeedModifier"), new GUIContent("Backwards Speed Modifier: "));
                                }
                            }
                    break;

                    case GroundEnemyMovement.MoveOptions.Charge:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeDuration"), new GUIContent("Charge Duration: "));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeFrequency"), new GUIContent("Charge Frequency: "));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeTelegraphDuration"), new GUIContent("Charge Telegraph Duration: "));
                        
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("isChargeParriable"), new GUIContent("Charge Parriable: "));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("isFollowingBetweenMoves"), new GUIContent("Follow Between Moves: "));

                        bool isFollowingBetweenMoves = serializedObject.FindProperty("isFollowingBetweenMoves").boolValue;

                        if(isFollowingBetweenMoves)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("isKeepingDistance"), new GUIContent("Keeping Distance: "));

                            isKeepingDistance = serializedObject.FindProperty("isKeepingDistance").boolValue;

                            if(isKeepingDistance)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToKeep"), new GUIContent("Distance to Keep: "));
                                
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("isRetreatingWhenTooClose"), new GUIContent("Retreats When Too Close: "));

                                bool isRetreatingWhenTooClose = serializedObject.FindProperty("isRetreatingWhenTooClose").boolValue;

                                if(isRetreatingWhenTooClose)
                                {
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("backwardsSpeedModifier"), new GUIContent("Backwards Speed Modifier: "));
                                }
                            }
                        }
                    break;

                    case GroundEnemyMovement.MoveOptions.Jump:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpSpeed"), new GUIContent("Jump Speed: "));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpDuration"), new GUIContent("Jump Duration: "));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpFrequency"), new GUIContent("Jump Frequency: "));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("isFollowingBetweenMoves"), new GUIContent("Following Between Moves: "));

                        isFollowingBetweenMoves = serializedObject.FindProperty("isFollowingBetweenMoves").boolValue;

                        if(isFollowingBetweenMoves)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("isKeepingDistance"), new GUIContent("Keeping Distance: "));

                            isKeepingDistance = serializedObject.FindProperty("isKeepingDistance").boolValue;

                            if(isKeepingDistance)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToKeep"), new GUIContent("Distance  to Keep: "));
                                
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("isRetreatingWhenTooClose"), new GUIContent("Retreats When Too Close: "));

                                bool isRetreatingWhenTooClose = serializedObject.FindProperty("isRetreatingWhenTooClose").boolValue;

                                if(isRetreatingWhenTooClose)
                                {
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("backwardsSpeedModifier"), new GUIContent("Backwards Speed Modifier: "));
                                }
                            }
                        }
                    break;
                }
            }
        }

        //The separators just add some space between attributes in the inspector.
        EditorGUILayout.Separator();

        //This is where the expandable list is created for the attack elements.
        idleFoldout = EditorGUILayout.Foldout(idleFoldout, new GUIContent("Idle"), EditorStyles.boldFont);

        if(idleFoldout)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentIdleOption"), new GUIContent("Current Idle Option: "));

            switch(currentIdleOption)
            {
                case GroundEnemyMovement.IdleOptions.Patrol:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("patrolSpeed"), new GUIContent("Patrol Speed: "));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("patrolOffset"), new GUIContent("Patrol Offset: "));
                break;
                
                case GroundEnemyMovement.IdleOptions.Asleep:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("sleepTimer"), new GUIContent("Sleep Timer: "));
                break;
                
                case GroundEnemyMovement.IdleOptions.CirclePlatform:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("isGoingRight"), new GUIContent("Going Right: "));
                break;
            }
        }
        
        EditorGUILayout.Separator();

        //This is where the expandable list is created for the attack elements.
        generalFoldout = EditorGUILayout.Foldout(generalFoldout, new GUIContent("General"), EditorStyles.boldFont);

        if(generalFoldout)
        {
            bool isCheckingForCheeseFromAbove = serializedObject.FindProperty("isCheckingForCheeseFromAbove").boolValue;
                
            if(currentMoveOption == GroundEnemyMovement.MoveOptions.Charge || currentMoveOption == GroundEnemyMovement.MoveOptions.Follow
            || currentMoveOption == GroundEnemyMovement.MoveOptions.Jump || currentIdleOption == GroundEnemyMovement.IdleOptions.Patrol
            || currentIdleOption == GroundEnemyMovement.IdleOptions.CirclePlatform || isCheckingForCheeseFromAbove)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"), new GUIContent("Speed: "));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionDamage"), new GUIContent("Collision Damage: "));

            if(currentIdleOption != GroundEnemyMovement.IdleOptions.CirclePlatform)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isBiggerThanPlayer"), new GUIContent("Bigger Than Player: "));
            }

            if(currentMoveOption == GroundEnemyMovement.MoveOptions.Charge || currentMoveOption == GroundEnemyMovement.MoveOptions.Follow
            || currentMoveOption == GroundEnemyMovement.MoveOptions.Jump || currentIdleOption == GroundEnemyMovement.IdleOptions.Patrol)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isRestrictedToArea"), new GUIContent("Restricted to Area: "));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("pathUpdateDistance"), new GUIContent("Path Update Distance: "));
            }

            if(currentIdleOption != GroundEnemyMovement.IdleOptions.CirclePlatform && currentMoveOption != GroundEnemyMovement.MoveOptions.Jump)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isCheckingForCheeseFromAbove"), new GUIContent("Cheese Check From Above: "));

                if(isCheckingForCheeseFromAbove)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("cheeseFromAboveMaxTime"), new GUIContent("Cheese Check Timer: "));
                }
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isSpriteFacingRight"), new GUIContent("Facing Right: "));
        }

        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
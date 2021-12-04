using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(EnemyCombat))]
public class EnemyCombatEditor : Editor
{
    private SerializedProperty phasesList;

    //The phases are shown in a reorderable list. That's what lets thinkgs be draggable and stuff.
	private ReorderableList phases;

    //We use this dictionary to store the individual attack elements. It's needed to be able to edit them.
    private Dictionary<string, ReorderableList> allAttacksDict = new Dictionary<string, ReorderableList>();
	
	private void OnEnable()
    {
        phasesList = serializedObject.FindProperty("phases");

		phases = new ReorderableList(serializedObject, phasesList, true, true, true, true)
        {
            //This is just a callback for creating the elements header.
            drawHeaderCallback = (rect) =>
            {
                //This creates a bold styled label in it's own row.
                EditorGUI.LabelField(rect, "Combat Phases");
            },

            //This callback defines how a certain element is represented in the inspector.
            drawElementCallback = (phaseRect, phaseIndex, phaseIsActive, phaseIsFocused) =>
            {
                var phaseElement = phases.serializedProperty.GetArrayElementAtIndex(phaseIndex);

                var allAttacksList = phaseElement.FindPropertyRelative("allAttacks");

                string listKey = phaseElement.propertyPath;

                ReorderableList allAttacksReorderable;

                //Check if the attack element's reorderable list exists. If yes, just get it.
                if(allAttacksDict.ContainsKey(listKey))
                {
                    allAttacksReorderable = allAttacksDict[listKey];
                }
                //If not, create a new one and store it.
                else
                {
                    //The attacks are also shown in a reorderable list, just like the phases.
                    allAttacksReorderable = new ReorderableList(phaseElement.serializedObject, allAttacksList, true, true, true, true)
                    {
                        drawHeaderCallback = attackRect =>
                        {
                            EditorGUI.LabelField(attackRect, "All Attacks");
                        },

                        drawElementCallback = (attackRect, attackIndex, attackIsActive, attackIsFocused) =>
                        {
                            var attackElement = allAttacksList.GetArrayElementAtIndex(attackIndex);

                            //Label width changes the space between the label and the property. Applies to every further label until it's changed again.
                            EditorGUIUtility.labelWidth = 40f;

                            //This looks for a certain property in DreamStatueCombat, puts it in the inspector and gives it a label.
                            EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y, attackRect.width, EditorGUIUtility.singleLineHeight),
                            attackElement.FindPropertyRelative("name"), new GUIContent("Name: "));

                            EditorGUIUtility.labelWidth = 105f;

                            //This is where the expandable list is created for the attack elements.
                            attackElement.isExpanded = EditorGUI.Foldout(new Rect(attackRect.x + 12f, attackRect.y + EditorGUIUtility.singleLineHeight, attackRect.width, EditorGUIUtility.singleLineHeight), attackElement.isExpanded, "Attack Details");

                            if(attackElement.isExpanded)
                            {
                                float currentLine = 2f;
                                
                                bool isInCycle = attackElement.FindPropertyRelative("isInCycle").boolValue;

                                EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y + (currentLine * EditorGUIUtility.singleLineHeight), attackRect.width, EditorGUIUtility.singleLineHeight),
                                attackElement.FindPropertyRelative("animationNumber"), new GUIContent("Animation No: "));

                                currentLine++;
                                
                                EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y + (currentLine * EditorGUIUtility.singleLineHeight), attackRect.width, EditorGUIUtility.singleLineHeight),
                                attackElement.FindPropertyRelative("range"), new GUIContent("Range: "));

                                currentLine++;
                                
                                EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y + (currentLine * EditorGUIUtility.singleLineHeight), attackRect.width, EditorGUIUtility.singleLineHeight),
                                attackElement.FindPropertyRelative("damage"), new GUIContent("Damage: "));

                                currentLine++;
                                
                                if(isInCycle)
                                {
                                    EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y + (currentLine * EditorGUIUtility.singleLineHeight), attackRect.width, EditorGUIUtility.singleLineHeight),
                                    attackElement.FindPropertyRelative("buildUpDuration"), new GUIContent("Build Up Duration: "));

                                    currentLine++;
                                }
                                
                                EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y + (currentLine * EditorGUIUtility.singleLineHeight), attackRect.width, EditorGUIUtility.singleLineHeight),
                                attackElement.FindPropertyRelative("attackDuration"), new GUIContent("Attack Duration: "));

                                currentLine++;
                                
                                EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y + (currentLine * EditorGUIUtility.singleLineHeight), attackRect.width, EditorGUIUtility.singleLineHeight),
                                attackElement.FindPropertyRelative("isInCycle"), new GUIContent("Part of the Cycle: "));

                                currentLine++;
                                
                                EditorGUI.PropertyField(new Rect(attackRect.x, attackRect.y + (currentLine * EditorGUIUtility.singleLineHeight), attackRect.width, EditorGUIUtility.singleLineHeight),
                                attackElement.FindPropertyRelative("isParriable"), new GUIContent("Parriable: "));

                                currentLine++;
                            }
                        },

                        //This callback defines how tall each element of a certain type should be.
                        elementHeightCallback = attackIndex => 
                        {
                            var attackElement = allAttacksList.GetArrayElementAtIndex(attackIndex);

                            if(attackElement.isExpanded)
                            {
                                int lines = 8;

                                if(attackElement.FindPropertyRelative("isInCycle").boolValue)
                                {
                                    lines++;
                                }

                                return(lines * EditorGUIUtility.singleLineHeight);
                            }
                            else
                            {
                                return(2 * EditorGUIUtility.singleLineHeight);
                            }
                        }
                    };
                    
                    allAttacksDict[listKey] = allAttacksReorderable;
                }

                EditorGUIUtility.labelWidth = 40f;

                allAttacksReorderable.DoList(new Rect(phaseRect.x, phaseRect.y + (2f * EditorGUIUtility.singleLineHeight) + 13f, phaseRect.width, phaseRect.height / 3f));

                EditorGUIUtility.labelWidth = 40f;

                EditorGUI.PropertyField(new Rect(phaseRect.x , phaseRect.y + 3f, phaseRect.width, EditorGUIUtility.singleLineHeight),
                phaseElement.FindPropertyRelative("name"), new GUIContent("Name: "));

                EditorGUIUtility.labelWidth = 100f;
                
                EditorGUI.PropertyField(new Rect(phaseRect.x, phaseRect.y + EditorGUIUtility.singleLineHeight + 8f, phaseRect.width, EditorGUIUtility.singleLineHeight),
                phaseElement.FindPropertyRelative("triggerHealthPercentage"), new GUIContent("Trigger Health %: "));
            },

            elementHeightCallback = index =>
            {
                var phaseElement = phasesList.GetArrayElementAtIndex(index);

                var allAttacksList = phaseElement.FindPropertyRelative("allAttacks");

                int lines = 0;

                for(int i = 0; i < allAttacksList.arraySize; i++)
                {
                    if(allAttacksList.GetArrayElementAtIndex(i).isExpanded)
                    {
                        lines += 8;

                        if(allAttacksList.GetArrayElementAtIndex(i).FindPropertyRelative("isInCycle").boolValue)
                        {
                            lines++;
                        }
                    }
                    else
                    {
                        lines += 2;
                    }
                }

                if(allAttacksList.arraySize <= 0)
                {
                    lines++;
                }

                return (lines * EditorGUIUtility.singleLineHeight + (6f * EditorGUIUtility.singleLineHeight));
            }
        };
	}
	
	public override void OnInspectorGUI()
    {
		serializedObject.Update();

		phases.DoLayoutList();

        EditorGUIUtility.labelWidth = 120f;
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenOutOfCycleAttacks"), new GUIContent("Out of Cycle Delay: "));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outOfCycleAttackResetDuration"), new GUIContent("Out of Cycle Reset Duration: "));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isInRandomOrder"), new GUIContent("Random Order: "));

		serializedObject.ApplyModifiedProperties();
	}
}

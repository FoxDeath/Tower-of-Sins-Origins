using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TOS.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        #region Attributes
        [TextArea(10,15)] [SerializeField] string text;

        [SerializeField] List<string> children = new List<string>();

        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);
        
        [SerializeField] bool isAQuest;
        #endregion

        #region Getters
        public Rect GetRect()
        {
            return rect;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }
        #endregion

        #region Setters
        #if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Moved Dialogue Node.");

            rect.position = newPosition;

            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if(newText != text)
            {
                Undo.RecordObject(this, "Updated Dialogue Text.");

                text = newText;

                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Added Dialogue Link.");

            children.Add(childID);

            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Removed Dialogue Link.");

            children.Remove(childID);

            EditorUtility.SetDirty(this);
        }
        #endif
        #endregion
    }
}

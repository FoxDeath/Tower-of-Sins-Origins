using UnityEngine;
using UnityEditor;
using System.IO;

namespace TOS.Dialogue.Editor
{
    //This class solves some Unity bug when renaming a scriptable object. Don't worry about it.
    public class DialogueModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            Dialogue dialogue = AssetDatabase.LoadMainAssetAtPath(sourcePath) as Dialogue;

            if(!dialogue)
            {
                return AssetMoveResult.DidNotMove;
            }

            if(Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath))
            {
                return AssetMoveResult.DidNotMove;
            }

            dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);

            return AssetMoveResult.DidNotMove;
        }
    }
}

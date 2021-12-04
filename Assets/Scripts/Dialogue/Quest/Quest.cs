using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 0)]
public class Quest : ScriptableObject
{
    [SerializeField] string[] tasks;

    public IEnumerable<string> GetTasks()
    {
        yield return "Task 1";
    }
}

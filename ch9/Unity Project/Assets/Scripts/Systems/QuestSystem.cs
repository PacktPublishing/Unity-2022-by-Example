using UnityEngine;
using System.Collections.Generic;

//[DefaultExecutionOrder(-500)]
public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }

    private Dictionary<string, bool> _quests = new();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    
    public void StartQuest(string questName)
    {
        if (!_quests.ContainsKey(questName))
            _quests.Add(questName, false);
    }

    public void CompleteQuest(string questName)
    {
        if (_quests.ContainsKey(questName))
            _quests[questName] = true;
    }

    public bool IsQuestComplete(string questName)
    {
        if (_quests.TryGetValue(questName, out bool status))
            return status;

        return false;
    }
}
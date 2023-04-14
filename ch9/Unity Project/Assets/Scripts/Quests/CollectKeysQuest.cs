using UnityEngine;

public class CollectKeysQuest : QuestBase
{
    [SerializeField] private int _numKeysRequired = 3;
    private int _keysCollected = 0;


    protected override void AddListeners()
    {
        base.AddListeners();
        EventSystem.Instance.AddListener<bool>(
            EventConstants.OnKeyCollected, KeyCollected);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        EventSystem.Instance.RemoveListener<bool>(
            EventConstants.OnKeyCollected, KeyCollected);
    }

    private void KeyCollected(bool arg0)
    {
        _keysCollected++;

        if (_keysCollected >= _numKeysRequired)
        {
            QuestSystem.Instance.CompleteQuest(QuestName.ToString());
            EventSystem.Instance.TriggerEvent(EventConstants.OnQuestCompleted, QuestName.ToString());
        }
    }
}
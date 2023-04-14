using UnityEngine;

public abstract class QuestBase : MonoBehaviour
{
    public QuestNames QuestName => _questName;
    [SerializeField]
    private QuestNames _questName;

    
    private void OnEnable() => AddListeners();

    private void OnDisable() => RemoveListeners();


    public virtual void StartQuest()
        => QuestSystem.Instance.StartQuest(QuestName.ToString());

    protected virtual void AddListeners()
        => EventSystem.Instance.AddListener<string>(
            EventConstants.OnQuestCompleted, QuestCompleted);

    protected virtual void RemoveListeners()
        => EventSystem.Instance.RemoveListener<string>(
            EventConstants.OnQuestCompleted, QuestCompleted);

    protected virtual void QuestCompleted(string questName)
        => Debug.Log($"Quest '{questName}' completed!");
}
using UnityEngine;
using UnityEngine.Events;

public class QuestHasCompleted : MonoBehaviour
{
    public QuestNames QuestName;
    public UnityEvent OnQuestComplete;
    public UnityEvent OnQuestIncomplete;


    public void CheckQuestComplete()
    {
        if (QuestSystem.Instance.IsQuestComplete(QuestName.ToString()))
        {
            OnQuestComplete?.Invoke();
            return;
        }

        OnQuestIncomplete?.Invoke();
    }
}

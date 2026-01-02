using DialogueEditor;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private NPCConversation _startConvo;
    [SerializeField] private NPCConversation _failedConvo;
    [SerializeField] private NPCConversation _powerConvo;
    [SerializeField] private NPCConversation _powerCompletedConvo;

    private bool _finishedFuseBox;
    void Start()
    {
        ConversationManager.Instance.StartConversation(_startConvo);
    }

    public void CompletedMiniGame(Component sender, object obj)
    {
        PowerRegulator power = sender as PowerRegulator;

        if (power != null && _finishedFuseBox)
        {
            ConversationManager.Instance.StartConversation(_powerCompletedConvo);
        }
        else if (power != null)
        {
            ConversationManager.Instance.StartConversation(_powerConvo);
        }
    }

    public void FailedMiniGame(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_failedConvo);
        _finishedFuseBox = false;
    }

    public void CompletedFuseBox(Component sender, object obj)
    {
        _finishedFuseBox = true;
    }
}

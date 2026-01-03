using DialogueEditor;
using System;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private NPCConversation _startConvo;
    [SerializeField] private NPCConversation _failedConvo;
    [SerializeField] private NPCConversation _powerConvo;
    [SerializeField] private NPCConversation _powerCompletedConvo;
    [SerializeField] private NPCConversation _failedPowerConvo;
    [SerializeField] private NPCConversation _powerHelpConvo;

    private bool _failedPower;
    void Start()
    {
        ConversationManager.Instance.StartConversation(_startConvo);
    }

    public void CompletedMiniGame(Component sender, object obj)
    {
        MiniGameFinishedEventArgs args = obj as MiniGameFinishedEventArgs;
        switch (args.FinishedMiniGame)
        {
            case MiniGame.PowerRegulating:
                if(_failedPower) ConversationManager.Instance.StartConversation(_powerCompletedConvo);
                else ConversationManager.Instance.StartConversation(_powerConvo);
                break;
            case MiniGame.FanBlock:

                break;
            case MiniGame.PipeBroke:

                break;
            case MiniGame.WasteManagement:

                break;
        }
    }

    public void FailedMiniGame(Component sender, object obj)
    {
        MiniGameFinishedEventArgs args = obj as MiniGameFinishedEventArgs;

        switch (args.FinishedMiniGame)
        {
            case MiniGame.PowerRegulating:
                ConversationManager.Instance.StartConversation(_failedPowerConvo);
                _failedPower = true;
                break;
            case MiniGame.FanBlock:

                break;
            case MiniGame.PipeBroke:

                break;
            case MiniGame.WasteManagement:

                break;
        }
    }

    public void OpenPowerHelpDialogue(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_powerHelpConvo);
    }
}

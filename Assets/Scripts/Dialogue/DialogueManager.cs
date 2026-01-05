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
    [SerializeField] private NPCConversation _turbineConvo;
    [SerializeField] private NPCConversation _turbineHelpConvo;
    [SerializeField] private NPCConversation _turbineCompleteConvo;
    [SerializeField] private NPCConversation _PipeConvo;
    [SerializeField] private NPCConversation _pipeHelpConvo;
    [SerializeField] private NPCConversation _pipeCompleteConvo;


    private bool _failedPower;

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
                ConversationManager.Instance.StartConversation(_turbineCompleteConvo);
                break;
            case MiniGame.PipeBroke:
                ConversationManager.Instance.StartConversation(_pipeCompleteConvo);
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
                ConversationManager.Instance.StartConversation(_failedConvo);
                break;
            case MiniGame.PipeBroke:
                ConversationManager.Instance.StartConversation(_failedConvo);
                break;
            case MiniGame.WasteManagement:
                ConversationManager.Instance.StartConversation(_failedConvo);
                break;
        }
    }

    public void OpenStartDialogue(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_startConvo);
    }

    public void OpenPowerHelpDialogue(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_powerHelpConvo);
    }

    public void OpenTurbineDialogue(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_turbineConvo);
    }

    public void OpenTurbineHelpDialogue(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_turbineHelpConvo);
    }

    public void OpenPipeDialogue(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_PipeConvo);
    }

    public void OpenPipeHelpDialogue(Component sender, object obj)
    {
        ConversationManager.Instance.StartConversation(_pipeHelpConvo);
    }
}

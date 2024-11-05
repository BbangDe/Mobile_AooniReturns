using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : NetworkBehaviour
{
    private struct Message
    {
        public string Content;
        public PlayerRef Sender;
    }

    [SerializeField] GameObject chatObjectPrefab;
    [SerializeField] Transform chatParent;

    [Header("UI")]
    [SerializeField] TMP_InputField chatInput;

    [Header("Options")]
    [SerializeField] int MessageQueueLength = 128;
    [SerializeField] int MessageBufferLength = 4;

    private Queue<GameObject> messageQueue;
    private Queue<Message> messageBuffer;

    private void Start()
    {
        messageQueue = new(MessageQueueLength);
        messageBuffer = new(MessageBufferLength);

        chatInput.onSubmit.AddListener(SendChat);
        chatInput.onSelect.AddListener(OpenPanel);
        chatInput.onEndEdit.AddListener(ClosePanel);
    }

    private void OpenPanel(string _msg)
    {
        chatInput.text = string.Empty;
        chatInput.ActivateInputField();
    }

    private void ClosePanel(string _msg)
    {
        chatInput.text = string.Empty;
        chatInput.DeactivateInputField();
    }

    private void Update()
    {
        while (messageBuffer.TryDequeue(out var msg))
        {
            var chatObj = messageQueue.Count < MessageQueueLength ?
                Instantiate(chatObjectPrefab, chatParent) :
                messageQueue.Dequeue();

            if (!chatObj.TryGetComponent<ChatText>(out var chatComp))
            {
                Debug.LogError("Chat prefab must have Chat component.");
                return;
            }

            chatObj.transform.SetAsLastSibling();
            chatComp.Setup(msg.Content, msg.Sender);
            messageQueue.Enqueue(chatObj);
        }
    }

    private void SendChat(string _msg)
    {
        if (string.IsNullOrWhiteSpace(_msg))
        {
            return;
        }

        if (_msg.StartsWith('\n'))
        {
            _msg = _msg[1..];
        }

        if (_msg.EndsWith('\n'))
        {
            _msg = _msg[0..^1];
        }

        Debug.Log("RPC_SendChat 호출");
        RPC_SendChat(_msg);

        chatInput.text = string.Empty;
        chatInput.ActivateInputField();
    }

    [Rpc]
    private void RPC_SendChat(string _msg, RpcInfo _info = default)
    {
        Debug.Log($"RPC_SendChat 호출됨: {_msg}");

        messageBuffer.Enqueue(new()
        {
            Content = _msg,
            Sender = _info.Source
        });
    }
}

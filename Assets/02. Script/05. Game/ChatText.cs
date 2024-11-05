using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class ChatText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI content;

    public void Setup(string _content, PlayerRef _sender)
    {
        // TODO: Customize message layout or sender nick
        content.text = $"Player{_sender.PlayerId} : {_content}";
    }
}
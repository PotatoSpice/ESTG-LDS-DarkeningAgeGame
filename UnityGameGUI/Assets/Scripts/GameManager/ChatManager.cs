using Assets.Scripts.Models;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ChatManager
{
    private Text chatHistory;
    private InputField newMessage;

    public ChatManager(Text chat, InputField message)
    {
        chatHistory = chat;
        newMessage = message;
    }

    public void AddMessageToHistory(string username, string content)
    {
        string fullMessage = "<color=blue>   " + username + ": </color>" + content;
        chatHistory.text = chatHistory.text + fullMessage + "\n";
    }

    //Send chat message to server
    public string SendChatMessage(string username)
    {
        string message = newMessage.text;

        if (!string.IsNullOrWhiteSpace(message))
        {
            ServerResponse sendMessage = new ServerResponse();
            sendMessage.EventType = "ChatMessage";
            sendMessage.data.Add(username);
            sendMessage.data.Add(message);

            newMessage.text = "";

            return JsonConvert.SerializeObject(sendMessage);
        }

        return "";
    }

    public string GetInputText()
    {
        return newMessage.text;
    }
}

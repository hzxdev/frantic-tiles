using DapperDino.UDCT.Utilities.DeveloperConsole;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    List<Message> messageList = new List<Message>();

        PlayerController pcont;
    public bool isChatFocused;
    [SerializeField] private TMP_InputField inputField = null;
   public DeveloperConsoleBehaviour developerConsoleBehaviour;
    public GameObject textPanel, chatObject;
    public Color playerMessageColor, systemMessageColor, selfMessageHighlight, otherMessageHighlight;

    public float messageCooldown = 2, msgCooldownTimestamp;
    public int messageCharLimit;

    void Start()
    {

        pcont = transform.root.GetComponentInChildren<PlayerController>();
        developerConsoleBehaviour = transform.root.GetComponentInChildren<DeveloperConsoleBehaviour>();
        isChatFocused = false;
        UnfocusChat();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (isChatFocused)
            {
                //process field text
                
                if (inputField.text.StartsWith(developerConsoleBehaviour.prefix))
                {
                    //it is a command
                    developerConsoleBehaviour.ProcessCommand(inputField.text);
                    inputField.text = string.Empty;
                    UnfocusChat();
                }
                else
                {
                    //it is a chat message
                    

                    if(PhotonNetwork.Time > msgCooldownTimestamp + messageCooldown) //check for spam
                    {
                        Chat(inputField.text, MessageType.PlayerMessage);
                    } else
                    {
                        Chat("Please don't spam!", MessageType.System);
                    }                  
                   

                    inputField.text = string.Empty;
                    UnfocusChat();
                }
                    isChatFocused = false;
                return;
            }

            else
            {
                isChatFocused = true;
                FocusChat();
            }
        }

    }

    public void FocusChat()
    {
        inputField.interactable = true;
        inputField.ActivateInputField();

        pcont.isTypingOnChat = true;
    }

    public void UnfocusChat()
    {
        inputField.DeactivateInputField();
        inputField.interactable = false;
        pcont.isTypingOnChat = false;
    }

    public void Chat(string text, MessageType messageType)
    {
        if (String.IsNullOrEmpty(text))
            return;

        if(text.Length > messageCharLimit)
        {
            Chat("Can't exceed 100 characters.", MessageType.System);
            return;
        }

        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(chatObject, textPanel.transform);
        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();
        if(messageType == MessageType.PlayerMessage)
        {
           

            newMessage.textObject.text = "<color=#" + ColorUtility.ToHtmlStringRGB(selfMessageHighlight) +  ">" + PhotonNetwork.NickName + "</color> " + ": " + newMessage.text;

            object[] content = new object[] { text, PhotonNetwork.NickName }; 
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.DoNotCache };
            PhotonNetwork.RaiseEvent(ModControls.PLAYER_MESSAGE_CODE, content, raiseEventOptions, SendOptions.SendReliable);
            msgCooldownTimestamp = (float)PhotonNetwork.Time;
        }
        else
            newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeToColor(messageType);
        messageList.Add(newMessage);

      

    }

    private new void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private new void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == ModControls.PLAYER_MESSAGE_CODE)
        {
            object[] data = (object[])photonEvent.CustomData;

            string text = (string)data[0];
            string senderNickname = (string)data[1];

            //No need for isMine check (Non-locals dont have canvases, therefore dont have ChatManager.cs)
                // I will receive the chat message

                if (String.IsNullOrEmpty(text))
                    return;
                Message newMessage = new Message();
                newMessage.text = text;
                GameObject newText = Instantiate(chatObject, textPanel.transform);
                newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();              
                newMessage.textObject.text = "<color=#" + ColorUtility.ToHtmlStringRGB(otherMessageHighlight) + ">" + senderNickname + " </color> " + ": " + newMessage.text;



               
                newMessage.textObject.color = MessageTypeToColor(MessageType.PlayerMessage);
                messageList.Add(newMessage);
            

        }
        
    }

    

    Color MessageTypeToColor(MessageType messageType)
    {
        Color color = systemMessageColor;

        switch (messageType)
        {
            case MessageType.PlayerMessage:
                color = playerMessageColor;
                break;

            case MessageType.System:
                color = systemMessageColor;
                break;
        }
        return color;

    }

    public void CouldntPerform(string commandName)
    {
        Chat("Couldn't perform (/" + commandName + ").", MessageType.System);
    }

    public void BadUsage(string commandName)
    {
        Chat("Incorrect usage of (/" + commandName + ").", MessageType.System);
    }

    public void NoPermission(string commandName)
    {
        Chat("You don't have the permission to use (/" + commandName + ").", MessageType.System);
    }
}




[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI textObject;
    public MessageType messageType;
}

public enum MessageType
{
    PlayerMessage,
    System
}

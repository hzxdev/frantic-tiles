using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Tilemaps;
using System.Text;

public class ModControls : MonoBehaviourPunCallbacks
{
    public bool isMod;
    public readonly static byte KICK_CODE = 1, KILL_CODE = 2, FREEZE_CODE = 3, GIVE_CODE = 4, PLAYER_MESSAGE_CODE = 5, REQUEST_WORLD_DATA = 6, SEND_WORLD_DATA = 7, CONSUMABLE_MC = 8;
    TilesInteraction tilei;
    PlayerController pcont;
    ItemsManager itemsManager;
    AudioManager audioManager;
    ChatManager chatManager;

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        isMod = true;
        tilei = GetComponent<TilesInteraction>();
        pcont = GetComponent<PlayerController>();
        chatManager = transform.root.GetComponentInChildren<ChatManager>();
        audioManager = transform.parent.GetComponentInChildren<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
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

        if (eventCode == KICK_CODE)
        {
            object[] data = (object[])photonEvent.CustomData;

            int toBeKickedActorNumber = (int)data[0];

            if(photonView.OwnerActorNr == toBeKickedActorNumber && photonView.IsMine)
            {
                // I will be kicked.

                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(0);
                Debug.Log("You got kicked from the room!");
            }
            
        }

        if (eventCode == GIVE_CODE)
        {
            object[] data = (object[])photonEvent.CustomData;

            int toBeGivenActorNumber = (int)data[0];
            int itemID = (int)data[1];
            int amount = (int)data[2];


            if (photonView.OwnerActorNr == toBeGivenActorNumber && photonView.IsMine)
            {
                // I will get the item.
                pcont.itemsManager.AddItem(tilei.allItems[itemID], amount, null);

                chatManager.Chat("You were given item(s)!", MessageType.System);
            }

        }
    }

    public void KickPlayer(string playerNickname)
    {
        if(!isMod)
        {
            chatManager.NoPermission("kick");
            return;
            
        }



        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == playerNickname) // check if player exists
            {
                
                object[] content = new object[] { PhotonNetwork.PlayerList[i].ActorNumber }; //Send player actor number (unique id in room)
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache }; 
                PhotonNetwork.RaiseEvent(KICK_CODE, content, raiseEventOptions, SendOptions.SendReliable);
                audioManager.PlayOneShot("command", false);
                return;

            }
        }

        chatManager.CouldntPerform("kick");

    }

    public void GiveToSelf(int itemId, int amount)
    {
        if (!isMod)
        {
            chatManager.NoPermission("give");
            return;

        }

        if (itemId >= tilei.allItems.Length || amount <= 0 || itemId < 0)
        {
            chatManager.CouldntPerform("give");
            return;
        }

        pcont.itemsManager.AddItem(tilei.allItems[itemId], amount, null);

        chatManager.Chat("You were given item(s)!", MessageType.System);

    }

    public void GivePlayer(string playerNickname, int itemId, int amount)
    {

        if (!isMod)
        {
            chatManager.NoPermission("give");
            return;

        }

        if(itemId >= tilei.allItems.Length || amount <= 0 || itemId < 0)
        {
            chatManager.CouldntPerform("give");
            return;
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == playerNickname) // check if player exists
            {

                object[] content = new object[] { PhotonNetwork.PlayerList[i].ActorNumber, itemId, amount }; //Send player actor number (unique id in room)
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };
                PhotonNetwork.RaiseEvent(GIVE_CODE, content, raiseEventOptions, SendOptions.SendReliable);
                audioManager.PlayOneShot("command", false);

                return;

            }
        }

        chatManager.CouldntPerform("give");

    }

    public void KillPlayer(string playerNickname)
    {

        if (!isMod)
        {
            chatManager.NoPermission("kill");
            return;

        }
    }

    public void FreezePlayer(string playerNickname)
    {

        if (!isMod)
        {
            chatManager.NoPermission("freeze");
            return;

        }
    }

    public void HelpCommand()
    {
        var builder = new StringBuilder();

        chatManager.Chat("Commands:", MessageType.System);

        for (int i = 0; i < chatManager.developerConsoleBehaviour.commands.Length; i++)
        {
            builder.Append(chatManager.developerConsoleBehaviour.commands[i].CommandWord.ToString());
            builder.Append(", ");
        }
        chatManager.Chat(builder.ToString(), MessageType.System);

    }
}

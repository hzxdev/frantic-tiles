using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public enum Gamemode
{
    None,
    Freeroam,
    Bedwars,
    Spleef,
    Parkour,
    HungerGames
}

public class StartMenu : MonoBehaviourPunCallbacks
{
    Hashtable roomProps = new Hashtable { { "gamemode", "none" } };
    public bool JoinOrCreateFreeroam; //quick, without selecting gamemode (it's auto freeroam)
    public Gamemode start;

    // Start is called before the first frame update
  

    public TextMeshProUGUI infoText;


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        infoText.text = "Connection successful";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        infoText.text = "Disconnected: " + cause.ToString();
    }

    public override void OnJoinedLobby()
    {
        if(start == Gamemode.None )
        {
            PhotonNetwork.JoinOrCreateRoom("room", new Photon.Realtime.RoomOptions { MaxPlayers = 8, IsOpen = true, IsVisible = true, }, TypedLobby.Default);

        } else 
        {
            ExitGames.Client.Photon.Hashtable customPropreties = new ExitGames.Client.Photon.Hashtable();

            PhotonNetwork.JoinOrCreateRoom("room", new Photon.Realtime.RoomOptions { MaxPlayers = 8, IsOpen = true, IsVisible = true, BroadcastPropsChangeToAll = true, CustomRoomProperties = roomProps}, TypedLobby.Default);

        }

    }

    public override void OnJoinedRoom()
    {
       

        if(start == Gamemode.None)
        {

            infoText.text = "Joined room! (Quick mode)";
            PhotonNetwork.LoadLevel(1);

        } else if(start == Gamemode.Bedwars)
        {

            infoText.text = "Joined room! (Updating room props?)";
            if(PhotonNetwork.IsMasterClient)
            {
                roomProps["gamemode"] = "bedwars";
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            }


            PhotonNetwork.LoadLevel(1);
        }
     
    }

}

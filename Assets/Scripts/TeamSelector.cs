using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TeamSelector : MonoBehaviourPun
{
    Tilemap tilemap;

    [SerializeField]
    public Dictionary<int, Vector2> constructedWorldData, receivedWorldData;

    void Start()
    {
      


        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
        {
            Destroy(gameObject);
            return;
        }

        tilemap = GameObject.FindGameObjectWithTag("MainTilemap").GetComponent<Tilemap>();


        //load world from masterclient
        if (!PhotonNetwork.IsMasterClient)
        RequestWorldData();




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

        if (eventCode == ModControls.REQUEST_WORLD_DATA) //received by master client
        {
            
            /*object[] data = (object[])photonEvent.CustomData;

            string text = (string)data[0];
            */

            //No need for isMine check (Non-locals dont have been destroyed in the start
     


           


        }

    }

    

    private void RequestWorldData()
    {
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; 
        PhotonNetwork.RaiseEvent(ModControls.REQUEST_WORLD_DATA, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public static string SaveTilemapAsString(Tilemap tilemap) // blockId,x,y;blockId,x,y; .....
    {
        string s = "";
        for (int i = 0; i < tilemap.GetTilesBlock(tilemap.cellBounds).Length; i++)
        {
            if (tilemap.GetTilesBlock(tilemap.cellBounds)[i] is Item item)
            {
                s = item.id + "," + s;
               
            }
        }
        return s;
    }

}

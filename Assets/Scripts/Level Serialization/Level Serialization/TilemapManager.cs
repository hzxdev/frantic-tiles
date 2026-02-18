using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviourPun {

    public Item[] allItems; // same with the one at TilesInteraction, but also need one here

    [SerializeField] private Tilemap mainTilemap, backgroundTilemap, shadowTilemap;
    [SerializeField] private int _levelIndex;
    public Level currentLevel;
    [TextArea(40, 40)]
    public string serializedCurrentLevel;
    ChatManager chatManager;

    private void Start()
    {

        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
        {
            enabled = false;
            return;
        }

        SortAllItemsArray();
        chatManager = transform.root.GetComponentInChildren<ChatManager>();
        mainTilemap = GameObject.FindGameObjectWithTag("MainTilemap").GetComponent<Tilemap>();
        backgroundTilemap = GameObject.FindGameObjectWithTag("BackgroundTilemap").GetComponent<Tilemap>();
        shadowTilemap = GameObject.FindGameObjectWithTag("ShadowTilemap").GetComponent<Tilemap>();
        //bgmap doesnt pass "is item" if statement so shouldnt be in the serialized tilemap string
        //because bg tiles dont have their items for now

        if (!PhotonNetwork.IsMasterClient)
        {
            RequestWorldData();
        }
          

 
    }

    void SortAllItemsArray()
    {
        allItems = new Item[Resources.LoadAll<Item>("Items/").Length];
        allItems = Resources.LoadAll<Item>("Items/");
        Array.Sort(allItems, delegate (Item x, Item y) { return x.id.CompareTo(y.id); });
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

            object[] data = (object[])photonEvent.CustomData;

            int requesterActorNumber = (int)data[0];

            

            //No need for isMine check (Non-locals TilemapManagers have been disabled in the start)

            currentLevel = new Level();
            currentLevel.MainTiles = GetTilesFromMap(mainTilemap).ToList();
            currentLevel.BackgroundTiles = GetTilesFromMap(backgroundTilemap).ToList();

            serializedCurrentLevel = currentLevel.Serialize();
            byte[] serializedCurrentLevelBytes = SerializeToBytes(serializedCurrentLevel);

            object[] content = new object[] { requesterActorNumber, serializedCurrentLevelBytes }; 
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(ModControls.SEND_WORLD_DATA, content, raiseEventOptions, SendOptions.SendReliable);

            int dataSizeInBytes = serializedCurrentLevelBytes.Length;
            chatManager = transform.root.GetComponentInChildren<ChatManager>();
            chatManager.Chat("Sending a world data of " + dataSizeInBytes.ToString() + " bytes to " + PhotonNetwork.CurrentRoom.GetPlayer(requesterActorNumber).NickName, MessageType.System);


        }

        if (eventCode == ModControls.SEND_WORLD_DATA) //received by world data requester
        {

            object[] data = (object[])photonEvent.CustomData;

            int receiverActorNumber = (int)data[0];
            byte[] worldDataInBytes = (byte[])data[1];

            if (PhotonNetwork.LocalPlayer.ActorNumber != receiverActorNumber)
                return;

            //Correct adress

            //ChatManager might not be assigned so guarentee
            chatManager = transform.root.GetComponentInChildren<ChatManager>();

            //Deserialize world data
            DeserializeFromBytes(worldDataInBytes);




        }

    }

    private void RequestWorldData()
    {
        object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber}; //Send player actor number (unique id in room)
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(ModControls.REQUEST_WORLD_DATA, content, raiseEventOptions, SendOptions.SendReliable);
    }





    IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
    {
        foreach (var pos in map.cellBounds.allPositionsWithin)
        {
            if (map.HasTile(pos))
            {
                if(map.GetTile<TileBase>(pos) is Item)
                {
                    var levelTile = map.GetTile<Item>(pos);
                    yield return new SavedTile()
                    {
                        Position = pos,
                        Tile = levelTile
                    };
                }

                if (map.GetTile<TileBase>(pos) is ItemRule)
                {
                    var levelTile = map.GetTile<ItemRule>(pos);
                    yield return new SavedTile()
                    {
                        Position = pos,
                        Tile = levelTile.reference
                    };
                }

            }
        }
    }

    public void ClearMap() {
        var maps = FindObjectsOfType<Tilemap>();

        foreach (var tilemap in maps) {
            tilemap.ClearAllTiles();
        }
    }

    //Not used for now
    public void LoadMap() {
        var level = Resources.Load<ScriptableLevel>($"Levels/Level {_levelIndex}");
        if (level == null) {
            Debug.LogError($"Level {_levelIndex} does not exist.");
            return;
        }

        ClearMap();

        foreach (var savedTile in level.GroundTiles) {
            switch (savedTile.Tile.blockKind) {
                case BlockKind.Default:
                    SetTile(mainTilemap, savedTile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (var savedTile in level.UnitTiles)
        {
            switch (savedTile.Tile.blockKind)
            {
                case BlockKind.Background:
                    SetTile(backgroundTilemap, savedTile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void SetTile(Tilemap map, SavedTile tile) {
            map.SetTile(tile.Position, tile.Tile);
        }
    }

   

public byte[] SerializeToBytes(string serializedData)
{
        serializedData = currentLevel.Serialize();

    using (MemoryStream memoryStream = new MemoryStream())
    using (BinaryWriter writer = new BinaryWriter(memoryStream))
    {
        writer.Write(serializedData);
        return memoryStream.ToArray();
    }
}

    public void DeserializeFromBytes(byte[] bytes)
    {
        chatManager.Chat("Deserializing from the bytes received...", MessageType.System);

        using (MemoryStream memoryStream = new MemoryStream(bytes))
        using (BinaryReader reader = new BinaryReader(memoryStream))
        {
            string serializedData = reader.ReadString();
            Deserialize(serializedData);
        }
    }

    public void Deserialize(string serializedData)
    {
        chatManager.Chat("Doing Deserialize", MessageType.System);


        mainTilemap.ClearAllTiles();
        shadowTilemap.ClearAllTiles();
      //  backgroundTilemap.ClearAllTiles();

        // Split the serialized string into main and background sections.
        string[] sections = serializedData.Split(new string[] { "main[", "background[" }, StringSplitOptions.RemoveEmptyEntries);

        if (sections.Length < 2)
        {
            Debug.LogError("Invalid serialized data.");
            return;
        }

        DeserializeTilemap(mainTilemap, sections[0]);
      //  DeserializeTilemap(backgroundTilemap, sections[1]);
    }

    private void DeserializeTilemap(Tilemap tilemap, string data)
    {
        chatManager.Chat("Building the tilemap...", MessageType.System);


        // Split the data into individual tile entries.
        string[] tileEntries = data.Split(')');

        foreach (string tileEntry in tileEntries)
        {
            if (!string.IsNullOrEmpty(tileEntry))
            {
                // Split each tile entry into id, posX, and posY.
                string[] parts = tileEntry.Split(new char[] { '(', ',' });

                if (parts.Length >= 3)
                {
                    int id = int.Parse(parts[0]);
                    float posX = float.Parse(parts[1]);
                    float posY = float.Parse(parts[2]);

                    Vector3Int cellPosition = new Vector3Int((int)posX, (int)posY, 0);
                    Item item = allItems[id];
                    if(item.ruleReference == null) //normal item
                    {
                        tilemap.SetTile(cellPosition, item);
                        shadowTilemap.SetTile(cellPosition, item);
                    } else //rule item
                    {
                        tilemap.SetTile(cellPosition, item.ruleReference);
                        shadowTilemap.SetTile(cellPosition, item);
                    }

                    
                }
            }
        }
    }

}






public struct Level {
    public int LevelIndex;
    public List<SavedTile> MainTiles;
    public List<SavedTile> BackgroundTiles;

    public string Serialize() {
        var builder = new StringBuilder();

        builder.Append("main[");
        foreach (var mainTile in MainTiles) {

         
            builder.Append($"{(int) mainTile.Tile.id}({mainTile.Position.x},{mainTile.Position.y})");
        }
        builder.Append("]");
        builder.Append("background[");
        foreach (var bgTile in BackgroundTiles)
        {
            builder.Append($"{(int)bgTile.Tile.id}({bgTile.Position.x},{bgTile.Position.y})");
        }
        builder.Append("]");

        return builder.ToString();
    }
}
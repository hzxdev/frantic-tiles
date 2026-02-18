using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.Rendering;

public class ItemUser : MonoBehaviourPun
{

    ItemsManager itemsManager;
    PlayerController pcont;
    Item selectedConsumable;
    Camera cam;
    public GameObject visualFireballObj, mcFireballObj;
    float fireBallSpeed = 6;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        itemsManager = transform.parent.GetComponentInChildren<ItemsManager>();
        pcont = GetComponent<PlayerController>();
        cam = transform.parent.GetComponentInChildren<Camera>();
        audioManager = transform.parent.GetComponentInChildren<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            return;

            if (Input.GetMouseButtonDown(1))
        {
            if(!pcont.IsBusyWithUI() && pcont.CanUseConsumables())
            {
                ConsumeItem();
            }
        }
    }

    public void UpdateSelectedConsumable()
    {
        selectedConsumable = itemsManager.selectedItem;
        switch (selectedConsumable.consumableType)
        {
            case ConsumableType.Fireball:
                //Fireball trailer guide, crosshair or smth

                break;
        }
    }

    public void ConsumeItem() //Right click on PC
    {
        if (itemsManager.selectedItem == null)
            return;

        if (itemsManager.selectedItem.consumableType != ConsumableType.None)
            selectedConsumable = itemsManager.selectedItem;
        else
            return;

        //Let's use it

        itemsManager.RemoveItem(selectedConsumable, 1);

        switch (selectedConsumable.consumableType)
        {
            case ConsumableType.Fireball:
                //Tell the MC to create fireball


               //                        mouse pos                                     
                Vector2 direction = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                

                object[] content = new object[] { (int)(selectedConsumable.consumableType), transform.position, direction.normalized };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };
                PhotonNetwork.RaiseEvent(ModControls.CONSUMABLE_MC, content, raiseEventOptions, SendOptions.SendReliable);

                //RPC the visual fireball to everyone except MC(mc check on his end) (visual fireball doesnt have collider)
                //
                photonView.RPC(nameof(FireballVisualRPC), RpcTarget.All, transform.position, direction.normalized);
                audioManager.PlayOneShot("launch3", true);


                break;
        }

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

        if (eventCode == ModControls.CONSUMABLE_MC)
        {
            //Received by masterclient. MC will now instantiate the consumable (eg. fireball)
            //On his end then RPC the visuals to everyone, and RaiseEvent or RPC the result (eg. explosion)

            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
                return;

            object[] data = (object[])photonEvent.CustomData;
            int a =(int)data[0];
            ConsumableType consumable = (ConsumableType)a;
            Vector3 pos = (Vector3)data[1];
            Vector2 normalizedDirection = (Vector2)data[2];

            switch (consumable)
            {
                case ConsumableType.Fireball:
                    //Simulate
                    //Instantiate real (mc) fireball
                    GameObject fireballGO = Instantiate(mcFireballObj, pos, Quaternion.Euler(normalizedDirection.x, normalizedDirection.y, 0));
                   
                    fireballGO.GetComponent<Rigidbody2D>().linearVelocity = normalizedDirection * fireBallSpeed;
                    fireballGO.GetComponent<Projectile>().Creator = this;
                    break;
            }


        }

    }

    [PunRPC]
    void FireballVisualRPC(Vector3 pos, Vector2 normalizedDirection)
    {
        if (PhotonNetwork.IsMasterClient) //I already the have the real prefab bitch
            return;

        GameObject fireballGO = Instantiate(visualFireballObj, pos, Quaternion.Euler(normalizedDirection.x, normalizedDirection.y, 0));

        fireballGO.GetComponent<Rigidbody2D>().linearVelocity = normalizedDirection * fireBallSpeed;
       


    }
   
    
   
}

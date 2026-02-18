using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorGO : MonoBehaviour
{
    public Item generatedItem;
    public float period = 2;
    float timer;
    ItemsManager localItemsManager; //Local player's ItemsManager
    GameObject localPlayer;

    public bool isPlayerIn;

    // Start is called before the first frame update
    void Start()
    {
        if(transform.parent.CompareTag("ShadowTilemap"))
        {
            Destroy(gameObject);
        }

        timer = 0;
       
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerIn)
        {
            timer += Time.deltaTime;
            if(timer > period)
            {
                GiveItem();
                timer = 0;
            }
        }
    }

    void GiveItem()
    {
        localItemsManager.AddItem(generatedItem, 1, null);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name);

        if(col.gameObject.CompareTag("Player"))
        {
            if(col.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                localPlayer = col.gameObject;
                localItemsManager = localPlayer.transform.root.GetComponentInChildren<ItemsManager>();
            }

        }

        if (col.gameObject == localPlayer)
        {
            isPlayerIn = true;
            timer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == localPlayer)
        {
            isPlayerIn = false;
            timer = 0;
        }

    }

}

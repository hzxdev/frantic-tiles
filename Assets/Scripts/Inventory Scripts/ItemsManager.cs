using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviourPun
{
   // public Dictionary<Item, int> items = new Dictionary<Item, int>();  
    public Item demandItem;
    public int demandAmount;
    public int inventorySpace;
    public Color itemHighlightColor;
    public ItemSlot[] itemSlots;
    public TilesInteraction tilesInteraction;
    Image inventoryImage;
    bool inventoryOpen;
    public int globalOrder;
    public bool blockSelected;
     AudioManager audioManager;
    public GameObject droppedItemPrefab;
    public Transform player;
    SpriteRenderer playerSR;
    public Item selectedItem;
    PlayerController pcont;
    ItemUser itemUser;

    void Start()
    {
        
        inventoryImage = GetComponent<Image>();
    
       
      

        for (int i = 0; i < itemSlots.Length; i++)
        {
         //   itemSlots[i].gameObject.SetActive(false);
            itemSlots[i].image.enabled = false;
            itemSlots[i].amountText.enabled = false;
        }

       
      
        audioManager = transform.parent.parent.GetComponentInChildren<AudioManager>();

        playerSR = player.GetComponent<SpriteRenderer>();
        pcont = player.GetComponent<PlayerController>();
        itemUser = player.GetComponent<ItemUser>();


        InventoryPressed();
        
    }

    void Update()
    {
        //test item ekleme

       
       
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(!pcont.IsTypingSomewhere())
            SlotButton(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!pcont.IsTypingSomewhere())
                SlotButton(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!pcont.IsTypingSomewhere())
                SlotButton(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!pcont.IsTypingSomewhere())
                SlotButton(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (!pcont.IsTypingSomewhere())
                SlotButton(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (!pcont.IsTypingSomewhere())
                SlotButton(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (!pcont.IsTypingSomewhere())
                SlotButton(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (!pcont.IsTypingSomewhere())
                SlotButton(7);
        }

        if (Input.GetKeyDown(KeyCode.Q) && !pcont.IsTypingSomewhere())
        {
            //Drop item logic

            if (itemSlots[globalOrder].item != null)// if the slot is not empty
            {
                if(playerSR.flipX) //facing left
                {
                    GameObject go = Instantiate(droppedItemPrefab, player.transform.position + Vector3.left, droppedItemPrefab.transform.rotation);
                    go.GetComponent<DroppedItem>().itemInside = itemSlots[globalOrder].item;
                    go.GetComponent<DroppedItem>().amount = itemSlots[globalOrder].amount;
                    go.GetComponent<DroppedItem>().isTakeable = true;
                    go.GetComponent<DroppedItem>().UpdateValues();
                    tilesInteraction.selectedBlock = null;
                    selectedItem = null;
                    tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
                    tilesInteraction.SetLiteralBlockInHand(-1);


                } else //facing right
                {
                    GameObject go = Instantiate(droppedItemPrefab, player.transform.position + Vector3.right, droppedItemPrefab.transform.rotation);
                    go.GetComponent<DroppedItem>().itemInside = itemSlots[globalOrder].item;
                    go.GetComponent<DroppedItem>().amount = itemSlots[globalOrder].amount;
                    go.GetComponent<DroppedItem>().isTakeable = true;
                    go.GetComponent<DroppedItem>().UpdateValues();
                    tilesInteraction.selectedBlock = null;
                    selectedItem = null;
                    tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
                    tilesInteraction.SetLiteralBlockInHand(-1);
                }
                photonView.RPC(nameof(DroppedItemValuesRPC), RpcTarget.Others, playerSR.flipX);

                itemSlots[globalOrder].amount = 1;
                     //itemSlots[globalOrder].slotItem = null;
                BlockPlaced();
            }
            

        }

      

        //inventory açma
        if (Input.GetButtonDown("Inventory") && !pcont.IsTypingSomewhere())
        {
            InventoryPressed();
        }

    }

    ItemSlot FindSlotByItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == item)
            {
                return itemSlots[i];
            }

          
        }
        return null;
    }


    [PunRPC]
    public void DroppedItemValuesRPC(bool flipX)
    {
        if (flipX) //facing left
        {
            GameObject go = Instantiate(droppedItemPrefab, player.transform.position + Vector3.left, droppedItemPrefab.transform.rotation);
            go.GetComponent<DroppedItem>().itemInside = itemSlots[globalOrder].item;
            go.GetComponent<DroppedItem>().amount = itemSlots[globalOrder].amount;
            go.GetComponent<DroppedItem>().isTakeable = true;
            go.GetComponent<DroppedItem>().UpdateValues();
            tilesInteraction.selectedBlock = null;
            selectedItem = null;
            tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
            tilesInteraction.SetLiteralBlockInHand(-1);
        }
        else //facing right
        {
            GameObject go = Instantiate(droppedItemPrefab, player.transform.position + Vector3.right, droppedItemPrefab.transform.rotation);
            go.GetComponent<DroppedItem>().itemInside = itemSlots[globalOrder].item;
            go.GetComponent<DroppedItem>().amount = itemSlots[globalOrder].amount;
            go.GetComponent<DroppedItem>().isTakeable = true;
            go.GetComponent<DroppedItem>().UpdateValues();
            tilesInteraction.selectedBlock = null;
            selectedItem = null;
            tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
            tilesInteraction.SetLiteralBlockInHand(-1);

        }
    }

    public int GetItemAmount(Item item)
    {
        if(!HasItem(item))
            return 0;

        return itemSlots[HasItemWhere(item)].amount;
    }

    public int HasItemWhere(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == item)
            {
                return i;
            }
        }
        Debug.Log("Doesn't have the item tho!");
        return -1;
    }

    public bool HasItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == item)
            {
                return true;
            }
        }
        return false;
    }

    int FindFirstEmptySlot()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == null)
            {
                return i;
            }
        }
        Debug.Log("No empty slot though!");
        return -1;
    }


    public void RemoveItem(Item item, int amount)
    {

        if (HasItem(item))
        {
            itemSlots[HasItemWhere(item)].amount -= amount;
            itemSlots[HasItemWhere(item)].UpdateSlot();

          

        }
        else
        {
            Debug.Log("Already don't have the item!");

        }
     
    }

    public void AddItem(Item item, int amount, GameObject pickupGameobject)
    {

        if (HasItem(item))
        {
            itemSlots[HasItemWhere(item)].amount += amount;
            itemSlots[HasItemWhere(item)].UpdateSlot();

            if (pickupGameobject != null)
            {
                Destroy(pickupGameobject);
                audioManager.PlayOneShot("ItemPickup", true);
            }
               
        } else
        {
            if (SpaceLeft() <= 0)
            {
                Debug.Log("Not enough room!");
                return;
            }

            int index = FindFirstEmptySlot();
            itemSlots[index].item = item;
            itemSlots[index].amount = amount;
            itemSlots[index].UpdateSlot();
           
            if (pickupGameobject != null)
            {
                Destroy(pickupGameobject);
                audioManager.PlayOneShot("ItemPickup", true);
            }

            if(index == globalOrder)//this slot was selected and was empty, now it's full
            {
                if (itemSlots[globalOrder].item != null)
                {
                    if(itemSlots[globalOrder].item.type == ItemType.Block)
                    {
                        tilesInteraction.selectedBlock = itemSlots[globalOrder].item;
                        tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(true);
                    } else
                    {
                        tilesInteraction.selectedBlock = null;
                        tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
                    }
                    selectedItem = itemSlots[globalOrder].item;
                    tilesInteraction.SetLiteralBlockInHand(itemSlots[globalOrder].item.id);
                }

                if (itemSlots[globalOrder].item == null)
                {
                    tilesInteraction.selectedBlock = null;
                    selectedItem = null;
                    tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
                    tilesInteraction.SetLiteralBlockInHand(-1);

                }
            }

           
        }
      //  Debug.Log("Added " + amount + " " + item.name + "s to the inventory!");

        //demandItem vardı
        //  slotsImages[items.Count - 1].sprite = item.sprite; //demandItem vardı
        //   UpdateUI();
    }

    public int SpaceLeft()
    {
        int j = 0;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == null)
                j++;
        }
        return j;
    }

    public void ButtonAdd()
    {
        AddItem(demandItem, demandAmount, null);
    }

    public void FrameVisual(int order)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].selectFrame.SetActive(false);
        }
        itemSlots[order].selectFrame.SetActive(true);
    }

    public void SlotButton(int order)
    {
        
        
        globalOrder = order;
       
      //  itemSlots[order].image.color = Color.white;
        audioManager.PlayOneShot("SelectTile",false);
        blockSelected = true;
    
        if (itemSlots[order].item != null)
        {
            selectedItem = itemSlots[order].item;
            if (itemSlots[order].item.type == ItemType.Block)
            {
                tilesInteraction.selectedBlock = itemSlots[order].item;
                
                tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(true);
            } else
            {
                tilesInteraction.selectedBlock = null;

                tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
            }
          
            tilesInteraction.SetLiteralBlockInHand(itemSlots[order].item.id);

            if (itemSlots[order].item.consumableType != ConsumableType.None)
                itemUser.UpdateSelectedConsumable();

        }
           
        if (itemSlots[order].item == null)
        {
            tilesInteraction.selectedBlock = null;
            selectedItem = null;
            tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
            tilesInteraction.SetLiteralBlockInHand(-1);

        }
        FrameVisual(order);
       
    }

   

    public void BlockPlaced() // blok koyulduğunda çağırılır
    {
        if(itemSlots[globalOrder].amount > 0)
        {
            Item itemInHand = itemSlots[globalOrder].item;
            RemoveItem(itemInHand, 1);
        } 
      
        if(itemSlots[globalOrder].amount <= 0) {
            tilesInteraction.selectedBlock = null;
            selectedItem = null;
            tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
            tilesInteraction.SetLiteralBlockInHand(-1);
            itemSlots[globalOrder].UpdateSlot();
        }

        
    }



    public void InventoryPressed()
    {
        

        if (inventoryOpen) // eğer açıksa kapa
        {
            inventoryImage.enabled = false;
            foreach (ItemSlot img in itemSlots)
            {
                img.transform.gameObject.SetActive(false);
            }
            inventoryOpen = false;
            return; // DIKKAT ET return araştır nereye kadar return şimdilik sorun yok
        }
        else // kapalıysa aç
        {
            inventoryImage.enabled = true;
            foreach (ItemSlot img in itemSlots)
            {
                img.transform.gameObject.SetActive(true);
            }
            inventoryOpen = true;

        }

        
            
      
    }

    public void Clear()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].amount = 0;
            itemSlots[i].item = null;
            itemSlots[i].UpdateSlot();
            tilesInteraction.selectedBlock = null;
            selectedItem = null;
            tilesInteraction.blockPlaceHelperGrid.gameObject.SetActive(false);
            tilesInteraction.SetLiteralBlockInHand(-1);
        }
    }
}

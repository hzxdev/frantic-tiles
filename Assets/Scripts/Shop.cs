using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject[] blocks, defense, utility;
    public int ironBalance, goldBalance, diamondBalance;
    public Item iron, gold, diamond;
    public TextMeshProUGUI ironText, goldText, diamondText;
    ItemsManager itemsManager;
    ChatManager chat;
    AudioManager audioManager;

    void Start()
    {
        chat = transform.parent.GetComponentInChildren<ChatManager>();
        itemsManager = transform.parent.GetComponentInChildren<ItemsManager>();
        audioManager = transform.parent.parent.GetComponentInChildren<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnOpened()
    {
        UpdateBalanceAndText();
        audioManager.PlayOneShot("InventoryOpen", false);
    }

    public void CloseShop(bool byClicking)
    {
        if(byClicking)
        audioManager.PlayOneShot("InventoryClose", false);

        gameObject.SetActive(false);

    }

    public void UpdateBalanceAndText() // when set active? supposed to be
    {
        if (itemsManager == null)
            Start();

        ironBalance = itemsManager.GetItemAmount(iron);
        goldBalance = itemsManager.GetItemAmount(gold);
        diamondBalance = itemsManager.GetItemAmount(diamond);
        ironText.text = "<sprite=0>" + ironBalance.ToString();
        goldText.text = "<sprite=1>"+ goldBalance.ToString();
        diamondText.text = "<sprite=2>" + diamondBalance.ToString();

    }


    //This is called from the respective ShopObject.cs
    public void BuyPressed(Item item)
    {

        if(!itemsManager.HasItem(item) && itemsManager.SpaceLeft() == 0)
        {
            //There is no space in inventory and we dont already have the item
            chat.Chat("No space in inventory to buy " + item.name + ".", MessageType.System);
            return;
        }

        switch(item.shopPrice.currency)
        {
            case Currency.Iron:
                if(ironBalance >= item.shopPrice.price)
                {
                    //We can afford the item
                    itemsManager.RemoveItem(iron, item.shopPrice.price);
                    itemsManager.AddItem(item, item.shopPrice.amount, null);
                    UpdateBalanceAndText();
                    audioManager.PlayOneShot("Purchase", false);
                } else
                {
                    chat.Chat("You need " + (item.shopPrice.price - ironBalance).ToString() + " more " + item.shopPrice.currency.ToString() + " to afford " + item.name + ".", MessageType.System);
                }

                break;
            case Currency.Gold:
                if (goldBalance >= item.shopPrice.price)
                {
                    //We can afford the item
                    itemsManager.RemoveItem(gold, item.shopPrice.price);
                    itemsManager.AddItem(item, item.shopPrice.amount, null);
                    UpdateBalanceAndText();
                    audioManager.PlayOneShot("Purchase", false);


                }
                else
                {
                    chat.Chat("You need " + (item.shopPrice.price - goldBalance).ToString() + " more " + item.shopPrice.currency.ToString() + " to afford " + item.name + ".", MessageType.System);
                }

                break;
            case Currency.Diamond:
                if (diamondBalance >= item.shopPrice.price)
                {
                    //We can afford the item
                    itemsManager.RemoveItem(diamond, item.shopPrice.price);
                    itemsManager.AddItem(item, item.shopPrice.amount, null);
                    UpdateBalanceAndText();
                    audioManager.PlayOneShot("Purchase", false);


                }
                else
                {
                    chat.Chat("You need " + (item.shopPrice.price - diamondBalance).ToString() + " more " + item.shopPrice.currency.ToString() + " to afford " + item.name + ".", MessageType.System);
                }

                break;
        }

    }

    public void ShowCategoryObjects(string category)
    {
        switch(category)
        {
            case "all":
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].SetActive(true);
                }

                for (int i = 0; i < defense.Length; i++)
                {
                    defense[i].SetActive(true);
                }

                for (int i = 0; i < utility.Length; i++)
                {
                    utility[i].SetActive(true);
                }

                break;
            case "blocks":
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].SetActive(true);
                }

                for (int i = 0; i < defense.Length; i++)
                {
                    defense[i].SetActive(false);
                }

                for (int i = 0; i < utility.Length; i++)
                {
                    utility[i].SetActive(false);
                }
                break;
            case "utility":
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].SetActive(false);
                }

                for (int i = 0; i < defense.Length; i++)
                {
                    defense[i].SetActive(false);
                }

                for (int i = 0; i < utility.Length; i++)
                {
                    utility[i].SetActive(true);
                }
                break;
            case "defense":
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].SetActive(false);
                }

                for (int i = 0; i < defense.Length; i++)
                {
                    defense[i].SetActive(true);
                }

                for (int i = 0; i < utility.Length; i++)
                {
                    utility[i].SetActive(false);
                }
                break;
        }
    }
}

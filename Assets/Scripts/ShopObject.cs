using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopObject : MonoBehaviour
{
    public Item item;
    public TextMeshProUGUI itemName, itemPrice, itemAmount;
    public Image itemImage;
    public Shop shop;

    void Start()
    {
        itemName.text = item.name;
        itemPrice.text = item.shopPrice.price.ToString() + " <sprite=" + ((int)item.shopPrice.currency).ToString() +">";
        itemAmount.text =  "x" + item.shopPrice.amount.ToString();
        itemImage.sprite = item.sprite;
        shop = transform.parent.parent.parent.parent.GetComponent<Shop>();
    }


    void Update()
    {
        
    }

    public void ButtonPressed()
    {
        shop.BuyPressed(item);
    }
}

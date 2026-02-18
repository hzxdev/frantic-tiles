using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item;
    public int amount;
    public Text amountText;
    public Image image;
    public Button button;
    public GameObject selectFrame;

    void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        button = GetComponentInChildren<Button>();
        amountText = GetComponentInChildren<Text>();
        selectFrame = transform.GetChild(4).gameObject;
        button.interactable = true;
    }

    public void UpdateSlot()
    {

        if(item == null || amount == 0)
        {
            image.enabled = false;
            amountText.enabled = false;
            item = null;
            amount = 0;
        } else
        {
            image.enabled = true;
            image.sprite = item.sprite;
            amountText.enabled = true;
            amountText.text = amount.ToString();
        }
    }
}

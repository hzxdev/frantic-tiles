using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DroppedItem : MonoBehaviour
{
    public Item itemInside;
    public int amount;
    public bool isTakeable;
    void Start()
    {
        UpdateValues();
    }

    public void UpdateValues()
    {
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = itemInside.sprite;
        transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = amount.ToString();

    }


}

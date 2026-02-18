using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
     ItemsManager itemsManager;
    Button buttonOnSameLevel;
    public int order;
    public Button[] slotsButtons;
    bool isClickable, isIconFollowing;

    void Start()
    {
        itemsManager = transform.parent.parent.GetComponent<ItemsManager>();
        slotsButtons = new Button[itemsManager. inventorySpace];
  
        buttonOnSameLevel = transform.parent.GetChild(2).GetComponent<Button>();
        for (int i = 0; i < itemsManager. inventorySpace; i++)
        {
            
            slotsButtons[i] = transform.parent.parent.GetChild(i).GetComponentInChildren<Button>();
        }
        for (int i = 0; i < itemsManager.inventorySpace; i++)
        {
            if(slotsButtons[i] != null)
            {
                if (this == slotsButtons[i].transform.parent.GetChild(3).GetComponent<ClickableObject>())
                {
                    order = i;
                }
            }
               
        }
    }

    void Update()
    {
        isClickable = slotsButtons[order].interactable;
        if(isIconFollowing)
        {
            transform.parent.GetChild(0).position = Input.mousePosition;
            transform.parent.GetChild(1).position = Input.mousePosition;
        }
            
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isClickable)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                itemsManager.SlotButton(order);
            else if (eventData.button == PointerEventData.InputButton.Middle)
                Debug.Log("Middle click");
            else if (eventData.button == PointerEventData.InputButton.Right)
            {

                Debug.Log("Right clicked " + order);
              //  isIconFollowing = true;
                
            }
                
        }
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public ItemsManager itemsManager;
    public Sprite normalRightArmSprite;
    Weapon currentWeapon;
    public bool equippedWeapon;
    public SpriteRenderer rightArmRenderer;
    GameObject rightArm;

    void Start()
    {
        rightArm = rightArmRenderer.gameObject;
    }

    void Update()
    {
        /*
        if(itemsManager.blockSelected && itemsManager.itemSlots[itemsManager.globalOrder].slotItem.weaponReference != null)
        {
            equippedWeapon = true;
            currentWeapon = itemsManager.itemSlots[itemsManager.globalOrder].slotItem.weaponReference;

        }
        if (!itemsManager.blockSelected)
            equippedWeapon = false;

        if(equippedWeapon)
        {
            rightArmRenderer.sprite = currentWeapon.withArmSprite;
        } else
        {
            rightArmRenderer.sprite = normalRightArmSprite;
        }

        
            if (equippedWeapon && currentWeapon.weaponType == WeaponType.Light)
            {
            // rightArm.transform.up = Camera.main.ScreenToWorldPoint(Input.mousePosition) - rightArm.transform.position;
            Vector2 direction = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - rightArm.transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y - rightArm.transform .position.y);
            float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rightArm. transform.eulerAngles = new Vector3(0, 0, rotation + 90);


        }
        
        */
        
    }
}

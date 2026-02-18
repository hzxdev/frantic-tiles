using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Light,
    Rifle,
    Melee
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/New Weapon")]
public class Weapon : ScriptableObject
{
    public Sprite withArmSprite;
    public WeaponType weaponType;

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType
{
    Block,
    Juice,
    Consumable,
    Clothe,
    Currency,
    Default
}

public enum BlockKind
{
    Default,
    Background,
    Liquid
}

public enum Hazard
{
    None,
    TouchKill
}

public enum SpecialProperty
{
    None,
    CheckpointInactive,
    CheckpointActive,
    Unbreakable,
    Explosive
}

public enum NPCProperty
{
    None,
    Villager
}

public enum ConsumableType
{
    None = 0,
    Fireball = 1,
    GoldenApple = 2,
    EnderPearl = 3,
    KamikazeBackpack = 4,
    HealthPotion = 5,
    SpeedPotion = 6,
    HastePotion = 7,
    JumpPotion = 8
}


[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class Item : Tile
{
    [Header("Set id or you will experince WEIRD SHIT")]
    public int id;
    public Material particleEffect;
    public Color particleColor;
    public ItemType type;
    public BlockKind blockKind;
    public Hazard hazardType;
    public SpecialProperty specialProperty;
    public NPCProperty npcProperty;
    public float breakTime;
    [TextArea(15, 20)]
    public string description;
    public ItemRule ruleReference;
    public Weapon weaponReference;
    public Price shopPrice;
    public ConsumableType consumableType;
}

public enum Currency
{
    Iron,
    Gold,
    Diamond
}

[Serializable]
public struct Price
{
    public int amount;
    public int price;
    public Currency currency;

}




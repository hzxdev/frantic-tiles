using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Rule Tile Item", menuName = "Items/New Rule Tile Item")]
public class ItemRule : RuleTile
{
    public Item reference;
    public Material particleEffect;
    public Color particleColor;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    
}
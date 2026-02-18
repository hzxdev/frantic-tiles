using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Emote", menuName = "Emote/Create Emote")]
public class Emote : ScriptableObject
{
    public Sprite sprite;
    public int unlockLevel;
    public float duration;
    public string soundName;
}

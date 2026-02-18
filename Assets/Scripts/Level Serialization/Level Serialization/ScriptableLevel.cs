using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScriptableLevel : ScriptableObject {
    public int LevelIndex;
    public List<SavedTile> GroundTiles;
    public List<SavedTile> UnitTiles;
}

[Serializable]
public class SavedTile {
    public Vector3Int Position;
    public Item Tile;
}
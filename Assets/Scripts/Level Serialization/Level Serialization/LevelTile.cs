using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Level Tile", menuName = "2D/Tiles/Level Tile")]
public class LevelTile : Tile
{
    public TileType Type;

}

[Serializable]
public enum TileType
{
    // Ground
   Grass = 0,


   // Unit
   Snorlax = 1000,
   Quinn = 1001
}

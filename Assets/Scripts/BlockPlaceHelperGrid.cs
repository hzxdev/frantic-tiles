using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockPlaceHelperGrid : MonoBehaviour
{

    public Transform player;
    Tilemap tilemap;

    

    void Start()
    {
        tilemap = GameObject.FindGameObjectWithTag("MainTilemap").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position =  tilemap.CellToWorld(tilemap.WorldToCell(player.position)) + new Vector3(0.5f, 0.5f, 0);
    }
}

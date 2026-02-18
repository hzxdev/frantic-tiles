using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockBreakPiecesEffect : MonoBehaviour
{
    public Sprite step1, step2, step3, step4;
    TilesInteraction ti;
    SpriteRenderer sr;
    AudioManager audioManager;
    float timer, breakTime;
    bool bool1, bool2, bool3, bool4;
    public string punchSound;
    BlockKind blockType;
    Tilemap tilemap;

    void Start()
    {
        tilemap = GameObject.FindGameObjectWithTag("MainTilemap").GetComponent<Tilemap>();

        sr = GetComponent<SpriteRenderer>();
        ti = GameObject.FindGameObjectWithTag("Player").GetComponent<TilesInteraction>();
        breakTime = ti.blockBreakTime;
        audioManager = FindObjectOfType<AudioManager>();
        var tile = tilemap.GetTile(tilemap.WorldToCell(transform.position));
        if (tile is ItemRule itemRuleTile)
        {
            tile = itemRuleTile.reference;
        }
        if (tile is Item itemTile)
        {
            ti.lastBreakingBlockKind = itemTile.blockKind;
            if (itemTile.blockKind == BlockKind.Default)
            {
                punchSound = "RockPunch2";
            }
             
            if (itemTile.blockKind == BlockKind.Liquid)
                punchSound = "WaterPunch1";

        }
        audioManager.Stop(punchSound);
        audioManager.Play(punchSound);

    }

    void Update()
    {
        timer = ti.breakingTimer;
        if(timer >= 0 & timer < breakTime * 1/4)
        {
            sr.sprite = step1;
           
        }

        if (timer >= breakTime * 1 / 4 & timer < breakTime * 2/4)
        {
            sr.sprite = step2;
            if (!bool2)
            {
                audioManager.Stop(punchSound);
                audioManager.Play(punchSound);

            }

            bool2 = true;
        }

        if (timer >= breakTime * 2 / 4 & timer < breakTime * 3 / 4)
        {
            sr.sprite = step3;
            if (!bool3)
            {
                audioManager.Stop(punchSound);
                audioManager.Play(punchSound);

            }
            bool3 = true;
        }
        if (timer >= breakTime * 3 / 4 & timer < breakTime)
        {
            sr.sprite = step4;
            if (!bool4)
            {
                audioManager.Stop(punchSound);
                audioManager.Play(punchSound);

            }


            bool4 = true;
        }
    }
}

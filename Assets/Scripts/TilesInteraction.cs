using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.Purchasing;

public class TilesInteraction : MonoBehaviourPun, IPunObservable
{
    [Header("Not to be set through inspector.")]
    public Item[] allItems; // assisgned and sorted by id with SortAllItemsArray();

    Vector3 point;
    public GameObject breakPiecesEffect, breakParticlesDone, blockBrokenParticles, blockPuffParticles, checkpointPartices, explosionEffectPrefab;
    SpriteRenderer spriteRenderer, raRenderer, laRenderer;
    GameObject lastBreakEffect, lastBreakEffect2;
    Vector3Int lastBreakingBlockPos;
     Tilemap tilemap, shadowTilemap;
    public Item selectedBlock;
    public float blockBreakTime, breakingTimer, breakArmRotateSpeed, breakRangeX, breakRangeY, placeRangeX, placeRangeY;
    public bool isPunchingBlock, inLiquid;
    Grid grid;
    Transform leftArm, rightArm;
    PlayerController pcont;
    public ItemsManager itemsManager;
    AudioManager audioManager;
    public string breakSound, waterBreakSound;
    public BlockKind lastBreakingBlockKind;
    GunController gunController;
    public Shop shop; //set thru inspctr
    public Item checkpointActive, checkpointInactive, bluePortal, redPortal;
    bool hasHitACheckpoint;
    //
    int currentConPoints;
    bool conPointDone;
    bool inVillagerGO;
    public SpriteRenderer literalBlockInHand; // visible on player's left arm
    [HideInInspector]
    public float lblockinhandoffset;
    public BlockPlaceHelperGrid blockPlaceHelperGrid;

    public bool insideSolidBlock;
    public float suffocationTimer, suffocationMaxTime = 1;

    void Start()
    {

        gunController = GetComponent<GunController>();
        pcont = GetComponent<PlayerController>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftArm = GetComponent<PlayerController>().leftArm;
        rightArm = GetComponent<PlayerController>().rightArm;
        raRenderer = rightArm.gameObject.GetComponent<SpriteRenderer>();
        laRenderer = leftArm.gameObject.GetComponent<SpriteRenderer>();
        audioManager = transform.parent.GetComponentInChildren<AudioManager>();
        tilemap = GameObject.FindGameObjectWithTag("MainTilemap").GetComponent<Tilemap>();
        shadowTilemap = GameObject.FindGameObjectWithTag("ShadowTilemap").GetComponent<Tilemap>();
        lblockinhandoffset = literalBlockInHand.transform.localPosition.x;

        SortAllItemsArray();
    }

    void Update()
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
        {
            if (pcont.receivedIsPunchingBlock)
            {


                 BlockBreakAnimation();

              /*
                    if (!pcont.receivedIsWalking & pcont.receivedGrounded)
                    {
                        if (receivedPoint.x > transform.position.x)
                        {
                            spriteRenderer.flipX = false;
                            laRenderer.flipX = false;
                            raRenderer.flipX = false;
                            rightArm.localPosition = new Vector3(-0.15625f, 0.03125f, 0);
                            leftArm.localPosition = new Vector3(0.125f, 0.03125f, 0);
                        }
                        else
                        {
                            spriteRenderer.flipX = true;
                            laRenderer.flipX = true;
                            raRenderer.flipX = true;
                            rightArm.localPosition = new Vector3(0.15625f, 0.03125f, 0);
                            leftArm.localPosition = new Vector3(-0.125f, 0.03125f, 0);
                        }
                    }
                */

            }


            return;
        }
           

       

        if(pcont.playedOnAndroid)
        {
          //  UpdateForAndroid();
            return;
        }



        
        
            if(isPunchingBlock)
            {
                if (!pcont.isWalking & pcont.isGrounded)
                {
                    if (point.x > transform.position.x)
                    {
                        spriteRenderer.flipX = false;
                        laRenderer.flipX = false;
                        raRenderer.flipX = false;
                    pcont.FlipAllClothes(false);
                        rightArm.localPosition = new Vector3(-0.15625f, 0.03125f, 0);
                        leftArm.localPosition = new Vector3(0.125f, 0.03125f, 0);
                    }
                    else
                    {
                        spriteRenderer.flipX = true;
                        laRenderer.flipX = true;
                        raRenderer.flipX = true;
                    pcont.FlipAllClothes(true);
                        rightArm.localPosition = new Vector3(0.15625f, 0.03125f, 0);
                        leftArm.localPosition = new Vector3(-0.125f, 0.03125f, 0);
                    }
                }
            }

            if(gunController.equippedWeapon/* && pcont.isGrounded*/)
            {
            point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (point.x > transform.position.x)
                    {
                        spriteRenderer.flipX = false;
                        laRenderer.flipX = false;
                        raRenderer.flipX = false;
                pcont.FlipAllClothes(false);
                rightArm.localPosition = new Vector3(-0.15625f, 0.03125f, 0);
                        leftArm.localPosition = new Vector3(0.125f, 0.03125f, 0);
                    }
                    else
                    {
                        spriteRenderer.flipX = true;
                        laRenderer.flipX = true;
                        raRenderer.flipX = true;
                pcont.FlipAllClothes(true);
                rightArm.localPosition = new Vector3(0.15625f, 0.03125f, 0);
                        leftArm.localPosition = new Vector3(-0.125f, 0.03125f, 0);
                    }
                
            }
           
           
        


        if (Input.GetMouseButton(0))
        {
            point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int selectedTile = tilemap.WorldToCell(point);
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            if(!gunController.equippedWeapon)
            isPunchingBlock = true;
            
            if (tilemap.GetTile(selectedTile) != null & selectedTile.x <= tilemap.WorldToCell(transform.position).x + breakRangeX & selectedTile.x >= tilemap.WorldToCell(transform.position).x - breakRangeX & selectedTile.y <= tilemap.WorldToCell(transform.position).y + breakRangeY & selectedTile.y >= tilemap.WorldToCell(transform.position).y - breakRangeY & !gunController.equippedWeapon)
            {
                var tile2 = tilemap.GetTile(selectedTile);
                if (tile2 is ItemRule itemRuleTile2)
                {
                    tile2 = itemRuleTile2.reference;
                }
                if (tile2 is Item itemTile2)
                {
                   blockBreakTime = itemTile2.breakTime;
                }
                if (lastBreakingBlockPos == selectedTile)
                {

                    breakingTimer += Time.deltaTime;//asd
                    if (lastBreakEffect == null)
                    {
                        var tile = tilemap.GetTile(selectedTile);
                        if (tile is ItemRule itemRuleTile)
                        {
                            tile = itemRuleTile.reference;
                        }
                        if (tile is Item itemTile)
                        {
                            if (itemTile.specialProperty != SpecialProperty.Unbreakable)
                            {
                                GameObject go = Instantiate(breakPiecesEffect, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                                lastBreakEffect = go;
                                ParticleSystem particleSystem = go.transform.GetChild(1).GetComponent<ParticleSystem>();
                                var main = particleSystem.main;
                                main.startColor = itemTile.particleColor;
                            }
                                

                            
                               
                        }
                    }

                   

                }
                else
                {

                    if (lastBreakEffect != null)
                        Destroy(lastBreakEffect.gameObject);
                    breakingTimer = 0;
                    switch (lastBreakingBlockKind)
                    {
                        case BlockKind.Default:
                            //audioManager.Stop(breakSound);
                            break;
                        case BlockKind.Liquid:
                            //audioManager.Stop(waterBreakSound);
                            break;

                    }
                    lastBreakingBlockPos = selectedTile;
                   
                    var tile = tilemap.GetTile(selectedTile);
                    if (tile is ItemRule itemRuleTile)
                    {
                        tile = itemRuleTile.reference;
                    }
                    if (tile is Item itemTile)
                    {
                        // devre dışı bırakınca bir şey olmadı hayırlısı bakalım
                        /*
                        GameObject go =  Instantiate(breakParticlesDone, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                        GameObject go2 =  Instantiate(breakPiecesEffect, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                        lastBreakEffect = go2;
                        ParticleSystem particleSystem = go2.transform.GetChild(1).GetComponent<ParticleSystem>();
                        var main = particleSystem.main;
                        main.startColor = itemTile.particleColor;
                        */
                    } 
                    
                    if (lastBreakEffect2 != null)
                        Destroy(lastBreakEffect2.gameObject);

                }

                if (breakingTimer >= blockBreakTime)
                {
                    switch (lastBreakingBlockKind)
                    {
                        case BlockKind.Default:
                            audioManager.PlayOneShot(breakSound, true);
                            break;
                        case BlockKind.Liquid:
                            audioManager.PlayOneShot(waterBreakSound, true);
                            break;
                      
                    }
                
                    isPunchingBlock = false;
                    breakingTimer = 0;
                    var tile = tilemap.GetTile(selectedTile);
                    if (tile is ItemRule itemRuleTile)
                    {
                        tile = itemRuleTile.reference;
                    }
                    if (tile is Item itemTile)
                    {  
                        GameObject go = Instantiate(blockBrokenParticles, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                        ParticleSystemRenderer particleSystem = go.GetComponent<ParticleSystemRenderer>();
                        particleSystem.material = itemTile.particleEffect;

                        if(itemTile.specialProperty == SpecialProperty.Explosive)
                        {
                            TNTExplode(new Vector2(selectedTile.x, selectedTile.y), true);
                        }

                    }
                        
                    
                    tilemap.SetTile(selectedTile, null);
                    shadowTilemap.SetTile(selectedTile, null);
                    photonView.RpcSecure(nameof(SetTileRPC), RpcTarget.All,false,  ((Vector3)selectedTile), -1);

                    

                    if (lastBreakEffect != null)
                        Destroy(lastBreakEffect.gameObject);
                    if (lastBreakEffect2 != null)
                        Destroy(lastBreakEffect2.gameObject);
                }

                
            }
            else
            {
                breakingTimer = 0;
                if (lastBreakEffect != null)
                    Destroy(lastBreakEffect.gameObject);
                if (lastBreakEffect2 != null)
                    Destroy(lastBreakEffect2.gameObject);
                lastBreakingBlockPos = selectedTile;
            }


        }
        else
        {
            isPunchingBlock = false;
            breakingTimer = 0;
            if (lastBreakEffect != null)
                Destroy(lastBreakEffect.gameObject);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int selectedTile = tilemap.WorldToCell(point);
            lastBreakingBlockPos = selectedTile;
            if (selectedTile.x <= tilemap.WorldToCell(transform.position).x + breakRangeX & selectedTile.x >= tilemap.WorldToCell(transform.position).x - breakRangeX & selectedTile.y <= tilemap.WorldToCell(transform.position).y + breakRangeY & selectedTile.y >= tilemap.WorldToCell(transform.position).y - breakRangeY & !gunController.equippedWeapon)
            {

                if(tilemap.GetTile(selectedTile) != null)
                {
                    lastBreakingBlockPos = selectedTile;
                    //  lastBreakEffect2 = Instantiate(breakPiecesEffect, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                    var tile = tilemap.GetTile(selectedTile);
                    if (tile is ItemRule itemRuleTile)
                    {
                        tile = itemRuleTile.reference;
                    }
                    if (tile is Item itemTile)
                    {
                        if (itemTile.specialProperty != SpecialProperty.Unbreakable)
                        {
                            GameObject go = Instantiate(breakPiecesEffect, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                            lastBreakEffect2 = go;
                            ParticleSystem particleSystem = go.transform.GetChild(1).GetComponent<ParticleSystem>();
                            var main = particleSystem.main;
                            main.startColor = itemTile.particleColor;
                        }

                    }
                } else
                {
                   // audioManager.PlayOneShot("PunchMiss");
                }

               
            }

          
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (lastBreakEffect2 != null)
            {
                Destroy(lastBreakEffect2.gameObject);
                var tile = tilemap.GetTile(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                if (tile is ItemRule itemRuleTile)
                {
                    tile = itemRuleTile.reference;
                }
                if (tile is Item itemTile)
                {

                    GameObject go = Instantiate(breakParticlesDone, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                 //   lastBreakEffect2 = go;
                    ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();
                    var main = particleSystem.main;
                    main.startColor = itemTile.particleColor;
                }
              //  Instantiate(breakParticlesDone, tilemap.CellToWorld(lastBreakingBlockPos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            }
                
        }

        if (Input.GetMouseButtonDown(1)) //downsuz da çalışıyor, ayrı bir ayar olabilir
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int selectedTile = tilemap.WorldToCell(point);
            if (selectedBlock != null && tilemap.GetTile(selectedTile) == null && selectedTile != tilemap.WorldToCell(transform.position) && selectedTile.x <= tilemap.WorldToCell(transform.position).x + placeRangeX & selectedTile.x >= tilemap.WorldToCell(transform.position).x - placeRangeX & selectedTile.y <= tilemap.WorldToCell(transform.position).y + placeRangeY & selectedTile.y >= tilemap.WorldToCell(transform.position).y - placeRangeY) {

                if(selectedBlock.ruleReference == null) //normal tile
                {
                    tilemap.SetTile(selectedTile, selectedBlock);
                    shadowTilemap.SetTile(selectedTile, selectedBlock);
                } else //rule tile
                {
                    tilemap.SetTile(selectedTile, selectedBlock.ruleReference);
                    shadowTilemap.SetTile(selectedTile, selectedBlock.ruleReference);
                }
                
                photonView.RpcSecure(nameof(SetTileRPC), RpcTarget.All, false, ((Vector3)selectedTile), selectedBlock.id);
                audioManager.PlayOneShot("BlockPlace" + Random.Range(1, 3).ToString(), true);
                itemsManager.BlockPlaced();
            } //blok seçmemiş de olabiliriz
           
        }

        if (isPunchingBlock)
        {
            BlockBreakAnimation();
        }
        else
        {

        }

        var tilee = tilemap.GetTile(tilemap.WorldToCell(transform.position));
        if (tilee is ItemRule itemRuleTilee)
        {
            tilee = itemRuleTilee.reference;
        }
        if (tilee is Item itemTilee)
        {
            if(itemTilee.specialProperty == SpecialProperty.CheckpointInactive)
            {
                if(pcont.spawnpoint.x != tilemap.WorldToCell(transform.position).x)
                {
                    if(pcont.spawnpoint.y != tilemap.WorldToCell(transform.position).y)
                    {
                        if(hasHitACheckpoint)
                        tilemap.SetTile(tilemap.WorldToCell(pcont.spawnpoint), checkpointInactive);
                        //Instantiate(checkpointPartices, tilemap.WorldToCell(transform.position) + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        audioManager.PlayOneShot("Checkpoint", false);
                    }
                }
                hasHitACheckpoint = true;
                pcont.spawnpoint = tilemap.WorldToCell(transform.position) + new Vector3(0.5f, 0.5f);
                tilemap.SetTile(tilemap.WorldToCell(transform.position), checkpointActive);
                shadowTilemap.SetTile(tilemap.WorldToCell(transform.position), checkpointActive);
            }

            



            if (itemTilee.blockKind == BlockKind.Liquid)
            {
                inLiquid = true;
            }

            if(itemTilee.colliderType != Tile.ColliderType.None) //inside a collider, check for suffocation
            {
                insideSolidBlock = true;
            } else
            {
                insideSolidBlock = false;
                suffocationTimer = 0;
            }

        } else
        {
            inLiquid = false;
        }

        if(insideSolidBlock && pcont.isAlive)
        {
            if(tilee == null)
            {
                suffocationTimer = 0;
                insideSolidBlock = false;
            }
            else
            {
                suffocationTimer += Time.deltaTime;
               

                if (suffocationTimer >= suffocationMaxTime)
                {
                    //Die by suffocation
                    pcont.Die();
                    suffocationTimer = 0;
                }
            }


           
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            //check the tile we're on
            var tileee = tilemap.GetTile(tilemap.WorldToCell(transform.position));
            if (tileee is ItemRule itemRuleTileee)
            {
                tileee = itemRuleTileee.reference;
            }
            if (tileee is Item itemTileee)
            {
                if (itemTileee.npcProperty == NPCProperty.Villager)
                {
                    shop.gameObject.SetActive(true);
               //     audioManager.PlayOneShot("", false);
                    shop.OnOpened();
                }          

            }
           
        }

        pcont.anim.SetBool("isSuffocating", insideSolidBlock);

    }

    private readonly Vector3Int[] neighbourPositions =
{
        Vector3Int.zero, //For fireball
    Vector3Int.up,
    Vector3Int.right,
    Vector3Int.down,
    Vector3Int.left,
    Vector3Int.up + Vector3Int.right,
    Vector3Int.up + Vector3Int.left,
    Vector3Int.down + Vector3Int.right,
    Vector3Int.down + Vector3Int.left,

};

   

    public void SetLiteralBlockInHand(int itemId)
    {
        if (itemId == -1)
            literalBlockInHand.sprite = null;
        else
            literalBlockInHand.sprite = allItems[itemId].sprite;

        photonView.RPC(nameof(SetLiteralBlockInHandRPC), RpcTarget.Others, itemId);
    }

    [PunRPC]
    public void SetLiteralBlockInHandRPC(int itemId)
    {
        if(itemId == -1)
            literalBlockInHand.sprite = null;
        else
        literalBlockInHand.sprite = allItems[itemId].sprite;
    }


    public void TNTExplode(Vector2 gameOjectPosition, bool explosionEffect)
    {
        
            var grid = tilemap.GetComponentInParent<GridLayout>();
            var gridPosition = grid.WorldToCell(gameOjectPosition);

            var sTiles = new List<TileBase>();
            foreach (var neighbourPosition in neighbourPositions)
            {
                var position = gridPosition + neighbourPosition;

                if (tilemap.HasTile(position))
                {
                    var neighbour = tilemap.GetTile(position);
                    sTiles.Add(neighbour);

                if (neighbour is Item item2)
                {
                    if(item2.specialProperty != SpecialProperty.Unbreakable)
                    {
                        tilemap.SetTile(position, null);
                        shadowTilemap.SetTile(position, null);
                        photonView.RpcSecure(nameof(SetTileRPC), RpcTarget.All, false, ((Vector3)position), -1);
                    }
                } else
                {
                    tilemap.SetTile(position, null);
                    shadowTilemap.SetTile(position, null);
                    photonView.RpcSecure(nameof(SetTileRPC), RpcTarget.All, false, ((Vector3)position), -1);

                }



                if (neighbour is Item item)
                {
                    if (item.specialProperty == SpecialProperty.Explosive)
                        TNTExplode(new Vector2(position.x, position.y), explosionEffect);


                }

                }
            }
        if (explosionEffect)
            ExplosionEffect(gameOjectPosition);
        audioManager.PlayOneShot("Explosion", false); //earrape
    }

    public void ExplosionEffect(Vector2 pos)
    {
        Instantiate(explosionEffectPrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
    }

    [PunRPC]
    void SetTileRPC(Vector3 pos, int itemId) //-1 id is empty
    {
        if(tilemap == null)
            tilemap = GameObject.FindGameObjectWithTag("MainTilemap").GetComponent<Tilemap>();
        if (shadowTilemap == null)
            shadowTilemap = GameObject.FindGameObjectWithTag("ShadowTilemap").GetComponent<Tilemap>();

        if (itemId == -1) // -1 id means empty
        {
            var tile = tilemap.GetTile(Vector3Int.FloorToInt(pos));
            if(tile != null)
            {
                if (tile is ItemRule itemRuleTile)
                {
                    tile = itemRuleTile.reference;
                }
                if (tile is Item itemTile)
                {
                    GameObject go2 = Instantiate(blockBrokenParticles, tilemap.CellToWorld(Vector3Int.FloorToInt(pos)) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                    ParticleSystemRenderer particleSystem2 = go2.GetComponent<ParticleSystemRenderer>();
                    particleSystem2.material = itemTile.particleEffect;


                    if (itemTile.specialProperty == SpecialProperty.Explosive)
                    {
                        Instantiate(explosionEffectPrefab, pos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                        audioManager.PlayOneShot("Explosion", false);
                    }
                }
            }
           
            tilemap.SetTile(Vector3Int.FloorToInt(pos), null);
            shadowTilemap.SetTile(Vector3Int.FloorToInt(pos), null);


        } else
        {
            if (allItems[itemId].ruleReference == null)
            { //it's not a rule tile
                tilemap.SetTile(Vector3Int.FloorToInt(pos), allItems[itemId]);
                shadowTilemap.SetTile(Vector3Int.FloorToInt(pos), allItems[itemId]);
               
                Debug.Log("Normal tile put");

                

            } else
            { //it is a rule tile
                tilemap.SetTile(Vector3Int.FloorToInt(pos), allItems[itemId].ruleReference);
                shadowTilemap.SetTile(Vector3Int.FloorToInt(pos), allItems[itemId].ruleReference);
                
                Debug.Log("Rule tile put");

               
            }

            Instantiate(blockPuffParticles, tilemap.CellToWorld(Vector3Int.FloorToInt(pos)) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
     //       ParticleSystemRenderer particleSystem2 = go2.GetComponent<ParticleSystemRenderer>();
          //  particleSystem2.material = allItems[itemId].particleEffect;

        }
    }

    void BlockBreakAnimation()
    {
        if (spriteRenderer.flipX)
        {
            rightArm.eulerAngles += new Vector3(0, 0, breakArmRotateSpeed * Time.deltaTime);
        }
        else
        {
            rightArm.eulerAngles += new Vector3(0, 0, -breakArmRotateSpeed * Time.deltaTime);
        }

    }

     void CheckForSuffucation()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {      

        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            return;

        /*
        Debug.Log("col.collider: " + col.collider.name);
        Debug.Log("col.transform: " + col.transform.name);
        Debug.Log("col.otherCollider.transform.name" + col.otherCollider.transform.name);
        Debug.Log("col.collider.transform.name" + col.collider.transform.name);
        */

        if (col.collider.CompareTag("TouchKillGO"))
        {
            if (pcont.isAlive)
                pcont.Die();
        }

        /*
            for (int i = 0; i < col.contacts.Length; i++)
        {

            //   Debug.Log(col.GetContact(i).point);
            if (tilemap == null)
                return;

            var tile = tilemap.GetTile(tilemap.WorldToCell(col.GetContact(i).point));
            if (tile is ItemRule itemRuleTile)
            {
                tile = itemRuleTile.reference;
            }
            if (tile is Item itemTile)
            {
                if (itemTile.hazardType == Hazard.TouchKill)
                {
                    if(pcont.isAlive)
                    pcont.Die();
                }

            }
        }
        */
    }


   


   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            return;
        if (collision.CompareTag("Portal"))
        {
            Debug.Log("Hit portal!");
                transform.position = tilemap.CellToWorld(tilemap.WorldToCell(collision.GetComponent<PortalTileGO>().destination)) + new Vector3(0.5f, 0.5f, 0);
            audioManager.PlayOneShot("Portal", true);
            
        }

        if (collision.CompareTag("GeneratorGO"))
        {
            Debug.Log("Entered a generator.");



        }

        if (collision.CompareTag("VillagerGO"))
        {
            inVillagerGO = true;



        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            return;
       

        if (collision.CompareTag("GeneratorGO"))
        {
            Debug.Log("Exited the generator.");



        }

        if (collision.CompareTag("VillagerGO"))
        {
            inVillagerGO = false;
            shop.CloseShop(false);


        }
    }


    void SortAllItemsArray()
    {
        allItems = new Item[ Resources.LoadAll<Item>("Items/").Length];
        allItems = Resources.LoadAll<Item>("Items/");
          Array.Sort(allItems, delegate (Item x, Item y) { return x.id.CompareTo(y.id); });
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
    }
}


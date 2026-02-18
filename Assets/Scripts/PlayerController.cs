using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    TilesInteraction tilei;
    [HideInInspector]
   public Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer, raRenderer, laRenderer ,raRendererShadow, laRendererShadow;
    public SpriteRenderer spriteRendererShadow;
    public Transform groundCheck, groundCheckL, groundCheckR, rightArm, leftArm;
    public float speed, jumpPower, armsLerpMultiplier, armsMaxJumpAngle;
    public bool isGrounded, isJumping, isWalking;
    [HideInInspector]
    public bool mobileLeft, mobileRight, mobileJump;
    public bool playedOnAndroid, isAlive;
    public GameObject mobileLeftArrow, mobileRightArrow, mobileJumpArrow;
    float airTime;
    bool airTimeIsStarted;
    public GameObject inventory, playerDieParticles, bloodParticles;
    [HideInInspector]
    public ItemsManager itemsManager;
    [HideInInspector]
    public AudioManager audioManager;
    GunController gunController;
    ShadowEffect shadowEffect;
    BoxCollider2D boxCollider;
    public Vector2 spawnpoint;
    [SerializeField]
   public bool receivedGrounded, receivedIsAlive, receivedIsWalking, receivedSpriteRendererFlip, receivedIsPunchingBlock;
    public Transform clothesParent;
    SpriteRenderer[] clothesSRs;
    public Shop shop;
    


    void Awake()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            playedOnAndroid = true;
        } else
        {
            playedOnAndroid = false;
        }
    }

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        shadowEffect = GetComponent<ShadowEffect>();
        gunController = GetComponent<GunController>();
        itemsManager = inventory.GetComponent<ItemsManager>();
        tilei = GetComponent<TilesInteraction>();
        airTime = Time.time;
        raRenderer = rightArm.gameObject.GetComponent<SpriteRenderer>();
        laRenderer = leftArm.gameObject.GetComponent<SpriteRenderer>();
        raRendererShadow = raRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>(); 
        laRendererShadow = laRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>(); 
        anim = GetComponent<Animator>();

        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = transform.parent.GetComponentInChildren<AudioManager>();


        clothesSRs = new SpriteRenderer[clothesParent.childCount];
        for (int i = 0; i < clothesParent.childCount; i++)
        {
            clothesSRs[i] = clothesParent.GetChild(i).GetComponent<SpriteRenderer>();
   

        }




        if (Application.platform == RuntimePlatform.Android)
        {
            playedOnAndroid = true;
        } else
        {
            playedOnAndroid = false;
        }

        if(!playedOnAndroid)
        {
            mobileLeftArrow.SetActive(false);
            mobileRightArrow.SetActive(false);
            mobileJumpArrow.SetActive(false);
        }
        isAlive = true;
    }

    void Update()
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
        {



            return;
        }
           


        if(transform.position.y < -20)
        {
            if(isAlive)
            Die();
        }
        
        if (!isGrounded)
            {
                anim.Play("Jump");
                airTimeIsStarted = true;
                if (airTimeIsStarted)
                {
                    airTime += Time.deltaTime;
                }
                if(spriteRenderer.flipX == false)
            {

                if (gunController.equippedWeapon)
                    return;

                if (!tilei.isPunchingBlock)
                rightArm.eulerAngles = AngleLerp(new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, rightArm.eulerAngles.z), new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, armsMaxJumpAngle), airTime * armsLerpMultiplier);
                leftArm.eulerAngles = AngleLerp(new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, leftArm.eulerAngles.z), new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, armsMaxJumpAngle), airTime * armsLerpMultiplier);
            } else
            {

                if (gunController.equippedWeapon)
                    return;

                if (!tilei.isPunchingBlock)
                    rightArm.eulerAngles = AngleLerp(new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, rightArm.eulerAngles.z), new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, -armsMaxJumpAngle), airTime * armsLerpMultiplier);
                leftArm.eulerAngles = AngleLerp(new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, leftArm.eulerAngles.z), new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, -armsMaxJumpAngle), airTime * armsLerpMultiplier);
            }
            

            } else
        {
            airTime = 0;
            airTimeIsStarted = false;

            if (gunController.equippedWeapon)
                return;
            if(rightArm.eulerAngles.z != 0)
            rightArm.eulerAngles = AngleLerp(new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, rightArm.eulerAngles.z), new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, 0), Time.deltaTime * armsLerpMultiplier * 2);
            if (leftArm.eulerAngles.z != 0)
                leftArm.eulerAngles = AngleLerp(new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, leftArm.eulerAngles.z), new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, 0), Time.deltaTime * armsLerpMultiplier * 2);
        }

       if(Input.GetKeyDown(KeyCode.F) && !IsTypingSomewhere())
        {
            anim.SetTrigger("hit");
        }

        
    }

     void FixedUpdate()
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
        {
            //non local and online
            if(receivedIsAlive)
            {
                if (!receivedSpriteRendererFlip)
                {
                    spriteRenderer.flipX = false;
                    laRenderer.flipX = false;
                    raRenderer.flipX = false;
                    spriteRendererShadow.flipX = false;
                    laRendererShadow.flipX = false;
                    raRendererShadow.flipX = false;
                    FlipAllClothes(false);
                    rightArm.localPosition = new Vector3(-0.15625f, 0.03125f, 0);
                    leftArm.localPosition = new Vector3(0.125f, 0.03125f, 0);
                }
                else
                {
                    spriteRenderer.flipX = true;
                    laRenderer.flipX = true;
                    raRenderer.flipX = true;
                    spriteRendererShadow.flipX = true;
                    laRendererShadow.flipX = true;
                    raRendererShadow.flipX = true;
                    FlipAllClothes(true);
                    rightArm.localPosition = new Vector3(0.15625f, 0.03125f, 0);
                    leftArm.localPosition = new Vector3(-0.125f, 0.03125f, 0);
                }
                if (receivedGrounded)
                {
                    if(receivedIsWalking)
                    anim.Play("Walk");
                    else
                        anim.Play("Idle");
                    if (rightArm.eulerAngles.z != 0)
                        rightArm.eulerAngles = AngleLerp(new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, rightArm.eulerAngles.z), new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, 0), Time.deltaTime * armsLerpMultiplier * 2);
                    if (leftArm.eulerAngles.z != 0)
                        leftArm.eulerAngles = AngleLerp(new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, leftArm.eulerAngles.z), new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, 0), Time.deltaTime * armsLerpMultiplier * 2);

                } else
                {
                    anim.Play("Jump");
                    if(receivedSpriteRendererFlip == false)
                    {
                        if (!receivedIsPunchingBlock)
                            rightArm.eulerAngles = AngleLerp(new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, rightArm.eulerAngles.z), new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, armsMaxJumpAngle), airTime * armsLerpMultiplier);
                        leftArm.eulerAngles = AngleLerp(new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, leftArm.eulerAngles.z), new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, armsMaxJumpAngle), airTime * armsLerpMultiplier);
                    } else
                    {
                        if (!receivedIsPunchingBlock)
                            rightArm.eulerAngles = AngleLerp(new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, rightArm.eulerAngles.z), new Vector3(rightArm.eulerAngles.x, rightArm.eulerAngles.y, -armsMaxJumpAngle), airTime * armsLerpMultiplier);
                        leftArm.eulerAngles = AngleLerp(new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, leftArm.eulerAngles.z), new Vector3(leftArm.eulerAngles.x, leftArm.eulerAngles.y, -armsMaxJumpAngle), airTime * armsLerpMultiplier);
                    }

                }
                    
            }

            

            return;
        }
           


        // eski ve yeni box collider settings
        //-0.0006320328 , 0.9406774  YENI:  offset y: 0.008019626 size y: 0.9233741  
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Block")) ||
            (Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Block"))) ||
         //   (Physics2D.Linecast(transform.position, groundCheck.position + new Vector3(0, 0.11f, 0), 1 << LayerMask.NameToLayer("Block"))) ||
          //  (Physics2D.Linecast(transform.position, groundCheck.position + new Vector3(0, -0.11f, 0), 1 << LayerMask.NameToLayer("Block"))) ||
            (Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Block"))))
        {
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }

      
        if(CrossPlatformInputManager.GetButtonDown("Right") && isAlive)
        {

        //    photonView.RPC(nameof(PlayerDirectionChangeRightRPC), RpcTarget.Others);
        }

        if (CrossPlatformInputManager.GetButtonDown("Left") && isAlive)
        {

          //  photonView.RPC(nameof(PlayerDirectionChangeLeftRPC), RpcTarget.Others);
        }

        if (CrossPlatformInputManager.GetButton("Right") && isAlive && CanMoveWithInput() && !IsTypingSomewhere())
        {
            isWalking = true;
           
            rigid.linearVelocity = new Vector2(speed, rigid.linearVelocity.y);
            spriteRenderer.flipX = false;
            laRenderer.flipX = false;
            raRenderer.flipX = false;
            spriteRendererShadow.flipX = false;
            laRendererShadow.flipX = false;
            raRendererShadow.flipX = false;
            FlipAllClothes(false);
            rightArm.localPosition = new Vector3(-0.15625f, 0.03125f, 0);
            leftArm.localPosition = new Vector3(0.125f, 0.03125f, 0);
            if (isGrounded)
                anim.Play("Walk");

        }
        else if (CrossPlatformInputManager.GetButton("Left") && isAlive && CanMoveWithInput() && !IsTypingSomewhere())
        {
            isWalking = true;

                rigid.linearVelocity = new Vector2(-speed, rigid.linearVelocity.y);
            spriteRenderer.flipX = true;
            laRenderer.flipX = true;
            raRenderer.flipX = true;
            spriteRendererShadow.flipX = true;
            laRendererShadow.flipX = true;
            raRendererShadow.flipX = true;
            FlipAllClothes(true);
            rightArm.localPosition = new Vector3(0.15625f, 0.03125f, 0);
            leftArm.localPosition = new Vector3(-0.125f, 0.03125f, 0);
            if (isGrounded)
                anim.Play("Walk");
        }
        else
        {
            isWalking = false;
            if (isGrounded)
                anim.Play("Idle");
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocity.y);
        }


        /* if (Input.GetKey(KeyCode.W) || mobileJump)
         {
             if(isGrounded)
             {
                 rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
                 isJumping = true;
             }

         }
        */

        if (CrossPlatformInputManager.GetButton("Jump1") && isAlive && CanMoveWithInput() && !IsTypingSomewhere())
        {
            if (isGrounded)
            {

                    rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpPower);
                isJumping = true;
            }

        }

        if(CrossPlatformInputManager.GetButton("Jump1") && isAlive && isGrounded && CanMoveWithInput() && !IsTypingSomewhere())
            audioManager.PlayOneShotIfNotPlaying("Jump", true);
        /* if (playedOnAndroid)
         {
             if (isJumping)
             {
                 rigid.velocity = new Vector2(rigid.velocity.x, 0);
                 isJumping = false;
             }
         } else*/
        //{
        if (isJumping &/* !Input.GetKey(KeyCode.W)*/ !CrossPlatformInputManager.GetButton("Jump1") && CanMoveWithInput() && !IsTypingSomewhere())
            {

                rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0);
                isJumping = false;
            }
       // }
        


        if (rigid.linearVelocity.y <= 0)
        {
            isJumping = false;
        }

    }

    public void MobileLeftArrowDown()
    {
        mobileLeft = true;
    }

    public void MobileLeftArrowUp()
    {
        mobileLeft = false;
    }

    public void MobileRightArrowDown()
    {
        mobileRight = true;
    }

    public void MobileRightArrowUp()
    {
        mobileRight = false;
    }

    public void MobileJumpArrowDown()
    {
        mobileJump = true;
    }

    public void MobileJumpArrowUp()
    {
        mobileJump = false;
    }

    Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    public void FlipAllClothes(bool flipX)
    {
        for (int i = 0; i < clothesSRs.Length; i++)
        {
            clothesSRs[i].flipX = flipX;
        }
        tilei.literalBlockInHand.flipY = flipX;
        if (flipX)
            tilei.literalBlockInHand.transform.localPosition = new Vector3(-tilei.lblockinhandoffset, tilei.literalBlockInHand.transform.localPosition.y, tilei.literalBlockInHand.transform.localPosition.z);
        else
            tilei.literalBlockInHand.transform.localPosition = new Vector3(tilei.lblockinhandoffset, tilei.literalBlockInHand.transform.localPosition.y, tilei.literalBlockInHand.transform.localPosition.z);

    }

    public void EnabledAllClothesSRs(bool value)
    {
        for (int i = 0; i < clothesSRs.Length; i++)
        {
            clothesSRs[i].enabled = value;
        }
        tilei.literalBlockInHand.enabled = value;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            return;

        if (collision.tag == "DroppedItem")
        {
            if(collision.GetComponent<DroppedItem>().isTakeable)
            {
                itemsManager.AddItem(collision.GetComponent<DroppedItem>().itemInside, collision.GetComponent<DroppedItem>().amount, collision.gameObject);
                shop.UpdateBalanceAndText();
            }
        }
    }



    public void Die() //always executed locally
    {
        isAlive = false;
        Instantiate(playerDieParticles, transform.position, Quaternion.identity);
        Instantiate(bloodParticles, transform.position, Quaternion.identity);
        audioManager.PlayOneShot("SpikeDie1", false);
        rightArm.gameObject.SetActive(false);
        leftArm.gameObject.SetActive(false);
        spriteRenderer.enabled = false;
        spriteRendererShadow.enabled = false;
        EnabledAllClothesSRs(false);
        Invoke("Spawn", 2);
     //   shadowEffect.transShadow.gameObject.SetActive(false);
     
        boxCollider.enabled = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        photonView.RPC(nameof(DieRPC), RpcTarget.Others);
    }

    [PunRPC]
    void DieRPC()
    {
       
        Instantiate(playerDieParticles, transform.position, Quaternion.identity);
        Instantiate(bloodParticles, transform.position, Quaternion.identity);
        audioManager.PlayOneShot("SpikeDie1", false);
        rightArm.gameObject.SetActive(false);
        leftArm.gameObject.SetActive(false);
        spriteRenderer.enabled = false;
        spriteRendererShadow.enabled = false;
        EnabledAllClothesSRs(false);
        // shadowEffect.transShadow.gameObject.SetActive(false);
        boxCollider.enabled = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Spawn()//Invoked after death  //always executed locally
    {
        transform.position = spawnpoint;
        isAlive = true;
        audioManager.PlayOneShot("Respawn", false);
        spriteRenderer.enabled = true;
        spriteRendererShadow.enabled = true;
        rightArm.gameObject.SetActive(true);
        leftArm.gameObject.SetActive(true);
        EnabledAllClothesSRs(true);
      //  shadowEffect.transShadow.gameObject.SetActive(true);        
        boxCollider.enabled = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        photonView.RPC(nameof(SpawnRPC), RpcTarget.Others);
    }

    [PunRPC]
    void SpawnRPC()
    {
        transform.position = spawnpoint;
        audioManager.PlayOneShot("Respawn", false);
        spriteRenderer.enabled = true;
        spriteRendererShadow.enabled = true;
        rightArm.gameObject.SetActive(true);
        leftArm.gameObject.SetActive(true);
        EnabledAllClothesSRs(true);
      //  shadowEffect.transShadow.gameObject.SetActive(true);
        boxCollider.enabled = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    /*
    [PunRPC]
    void PlayerDirectionChangeRightRPC()
    {
        spriteRenderer.flipX = false;
        laRenderer.flipX = false;
        raRenderer.flipX = false;
        rightArm.localPosition = new Vector3(-0.15625f, 0.03125f, 0);
        leftArm.localPosition = new Vector3(0.125f, 0.03125f, 0);
    }

    
    [PunRPC]
    void PlayerDirectionChangeLeftRPC()
    {
        spriteRenderer.flipX = true;
        laRenderer.flipX = true;
        raRenderer.flipX = true;
        rightArm.localPosition = new Vector3(0.15625f, 0.03125f, 0);
        leftArm.localPosition = new Vector3(-0.125f, 0.03125f, 0);
    }
    */
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if(stream.IsWriting)
        {
            //sending
            stream.SendNext(spriteRenderer.flipX);
            stream.SendNext(isWalking);
            stream.SendNext(isAlive);
            stream.SendNext(isGrounded);
            stream.SendNext(tilei.isPunchingBlock);
        }
        if(stream.IsReading)
        {
            //receiving
           receivedSpriteRendererFlip =  (bool)stream.ReceiveNext();
            receivedIsWalking = (bool)stream.ReceiveNext();
            receivedIsAlive = (bool)stream.ReceiveNext();
            receivedGrounded = (bool)stream.ReceiveNext();
            receivedIsPunchingBlock = (bool)stream.ReceiveNext();
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("Room properties updated! Gamemode: " + propertiesThatChanged["gamemode"]);

       


    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //event
    }

    //STATE MANAGEMENT

    public bool isShopOpen, isTypingOnChat, DevFreeze, freezed;

    public bool IsBusyWithUI()
    {
        if(isShopOpen || isTypingOnChat)
        {
            return true;
        }
        return false;
    }

    public bool CanMoveWithInput()
    {
        if (!isAlive || DevFreeze || freezed)
            return false;

        

       
        return true;
    }

    public bool CanUseConsumables()
    {
        if (!isAlive || DevFreeze)
            return false;




        return true;
    }

    public bool IsTypingSomewhere()
    {
        if (isTypingOnChat)
            return true;

        return false;

    }


}

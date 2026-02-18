using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPun
{
    public float rotationSpeed;

    ItemUser creator;

    [Tooltip("Set by ItemUser.cs")]
    public ItemUser Creator
    {
        get { return this.creator; }
        set { this.creator = value; creatorTI = Creator.GetComponent<TilesInteraction>(); creatorCM = Creator.transform.parent.GetComponentInChildren<ChatManager>(); Physics2D.IgnoreCollision(transform.GetComponent<CircleCollider2D>(), Creator.GetComponent<Collider2D>()); }
    }
    TilesInteraction creatorTI;
    ChatManager creatorCM;


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //This func calls both on MC and visual

        if(collision.transform.CompareTag("MainTilemap"))
        {
            creatorCM.Chat("The fireball I've thrown hit some blocks! Yay!", MessageType.PlayerMessage);
            if (PhotonNetwork.IsMasterClient)
                creatorTI.TNTExplode(collision.GetContact(0).point, true);
            else
                creatorTI.ExplosionEffect(collision.GetContact(0).point);

            Destroy(gameObject);
        }
    }

  

}

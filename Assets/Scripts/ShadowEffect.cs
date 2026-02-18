using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowEffect : MonoBehaviour
{
    public Vector2 offset;
    SpriteRenderer sprRndCaster;
    SpriteRenderer sprRndShadow;
    TilemapRenderer tmpRndCaster, tmpRndShadow;
    Tilemap tmpShadow, tmpCaster;

    public Transform transCaster, transShadow;

    public Material shadowMat;
    public Color shadowColor;

    public bool isTilemap;

    void Start()
    {
        if(!isTilemap)
        {
            transCaster = transform;
            transShadow = new GameObject().transform;
            transShadow.parent = transCaster;
            transShadow.gameObject.name = "shadow";
            transShadow.localRotation = Quaternion.identity;

            sprRndCaster = GetComponent<SpriteRenderer>();
            sprRndShadow = transShadow.gameObject.AddComponent<SpriteRenderer>();

            sprRndShadow.material = shadowMat;
            sprRndShadow.color = shadowColor;
            sprRndShadow.sortingLayerName = sprRndCaster.sortingLayerName;
            sprRndShadow.sortingOrder = sprRndCaster.sortingOrder - 5;
        } else
        {
            transCaster = transform;
            transShadow = new GameObject().transform;
            transShadow.parent = transCaster.parent;
            transShadow.gameObject.name = "tilemap shadow";
            transShadow.localRotation = Quaternion.identity;
            tmpShadow = transShadow.gameObject.AddComponent<Tilemap>();
            tmpCaster = GetComponent<Tilemap>();

            tmpRndCaster = GetComponent<TilemapRenderer>();
            tmpRndShadow = transShadow.gameObject.AddComponent<TilemapRenderer>();
            tmpRndShadow.material = shadowMat;
            tmpShadow.color = shadowColor;
            tmpRndShadow.sortingLayerName = tmpRndCaster.sortingLayerName;
            tmpRndShadow.sortingOrder = tmpRndCaster.sortingOrder - 2;
        }
       
    }

    void LateUpdate()
    {
        if (!isTilemap)
        {
            transShadow.position = new Vector2(transCaster.position.x + offset.x, transCaster.position.y + offset.y);
            sprRndShadow.flipX = sprRndCaster.flipX;
            sprRndShadow.sprite = sprRndCaster.sprite;
        } else
        {
            tmpRndShadow = tmpRndCaster;
            tmpShadow = tmpCaster;
        }
         
    }
}

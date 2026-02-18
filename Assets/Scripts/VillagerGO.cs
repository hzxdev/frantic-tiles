using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerGO : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.CompareTag("ShadowTilemap"))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateGrid : MonoBehaviour
{

    public GameObject prefab;
    public int numberToCreate;

    void Start()
    {
        Populate();
    }

    void Update()
    {
        
    }

    void Populate()
    {
        GameObject newObj;

        for(int i = 1; i < numberToCreate;i++)
        {
            newObj = Instantiate(prefab, transform);
        }
    }

}

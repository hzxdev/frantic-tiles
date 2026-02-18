using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class Manager : MonoBehaviour
{
    public GameObject playerPrefab;
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position, playerPrefab.transform.rotation);
        }
        else
        {
            PhotonNetwork.OfflineMode = true;
            Instantiate(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

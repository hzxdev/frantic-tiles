using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerName : MonoBehaviourPun
{
    public TextMeshProUGUI usernameText;

    void Start()
    {
        if(photonView.IsMine)
        PhotonNetwork.NickName = "player" + photonView.CreatorActorNr.ToString();
        usernameText.text = PhotonNetwork.NickName;
     // big problem here
    }


    void Update()
    {
        
    }


}

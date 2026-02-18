using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviourPun
{

    public Transform player, backgroundParent;
    Camera cam;
    public float minZoom, maxZoom, zoomSensivity, zoomSensivityMobile;
    PlayerController playerController;
    public TextMeshProUGUI roomOwnerText;



    void Start()
    {
        cam = GetComponent<Camera>();
        playerController = player.gameObject.GetComponent<PlayerController>();

        if (!photonView.IsMine && !PhotonNetwork.OfflineMode) //Destroy camera and canvas if not local
        {
            Destroy(transform.parent.GetChild(2).gameObject);
            Destroy(gameObject);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            roomOwnerText.gameObject.SetActive(true);

        } else
        {
            roomOwnerText.gameObject.SetActive(false);

        }
    }

    void Update()
    {
        if (playerController.isAlive)
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        if (Input.GetAxis("Mouse ScrollWheel") > 0 & cam.orthographicSize >= minZoom)
        {
            cam.orthographicSize -= zoomSensivity;
        }

            if (Input.GetAxis("Mouse ScrollWheel") < 0 & cam.orthographicSize <= maxZoom)
            {
                cam.orthographicSize += zoomSensivity;
            }

            if (Input.touchCount == 2 & !playerController.isWalking & !playerController.isJumping)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
                float difference = currentMagnitude - prevMagnitude;
                Zoom(difference * zoomSensivityMobile);
            }
            backgroundParent.localScale = new Vector3(Map(cam.orthographicSize, minZoom, maxZoom, 1, 2.3f), Map(cam.orthographicSize, minZoom, maxZoom, 1, 2.3f), Map(cam.orthographicSize, minZoom, maxZoom, 1, 2.3f));
        

        //minscale = 1 max = 2.3

       
    }

    void Zoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, minZoom, maxZoom);
    }

    float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

}


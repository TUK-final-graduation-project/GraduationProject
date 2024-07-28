using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Resource : MonoBehaviourPun
{
    private bool isBeingMined = false;

    void OnMouseDown()
    {
        if (!isBeingMined)
        {
            StartMining();
        }
    }

    void StartMining()
    {
        if (!isBeingMined)
        {
            isBeingMined = true;
            photonView.RPC("RPC_StartMining", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_StartMining()
    {
        StartCoroutine(MineResource());
    }

    IEnumerator MineResource()
    {
        yield return new WaitForSeconds(3f); // Ã¤±¼ ½Ã°£

        // ÀÚ¿ø È¹µæ ·ÎÁ÷
        Debug.Log("Resource mined!");

        photonView.RPC("RPC_DestroyResource", RpcTarget.All);
    }

    [PunRPC]
    void RPC_DestroyResource()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

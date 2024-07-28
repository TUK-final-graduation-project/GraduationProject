using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemAcquisition : MonoBehaviourPun
{
    public Item item;

    [PunRPC]
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviourPun
{
    [SerializeField]
    private int hp;
    [SerializeField]
    private float destroyTime;
    [SerializeField]
    private SphereCollider col;

    [SerializeField]
    private GameObject go_rock;
    [SerializeField]
    private GameObject go_debris;
    [SerializeField]
    private GameObject go_effect_prefabs;
    [SerializeField]
    public GameObject rock;

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    public void Mining()
    {
        photonView.RPC("RPC_Mine", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_Mine()
    {
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Mining);
        if (hp <= 0)
        {
            photonView.RPC("RPC_Destruction", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void RPC_Destruction()
    {
        col.enabled = false;
        go_rock.SetActive(false);
        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);

        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPosition = go_debris.transform.GetChild(i).gameObject.transform.position;
            PhotonNetwork.Instantiate(rock.name, spawnPosition, Quaternion.identity);
        }

        if (resourceManager != null)
        {
            resourceManager.ScheduleRespawn(go_rock, transform.position);
        }

        Destroy(gameObject);
    }
}

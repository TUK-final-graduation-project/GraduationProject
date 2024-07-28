using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResourceManager : MonoBehaviourPun
{
    [System.Serializable]
    public class PrefabInfo
    {
        public GameObject prefab;
        public int maxCount;
    }

    [SerializeField]
    private List<PrefabInfo> prefabInfos;

    [SerializeField]
    private float mapWidth = 1000f;
    [SerializeField]
    private float mapHeight = 1000f;
    [SerializeField]
    public float respawnTime = 30f;
    [SerializeField]
    private float minSpawnDistance = 5f;
    [SerializeField]
    private float exclusionZoneWidth = 200f;
    [SerializeField]
    private float exclusionZoneHeight = 200f;

    private Dictionary<GameObject, List<GameObject>> activePrefabs = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<Vector3>> respawnQueue = new Dictionary<GameObject, List<Vector3>>();

    void Start()
    {
        foreach (var prefabInfo in prefabInfos)
        {
            activePrefabs[prefabInfo.prefab] = new List<GameObject>();
            respawnQueue[prefabInfo.prefab] = new List<Vector3>();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_SpawnAllPrefabs", RpcTarget.AllBuffered);
        }

        StartCoroutine(RespawnCycle());
    }

    [PunRPC]
    public void RPC_SpawnAllPrefabs()
    {
        foreach (var prefabInfo in prefabInfos)
        {
            for (int i = 0; i < prefabInfo.maxCount; i++)
            {
                Vector3 spawnPosition = GetRandomPosition();
                while (IsPositionOccupied(spawnPosition) || IsWithinExclusionZone(spawnPosition))
                {
                    spawnPosition = GetRandomPosition();
                }
                SpawnPrefab(prefabInfo.prefab, spawnPosition);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-mapWidth / 2, mapWidth / 2);
        float z = Random.Range(-mapHeight / 2, mapHeight / 2);
        return new Vector3(x, 0, z);
    }

    bool IsWithinExclusionZone(Vector3 position)
    {
        float exclusionZoneXMin = -exclusionZoneWidth / 2;
        float exclusionZoneXMax = exclusionZoneWidth / 2;
        float exclusionZoneZMin = -exclusionZoneHeight / 2;
        float exclusionZoneZMax = exclusionZoneHeight / 2;

        return position.x > exclusionZoneXMin && position.x < exclusionZoneXMax && position.z > exclusionZoneZMin && position.z < exclusionZoneZMax;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        foreach (var list in activePrefabs.Values)
        {
            foreach (var obj in list)
            {
                if (Vector3.Distance(obj.transform.position, position) < minSpawnDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void ScheduleRespawn(GameObject prefab, Vector3 position)
    {
        if (!respawnQueue.ContainsKey(prefab))
        {
            respawnQueue[prefab] = new List<Vector3>();
        }
        respawnQueue[prefab].Add(position);
    }

    IEnumerator RespawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);

            foreach (var prefabInfo in prefabInfos)
            {
                GameObject prefab = prefabInfo.prefab;
                List<GameObject> currentPrefabs = activePrefabs[prefab];
                int missingCount = prefabInfo.maxCount - currentPrefabs.Count;

                if (missingCount > 0)
                {
                    for (int i = 0; i < missingCount; i++)
                    {
                        if (respawnQueue[prefab].Count > 0)
                        {
                            Vector3 position = respawnQueue[prefab][0];
                            if (!IsPositionOccupied(position) && !IsWithinExclusionZone(position))
                            {
                                SpawnPrefab(prefab, position);
                            }
                            respawnQueue[prefab].RemoveAt(0);
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    private void RPC_SpawnPrefab(string prefabName, Vector3 position)
    {
        foreach (var prefabInfo in prefabInfos)
        {
            if (prefabInfo.prefab.name == prefabName)
            {
                var spawnedObject = PhotonNetwork.InstantiateSceneObject(prefabInfo.prefab.name, position, Quaternion.identity);
                activePrefabs[prefabInfo.prefab].Add(spawnedObject);
                spawnedObject.AddComponent<PrefabTracker>().resourceManager = this;
                break;
            }
        }
    }

    private void SpawnPrefab(GameObject prefab, Vector3 position)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_SpawnPrefab", RpcTarget.AllBuffered, prefab.name, position);
        }
        else
        {
            var spawnedObject = PhotonNetwork.InstantiateSceneObject(prefab.name, position, Quaternion.identity);
            activePrefabs[prefab].Add(spawnedObject);
            spawnedObject.AddComponent<PrefabTracker>().resourceManager = this;
        }
    }

    public void RemovePrefab(GameObject prefab, GameObject prefabInstance)
    {
        if (activePrefabs.ContainsKey(prefab))
        {
            activePrefabs[prefab].Remove(prefabInstance);
        }
    }

    public void SetMaxCount(GameObject prefab, int count)
    {
        foreach (var prefabInfo in prefabInfos)
        {
            if (prefabInfo.prefab == prefab)
            {
                prefabInfo.maxCount = count;
                break;
            }
        }
    }

    public void PlusMaxCount(GameObject prefab, int count)
    {
        foreach (var prefabInfo in prefabInfos)
        {
            if (prefabInfo.prefab == prefab)
            {
                prefabInfo.maxCount += count;
                break;
            }
        }
    }

    public void MinusMaxCount(GameObject prefab, int count)
    {
        foreach (var prefabInfo in prefabInfos)
        {
            if (prefabInfo.prefab == prefab)
            {
                prefabInfo.maxCount -= count;
                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Resource"))
                {
                    Rock rock = hit.collider.GetComponent<Rock>();
                    if (rock != null)
                    {
                        rock.Mining();
                    }
                }
            }
        }
    }
}
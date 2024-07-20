using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // ������ ���� Ŭ����: �� �������� �ִ� ������ �����ϴ� Ŭ����
    [System.Serializable]
    public class PrefabInfo
    {
        public GameObject prefab;  // ������ ������Ʈ
        public int maxCount;       // �ִ� ���� ����
    }

    [SerializeField]
    private List<PrefabInfo> prefabInfos;  // ������ ���� ����Ʈ

    [SerializeField]
    private float mapWidth = 1000f;          // ���� �ʺ�
    [SerializeField]
    private float mapHeight = 1000f;         // ���� ����
    [SerializeField]
    public float respawnTime = 30f;        // ������ �ֱ� (�� ����)
    [SerializeField]
    private float minSpawnDistance = 5f;   // �ּ� ���� �Ÿ� (�ߺ� ���� ������)

    private Dictionary<GameObject, List<GameObject>> activePrefabs = new Dictionary<GameObject, List<GameObject>>();  // Ȱ��ȭ�� ������ ����Ʈ
    private Dictionary<GameObject, Vector3> respawnQueue = new Dictionary<GameObject, Vector3>();  // ������ ��⿭

    void Start()
    {
        // activePrefabs ��ųʸ� �ʱ�ȭ: �� �����տ� ���� �� ����Ʈ�� ����
        foreach (var prefabInfo in prefabInfos)
        {
            activePrefabs[prefabInfo.prefab] = new List<GameObject>();
        }
        SpawnAllPrefabs();  // ������ ����
        StartCoroutine(RespawnCycle());  // ������ ����Ŭ ����
    }

    void Update()
    {
    }

    // ������ ����
    void SpawnAllPrefabs()
    {
        foreach (var prefabInfo in prefabInfos)
        {
            for (int i = 0; i < prefabInfo.maxCount; i++)
            {
                Vector3 spawnPosition = GetRandomPosition();  // ���� ��ġ ���
                // ��ġ�� �̹� �����Ǿ����� Ȯ��
                while (IsPositionOccupied(spawnPosition))
                {
                    spawnPosition = GetRandomPosition();  // ���ο� ���� ��ġ ���
                }
                // ������ ����
                var spawnedObject = Instantiate(prefabInfo.prefab, spawnPosition, Quaternion.identity);
                activePrefabs[prefabInfo.prefab].Add(spawnedObject);  // ������ �������� ����Ʈ�� �߰�
                spawnedObject.AddComponent<PrefabTracker>().resourceManager = this;  // PrefabTracker ������Ʈ�� �߰��ϰ� ResourceManager�� ������ ����
            }
        }
    }

    // ���� ��ġ ��ȯ
    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-mapWidth / 2, mapWidth / 2);
        float z = Random.Range(-mapHeight / 2, mapHeight / 2);
        return new Vector3(x, 0, z);
    }

    // �־��� ��ġ�� �̹� �����Ǿ����� Ȯ��
    bool IsPositionOccupied(Vector3 position)
    {
        foreach (var list in activePrefabs.Values)
        {
            foreach (var obj in list)
            {
                // �ּ� ���� �Ÿ� �̳��� �ٸ� �������� ������ ����
                if (Vector3.Distance(obj.transform.position, position) < minSpawnDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // ������ ������ ����
    public void ScheduleRespawn(GameObject prefab, Vector3 position)
    {
        if (!respawnQueue.ContainsKey(prefab))
        {
            respawnQueue[prefab] = position;  // ������ ��⿭�� �߰�
        }
    }

    // ������ ����Ŭ�� ó���ϴ� �ڷ�ƾ
    IEnumerator RespawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);  // ������ ������ �ð� ���

            foreach (var prefabInfo in prefabInfos)
            {
                GameObject prefab = prefabInfo.prefab;
                List<GameObject> currentPrefabs = activePrefabs[prefab];
                int missingCount = prefabInfo.maxCount - currentPrefabs.Count;  // ������ ���� ���

                if (missingCount > 0)
                {
                    for (int i = 0; i < missingCount; i++)
                    {
                        if (respawnQueue.ContainsKey(prefab))
                        {
                            Vector3 position = respawnQueue[prefab];  // �������� ��ġ
                            if (!IsPositionOccupied(position))
                            {
                                var spawnedObject = Instantiate(prefab, position, Quaternion.identity);
                                currentPrefabs.Add(spawnedObject);  // ����Ʈ�� �߰�
                                spawnedObject.AddComponent<PrefabTracker>().resourceManager = this;  // PrefabTracker ������Ʈ�� �߰��ϰ� ResourceManager�� ������ ����
                            }
                            respawnQueue.Remove(prefab);  // ��⿭���� ����
                            break;
                        }
                    }
                }
            }
        }
    }

    public void RemovePrefab(GameObject prefab, GameObject prefabInstance)
    {
        if (activePrefabs.ContainsKey(prefab))
        {
            activePrefabs[prefab].Remove(prefabInstance);  // Ȱ��ȭ�� ������ ����Ʈ���� ����
        }
    }

    // Set, Minus, Plus
    // ������ �ִ� ���� ���� ����
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

    // ������ �ִ� ���� ���� ����
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

    // ������ �ִ� ���� ���� ����
    public void MinusMaxCount(GameObject prefab, int count)
    {
        foreach (var prefabInfo in prefabInfos)
        {
            if (prefabInfo.prefab == prefab)
            {
                prefabInfo.maxCount = Mathf.Max(0, prefabInfo.maxCount - count);  // �ּ� 0���� ����
                break;
            }
        }
    }

    // ������ �ֱ� ����
    public void SetRespawnTime(float time)
    {
        respawnTime = time;
    }
    // ������ �ֱ� ����
    public void PlusRespawnTime(float time)
    {
        respawnTime += time;
    }
    // ������ �ֱ� ����
    public void MinusRespawnTime(float time)
    {
        respawnTime -= time;
    }
}



public class PrefabTracker : MonoBehaviour
{
    public ResourceManager resourceManager;  // ResourceManager�� ���� ����

    // �������� ���ŵ� �� ȣ��
    void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.RemovePrefab(this.gameObject, this.gameObject);  // ResourceManager���� ������ ����
            resourceManager.ScheduleRespawn(this.gameObject, this.transform.position);  // ������ ����
        }
    }
}



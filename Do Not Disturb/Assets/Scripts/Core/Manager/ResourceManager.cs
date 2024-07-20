using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // 프리팹 정보 클래스: 각 프리팹의 최대 개수를 저장하는 클래스
    [System.Serializable]
    public class PrefabInfo
    {
        public GameObject prefab;  // 프리팹 오브젝트
        public int maxCount;       // 최대 생성 개수
    }

    [SerializeField]
    private List<PrefabInfo> prefabInfos;  // 프리팹 정보 리스트

    [SerializeField]
    private float mapWidth = 1000f;          // 맵의 너비
    [SerializeField]
    private float mapHeight = 1000f;         // 맵의 높이
    [SerializeField]
    public float respawnTime = 30f;        // 리스폰 주기 (초 단위)
    [SerializeField]
    private float minSpawnDistance = 5f;   // 최소 생성 거리 (중복 생성 방지용)

    private Dictionary<GameObject, List<GameObject>> activePrefabs = new Dictionary<GameObject, List<GameObject>>();  // 활성화된 프리팹 리스트
    private Dictionary<GameObject, Vector3> respawnQueue = new Dictionary<GameObject, Vector3>();  // 리스폰 대기열

    void Start()
    {
        // activePrefabs 딕셔너리 초기화: 각 프리팹에 대해 빈 리스트를 생성
        foreach (var prefabInfo in prefabInfos)
        {
            activePrefabs[prefabInfo.prefab] = new List<GameObject>();
        }
        SpawnAllPrefabs();  // 프리팹 생성
        StartCoroutine(RespawnCycle());  // 리스폰 사이클 시작
    }

    void Update()
    {
    }

    // 프리팹 생성
    void SpawnAllPrefabs()
    {
        foreach (var prefabInfo in prefabInfos)
        {
            for (int i = 0; i < prefabInfo.maxCount; i++)
            {
                Vector3 spawnPosition = GetRandomPosition();  // 랜덤 위치 계산
                // 위치가 이미 점유되었는지 확인
                while (IsPositionOccupied(spawnPosition))
                {
                    spawnPosition = GetRandomPosition();  // 새로운 랜덤 위치 계산
                }
                // 프리팹 생성
                var spawnedObject = Instantiate(prefabInfo.prefab, spawnPosition, Quaternion.identity);
                activePrefabs[prefabInfo.prefab].Add(spawnedObject);  // 생성된 프리팹을 리스트에 추가
                spawnedObject.AddComponent<PrefabTracker>().resourceManager = this;  // PrefabTracker 컴포넌트를 추가하고 ResourceManager를 참조로 설정
            }
        }
    }

    // 랜덤 위치 반환
    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-mapWidth / 2, mapWidth / 2);
        float z = Random.Range(-mapHeight / 2, mapHeight / 2);
        return new Vector3(x, 0, z);
    }

    // 주어진 위치에 이미 생성되었는지 확인
    bool IsPositionOccupied(Vector3 position)
    {
        foreach (var list in activePrefabs.Values)
        {
            foreach (var obj in list)
            {
                // 최소 생성 거리 이내에 다른 프리팹이 있으면 리턴
                if (Vector3.Distance(obj.transform.position, position) < minSpawnDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 프리팹 리스폰 예약
    public void ScheduleRespawn(GameObject prefab, Vector3 position)
    {
        if (!respawnQueue.ContainsKey(prefab))
        {
            respawnQueue[prefab] = position;  // 리스폰 대기열에 추가
        }
    }

    // 리스폰 사이클을 처리하는 코루틴
    IEnumerator RespawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);  // 지정된 리스폰 시간 대기

            foreach (var prefabInfo in prefabInfos)
            {
                GameObject prefab = prefabInfo.prefab;
                List<GameObject> currentPrefabs = activePrefabs[prefab];
                int missingCount = prefabInfo.maxCount - currentPrefabs.Count;  // 부족한 개수 계산

                if (missingCount > 0)
                {
                    for (int i = 0; i < missingCount; i++)
                    {
                        if (respawnQueue.ContainsKey(prefab))
                        {
                            Vector3 position = respawnQueue[prefab];  // 리스폰할 위치
                            if (!IsPositionOccupied(position))
                            {
                                var spawnedObject = Instantiate(prefab, position, Quaternion.identity);
                                currentPrefabs.Add(spawnedObject);  // 리스트에 추가
                                spawnedObject.AddComponent<PrefabTracker>().resourceManager = this;  // PrefabTracker 컴포넌트를 추가하고 ResourceManager를 참조로 설정
                            }
                            respawnQueue.Remove(prefab);  // 대기열에서 제거
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
            activePrefabs[prefab].Remove(prefabInstance);  // 활성화된 프리팹 리스트에서 제거
        }
    }

    // Set, Minus, Plus
    // 프리팹 최대 생성 개수 설정
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

    // 프리팹 최대 생성 개수 증가
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

    // 프리팹 최대 생성 개수 감소
    public void MinusMaxCount(GameObject prefab, int count)
    {
        foreach (var prefabInfo in prefabInfos)
        {
            if (prefabInfo.prefab == prefab)
            {
                prefabInfo.maxCount = Mathf.Max(0, prefabInfo.maxCount - count);  // 최소 0으로 제한
                break;
            }
        }
    }

    // 리스폰 주기 설정
    public void SetRespawnTime(float time)
    {
        respawnTime = time;
    }
    // 리스폰 주기 증가
    public void PlusRespawnTime(float time)
    {
        respawnTime += time;
    }
    // 리스폰 주기 감소
    public void MinusRespawnTime(float time)
    {
        respawnTime -= time;
    }
}



public class PrefabTracker : MonoBehaviour
{
    public ResourceManager resourceManager;  // ResourceManager에 대한 참조

    // 프리팹이 제거될 때 호출
    void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.RemovePrefab(this.gameObject, this.gameObject);  // ResourceManager에서 프리팹 제거
            resourceManager.ScheduleRespawn(this.gameObject, this.transform.position);  // 리스폰 예약
        }
    }
}



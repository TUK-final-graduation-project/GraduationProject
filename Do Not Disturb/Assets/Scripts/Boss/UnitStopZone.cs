using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStopZone : MonoBehaviour
{
    public GameObject damageRock;

    List<GameObject> rocks = new List<GameObject>();
    
    public void StartMakeRock()
    {
        MakeRock().Forget();
    }
    async UniTaskVoid MakeRock()
    {
        while (true)
        {
            await UniTask.Delay(6000);

            if (this == null)
            {
                return;
            }

            Vector3 pos = transform.position + new Vector3(Random.Range(-5f, 5f) * 10f, 0.5f, Random.Range(-5f, 5f) * 10f);

            GameObject rock = Instantiate(damageRock, pos, Quaternion.identity);
            rock.transform.position = new Vector3(rock.transform.position.x, 0, rock.transform.position.z);
            rocks.Add(rock);
        }
    }
    private void OnDestroy()
    {
        foreach(GameObject rock in rocks)
        {
            Destroy(rock);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<OurUnitController>() != null)
        {
            other.gameObject.GetComponent<OurUnitController>().StopToBossPoint();
        }
    }
}

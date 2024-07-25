using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStopZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<OurUnitController>() != null)
        {
            Debug.Log("dd");
            other.gameObject.GetComponent<OurUnitController>().StopToBossPoint();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : MonoBehaviour
{
    public int[] itemCount = new int[4];
    [SerializeField]
    InventorySlot inventorySlot_rock;
    
    [SerializeField]
    InventorySlot inventorySlot_steel;
    
    [SerializeField]
    InventorySlot inventorySlot_wood;
    
    [SerializeField]
    InventorySlot inventorySlot_ice;


    // Start is called before the first frame update
    void Start()
    {
        itemCount[0] = inventorySlot_rock.itemCount;
        itemCount[1] = inventorySlot_steel.itemCount;
        itemCount[2] = inventorySlot_wood.itemCount;
        itemCount[3] = inventorySlot_ice.itemCount;
    }

    // Update is called once per frame
    void Update()
    {
        itemCount[0] = inventorySlot_rock.itemCount;
        itemCount[1] = inventorySlot_steel.itemCount;
        itemCount[2] = inventorySlot_wood.itemCount;
        itemCount[3] = inventorySlot_ice.itemCount;
    }
}

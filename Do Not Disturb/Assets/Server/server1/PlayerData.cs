using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string clientID;
    public Vector3 position;
    public List<ItemData> items;
    public List<TurretData> turrets;
}

[Serializable]
public class ItemData
{
    public string itemType;
    public int itemCount;
}

[Serializable]
public class TurretData
{
    public string turretType;
    public Vector3 position;
}

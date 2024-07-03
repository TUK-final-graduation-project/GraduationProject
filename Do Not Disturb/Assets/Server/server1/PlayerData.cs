using System;
using System.Collections.Generic;
using UnityEngine;

enum state { LIVE, DEAD, ATTACK, HIT, MOVE, COUNT};

[Serializable]
public class PlayerData
{
    public string clientID;
    public Vector3 position;
    public enum state;
}

[Serializable]
public class MapData
{
    public List<ItemData> items;
    public List<TowerData> towers;
    public List<PlayerData> playerData;
    public List<MapData> mapData;
    public List<UnitData> unitData;
}

[Serializable]
public class ItemData
{
    public string itemType;
    public int itemCount;
}

[Serializable]
public class ResourceData
{
    public string resourceType;
    public int position;
    public enum state;
}

[Serializable]
public class TowerData
{
    public string towerType;
    public Vector3 position;
    public float hp;
    public enum state;
}

[Serializable]
public class UnitData
{
    public string unitType;
    public Vector3 position;
    public float hp;
    public enum state;
}

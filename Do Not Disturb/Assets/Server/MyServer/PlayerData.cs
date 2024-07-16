using System;
using System.Collections.Generic;
using UnityEngine;

public enum State { IDLE, DEAD, ATTACK, HIT, WALK, RUN, COUNT };

[Serializable]
public class PlayerData
{
    public string clientID;
    public Vector3 position;
    public Vector3 direction;
    public State state;
}

[Serializable]
public class MapData
{
    public List<ItemData> items;
    public List<TowerData> towers;
    public List<PlayerData> playerData;
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
    public Vector3 position;
    public State state;
}

[Serializable]
public class TowerData
{
    public string towerType;
    public Vector3 position;
    public float hp;
    public State state;
}

[Serializable]
public class UnitData
{
    public string unitType;
    public Vector3 position;
    public float hp;
    public State state;
}

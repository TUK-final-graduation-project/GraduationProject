using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 전용 생성메뉴 만들기
[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]

public class Item : ScriptableObject
{
    public string itemName; // 아이템의 이름.
    public int itemCount;   // 아이템 개수.
    public ItemType itemType; // 아이템의 유형.
    public Sprite itemImage; // 아이템의 이미지.
    public GameObject itemPrefab; // 아이템의 프리팹.


    public enum ItemType
    {
        JEM,
        CANDY,
        WOOD,
        STONE,
        ETC
    }

}

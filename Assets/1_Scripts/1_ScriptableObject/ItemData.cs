using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;             // 아이템 이름
    public string itemDescription;      // 아이템 설명

    public DateTime itemExpireDate;
    public DateTime itemCreateDate;

    public Sprite itemImage;
}

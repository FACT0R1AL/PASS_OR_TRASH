using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;

    public DateTime itemExpireDate;
    public DateTime itemCreateDate;
}

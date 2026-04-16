using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public string itemDescription;

    public DateTime itemExpireDate;
    public DateTime itemCreateDate;
}

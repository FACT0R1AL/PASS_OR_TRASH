using System;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public string itemDescription;

    public DateTime itemExpireDate;
    public DateTime itemCreateDate;

    public bool isTrashProduct;

    public Sprite[] itemSprites;
    // 0 Front
    // 1 Back
    // 2 Left
    // 3 Right
    // 4 Up
    // 5 Down
}

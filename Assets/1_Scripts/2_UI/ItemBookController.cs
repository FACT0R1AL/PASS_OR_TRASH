using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ItemBookController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject bookOverlay;
    [SerializeField] private TextMeshProUGUI nameLabel; // 아이템 이름 표시
    [SerializeField] private TextMeshProUGUI descLabel; // 아이템 설명 표시
    [SerializeField] private Image itemDisplayImage;    // 아이템 표시 이미지

    [Header("DB")]
    [SerializeField] private List<ItemData> itemDatabase = new List<ItemData>();

    private int currentIndex = 0;

    private void Awake()
    {
        if(bookOverlay != null) bookOverlay.SetActive(false);
    }

    // 도감 열기 
    public void OpenBoook()
    {
        bookOverlay.SetActive(true);
        RefreshUI();
    }

    // 도감 닫기
    public void CloseBook()
    {
        bookOverlay.SetActive(false);
    }

    // 좌하단 화살표(다음 아이템)
    public void ShowNextItem()
    {
        if(itemDatabase.Count == 0) return;

        // 순환 (1 -> 10)
        currentIndex = (currentIndex + 1) % itemDatabase.Count;
        RefreshUI();

    }

    // 우하단 화살표(이전 아이템)
    public void ShowPreviousItem()
    {
        if(itemDatabase.Count == 0) return;

        // 순환(10 -> 1)
        currentIndex = (currentIndex - 1 + itemDatabase.Count) % itemDatabase.Count;
        RefreshUI();
    }

    // UI Update
    private void RefreshUI()
    {
        if(itemDatabase.Count <= currentIndex) return;

        ItemData currentData = itemDatabase[currentIndex];

        // 데이터 적용
        if(nameLabel != null) nameLabel.text = currentData.itemName;
        if(descLabel != null) descLabel.text = currentData.itemDescription;
        if(itemDisplayImage != null) itemDisplayImage.sprite = currentData.itemImage;
    }
}

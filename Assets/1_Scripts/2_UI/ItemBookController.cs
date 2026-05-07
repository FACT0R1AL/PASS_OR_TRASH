using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ItemBookController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject ItemBookButton; // 도감 버튼
    [SerializeField] private TextMeshProUGUI nameLabel; // 아이템 이름 표시
    [SerializeField] private TextMeshProUGUI descLabel; // 아이템 설명 표시
    [SerializeField] private Image ItemBookPopUpImage;
    [SerializeField] private Animator bookAnimator;     // 애니메이션 재생 시간

    [Header("Setting")]
    [SerializeField] private float animationDelay = 0.3f;

    [Header("DB")]
    [SerializeField] private List<ItemData> itemDatabase = new List<ItemData>();

    private int currentIndex = 0;

    public void Awake()
    {   
        // 시작할때 도감 비활성화
        if (bookAnimator != null)
        {
            bookAnimator.gameObject.SetActive(false);
        }
    }

    // 도감 ON / OFF 관리
    public void ToggleBook()
    {
        bool isBookOpen = ItemBookPopUpImage.gameObject.activeSelf;

        if (isBookOpen)
        {
            StartCoroutine(CloseBookRoutine());
        }
        else
        {
            OpenBook();
        }
    }

    // 도감 열기 
    public void OpenBook()
    {
        ItemBookPopUpImage.gameObject.SetActive(true);
        
        if (bookAnimator != null) bookAnimator.SetTrigger("Open");

        RefreshUI();
    }

    // 도감 닫기
    private IEnumerator CloseBookRoutine()
    {
        if(bookAnimator != null) bookAnimator.SetTrigger("Close");

        // 애니메이션 재생 시간
        yield return new WaitForSecondsRealtime(animationDelay);

        ItemBookPopUpImage.gameObject.SetActive(false);
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
        if(itemDatabase == null) return;

        if(itemDatabase.Count <= currentIndex) return;      // 현재 인덱스가 리스트 범위를 넘으면 함수 종료

        ItemData currentData = itemDatabase[currentIndex];  // 아이템 데이터 가져옴

        if (currentData == null) return;

        // 데이터 적용
        if(nameLabel != null) nameLabel.text = currentData.itemName;
        if(descLabel != null) descLabel.text = currentData.itemDescription;
        if(ItemBookPopUpImage != null) ItemBookPopUpImage.sprite = currentData.itemImage;
    }
}

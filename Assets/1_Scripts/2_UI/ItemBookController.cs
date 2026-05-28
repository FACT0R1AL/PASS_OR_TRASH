using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ItemBookController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject itemBookButton; // 도감 버튼
    [SerializeField] private TextMeshProUGUI nameLabel; // 아이템 이름 표시
    [SerializeField] private TextMeshProUGUI descLabel; // 아이템 설명 표시
    [SerializeField] private Image itemBookPopupImage;  // 아이템 이미지
    [SerializeField] private RectTransform bookPopupRect;   // 도감 배경이미지

    [Header("Setting")]
    [SerializeField] private float animationDelay = 0.25f;   // 애니메이션 재생

    [Header("DB")]
    [SerializeField] private List<ItemData> itemDatabase = new List<ItemData>();

    private int currentIndex = 0;
    private bool isAnimating = false;   // 애니메이션 실행 중복 방지용

    public void Awake()
    {   
        // 시작할때 도감 비활성화
        if (bookPopupRect != null)
        {
            bookPopupRect.gameObject.SetActive(false);
        }
    }

    // 도감 ON / OFF 관리
    public void ToggleBook()
    {
        if (isAnimating) return;

        bool isCurrentlyActive = bookPopupRect.gameObject.activeSelf;

        if (isCurrentlyActive)
        {
            StartCoroutine(CloseBookRoutine());
        }
        else
        {
            StartCoroutine(OpenBookRoutine());
        }
    }

    // 도감 열기 
    public void OpenBook()
    {
        if (isAnimating) return;
        StartCoroutine(OpenBookRoutine());
    }

    // 도감 열기
    private IEnumerator OpenBookRoutine()
    {
        isAnimating = true;

        bookPopupRect.localScale = Vector3.zero;
        bookPopupRect.gameObject.SetActive(true);
        RefreshUI();

        float elapsed = 0f;
        float duration = animationDelay > 0 ? animationDelay : 0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 살짝 커졌다가 작아지는 효과
            float scale = t < 0.7f ? Mathf.Lerp(0f, 1.1f, t / 0.7f) : Mathf.Lerp(1.1f, 1f, (t - 0.7f) / 0.3f);

            bookPopupRect.localScale = Vector3.one * scale;
            yield return null;
        }

        bookPopupRect.localScale = Vector3.one;
        isAnimating = false;
    }

    // 도감 닫기
    private IEnumerator CloseBookRoutine()
    {
        isAnimating = true;

        float elapsed = 0f;
        float duration = animationDelay > 0 ? animationDelay : 0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // 1에서 0으로 작아지는 효과
            bookPopupRect.localScale = Vector3.one * (1f - t);
            yield return null;
        }

        bookPopupRect.localScale = Vector3.zero;
        bookPopupRect.gameObject.SetActive(false);
        isAnimating = false;
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
        if(itemBookPopupImage != null) itemBookPopupImage.sprite = currentData.itemImage;
    }
}
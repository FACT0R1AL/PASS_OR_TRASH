using UnityEngine;
using TMPro;

public class Date : MonoBehaviour
{
    public static Date Instance { get; private set; }

    [Header("UI 연결")]
    public TextMeshProUGUI dateText;    // 날짜 표시 텍스트
    public TextMeshProUGUI dayText;     // Day 표시 텍스트

    [Header("날짜 설정")]
    public int startYear  = 2226;       // 시작 연도
    public int startMonth = 1;          // 시작 월
    public int startDay   = 1;          // 시작 일

    // 현재 날짜
    public int CurrentYear  { get; private set; }
    public int CurrentMonth { get; private set; }
    public int CurrentDay   { get; private set; }

    // 경과 일수 (1부터 시작)
    public int DayCount { get; private set; } = 1;

    // 현재 월의 마지막 일자
    public int LastDayOfCurrentMonth => daysInMonth[CurrentMonth - 1];

    // 월별 마지막 일자
    private static readonly int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CurrentYear  = startYear;
        CurrentMonth = startMonth;
        CurrentDay   = startDay;

        UpdateDateText();
    }

    // 하루 증가
    public void AdvanceDay()
    {
        DayCount++;
        CurrentDay++;

        // 해당 월의 마지막 일자를 넘으면 다음 달
        if (CurrentDay > LastDayOfCurrentMonth)
        {
            CurrentDay = 1;
            CurrentMonth++;

            // 12월 넘으면 다음 해
            if (CurrentMonth > 12)
            {
                CurrentMonth = 1;
                CurrentYear++;
            }
        }

        UpdateDateText();
    }

    // 날짜 UI 갱신
    // 표시 예시: 2226-01-01
    private void UpdateDateText()
    {
        if (dateText != null)
            dateText.text = $"{CurrentYear}-{CurrentMonth:00}-{CurrentDay:00}";

        if (dayText != null)
            dayText.text = $"Day {DayCount}";
    }
}
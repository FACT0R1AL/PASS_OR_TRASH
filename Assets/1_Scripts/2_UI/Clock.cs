using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clock : MonoBehaviour
{
    [Header("ClockText")]
    public TextMeshProUGUI clockText;

    [Header("시간 설정")]
    public float startHour = 9f;        // 시작시간(09:00)
    public float endHour = 18f;         // 종료시간 (18:00)

    [Header("시간 속도")]
    public float timeMultiplier = 1f;   // 현실 1초
    private float currentTimeInSeconds;
    private bool isRunning = true;


    void Start()
    {   // 시간 분 단위로 변환 
        currentTimeInSeconds = startHour * 60f;
    }

    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        currentTimeInSeconds += Time.deltaTime * timeMultiplier;

        // 시간 계산
        int hour = Mathf.FloorToInt(currentTimeInSeconds / 60f);
        int minute = Mathf.FloorToInt(currentTimeInSeconds % 60f);
        
        clockText.text = $"{hour:00}:{minute:00}";

        if (hour >= endHour)
        {
            isRunning = false;
        }
    }

}
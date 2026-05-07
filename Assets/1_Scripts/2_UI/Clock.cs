using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI clockText;   // Clock Text
    public GameObject endPanel;         // 종료시간때 나타날 패널(임시)
    private Image endPanelImage;

    [Header("패널 설정")]
    public float fadeInSpeed = 2f;      // 패널 나타날떄 걸리는 시간
    public float endPanelStayTime = 4f; // 패널 유지 시간
    public float fadeOutSpeed = 2f;     // 패널 사라질때 걸리는 시간


    [Header("시간 설정")]
    public float startHour = 9f;        // 시작시간(09:00)
    public float endHour = 18f;         // 종료시간 (18:00)
    public float panelWaitTime = 3f;    // 패널 떠있는 시간

    [Header("시간 속도")]
    public float timeMultiplier = 1f;   // 현실 1초
    private float currentTimeInSeconds;
    private bool isRunning = true;


    void Start()
    {   
        if (endPanel != null)
        {
            endPanelImage = endPanel.GetComponent<Image>();
            endPanel.SetActive(false);
        }

        ResetClock();
        
        // 시간 분 단위로 변환 
        currentTimeInSeconds = startHour * 60f;
    }


    void Update()
    {
        if (!isRunning) return;

        currentTimeInSeconds += Time.deltaTime * timeMultiplier;

        // 시간 계산
        int hour = Mathf.FloorToInt(currentTimeInSeconds / 60f);
        int minute = Mathf.FloorToInt(currentTimeInSeconds % 60f);
        
        clockText.text = $"{hour:00}:{minute:00}";

        // 종료 시간 체크
        if (hour >= endHour)
        {
            StartCoroutine(EndAndRestartRoutine());
        }
    }

    // 시간 초기화
    void ResetClock()
    {
        currentTimeInSeconds = startHour * 60f;
        isRunning = true;
    }

    // 종료 -> 재시작 과정
    IEnumerator EndAndRestartRoutine()
    {
        isRunning = false;

        
        if (endPanel != null && endPanelImage != null)
        {
            clockText.text = $"{(int)endHour}:00";

            Color color = endPanelImage.color;
            color.a = 0f;
            endPanelImage.color = color;
            endPanel.SetActive(true);

            // 패널 나타냄
            float timer = 0f;
            while (timer < fadeInSpeed)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, timer / fadeInSpeed);
                endPanelImage.color = color;
                yield return null;
            }
            color.a = 1f;
            endPanelImage.color = color;

            currentTimeInSeconds = startHour * 60f;
            clockText.text = $"{(int)startHour:00}:00"; 

            // 대기 시간
            yield return new WaitForSeconds(endPanelStayTime);

            // 패널 사라짐
            timer = 0f;
            while (timer < fadeOutSpeed)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, timer / fadeOutSpeed);
                endPanelImage.color = color;
                yield return null;
            }
        }
        // 시간 재시작
        ResetClock();
    }

}
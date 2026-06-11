using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [Header("엔딩 오브젝트")]
    public GameObject endingObject;
    public Image fadeImage;                     // 검은 화면용 이미지

    [Header("페이드 설정")]
    public float fadeDelay = 1f;                // 오브젝트 활성화 후 페이드 시작까지 대기 시간
    public float fadeDuration = 2f;             // 검은 화면으로 변하는 시간

    [Header("Text")]
    public TextMeshProUGUI endingMessage;       // 엔딩 메시지

    private bool isFading = false;


    void Awake()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }

    void OnEnable()
    {
        Productcan.OnEndProduct += TriggerEnding;
        Trashcan.OnEndTrash    += TriggerEnding;
    }

    void OnDisable()
    {
        Productcan.OnEndProduct -= TriggerEnding;
        Trashcan.OnEndTrash    -= TriggerEnding;
    }

    // 엔딩 시작
    private void TriggerEnding()
    {
        if (isFading) return;

        // 엔딩 오브젝트 활성화
        if (endingObject != null) endingObject.SetActive(true);

        StartCoroutine(FadeToBlack());
    }

    // 검은 화면으로 변함
    private IEnumerator FadeToBlack()
    {
        isFading = true;

        yield return new WaitForSeconds(fadeDelay);

        if (fadeImage == null) yield break;

        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }
}
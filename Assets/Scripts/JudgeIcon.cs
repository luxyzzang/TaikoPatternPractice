using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JudgeIcon : MonoBehaviour
{
    public static JudgeIcon Instance;

    [Header("GameObjects")]
    public Text perfect;
    public Text good;
    public Text miss;

    [Header("Fade")]
    public float fadeTime = 0.3f;

    [Header("Move")]
    public RectTransform startPos; // Transform 대신 RectTransform으로 변경
    public float moveHeight = 30f;

    private Coroutine effectCoroutine;
    private Text activeText; // 현재 재생 중인 텍스트 추적용

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        perfect.gameObject.SetActive(false);
        good.gameObject.SetActive(false);
        miss.gameObject.SetActive(false);
    }

    public void PlayPerfectEffect() => PlayEffect(perfect);
    public void PlayGoodEffect() => PlayEffect(good);
    public void PlayMissEffect() => PlayEffect(miss);

    private void PlayEffect(Text text)
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            if (activeText != null)
            {
                ResetText(activeText);
            }
        }

        perfect.gameObject.SetActive(false);
        good.gameObject.SetActive(false);
        miss.gameObject.SetActive(false);

        activeText = text;
        effectCoroutine = StartCoroutine(EffectCoroutine(text));
    }

    private IEnumerator EffectCoroutine(Text text)
    {
        text.gameObject.SetActive(true);

        RectTransform rect = text.rectTransform;

        // 1. Z축 왜곡 방지를 위해 anchoredPosition (2D Vector2) 사용
        Vector2 startPosition = startPos.anchoredPosition;

        // Z축 수치가 튀지 않도록 Z값을 0으로 강제 고정
        rect.anchoredPosition3D = new Vector3(startPosition.x, startPosition.y, 0f);

        Color c = text.color;
        c.a = 1f;
        text.color = c;

        Vector2 peakPosition = startPosition + Vector2.up * moveHeight;
        float t = 0f;

        // 위로 이동
        float upDuration = fadeTime * 0.35f;
        while (t < upDuration)
        {
            t += Time.deltaTime;
            float p = t / upDuration;
            p = Mathf.Sin(p * Mathf.PI * 0.5f);

            // 2D Vector2 Lerp 사용으로 Z축 안전 보장
            rect.anchoredPosition = Vector2.Lerp(startPosition, peakPosition, p);

            yield return null;
        }

        t = 0f;
        // 내려오면서 페이드
        float downDuration = fadeTime * 0.65f;
        while (t < downDuration)
        {
            t += Time.deltaTime;
            float p = t / downDuration;

            rect.anchoredPosition = Vector2.Lerp(peakPosition, startPosition, p);

            c.a = 1f - p;
            text.color = c;
            yield return null;
        }

        ResetText(text);
        text.gameObject.SetActive(false);
        effectCoroutine = null;
        activeText = null;
    }

    private void ResetText(Text text)
    {
        if (text == null) return;
        text.rectTransform.anchoredPosition3D = new Vector3(startPos.anchoredPosition.x, startPos.anchoredPosition.y, 0f);

        Color c = text.color;
        c.a = 1f;
        text.color = c;
    }
}
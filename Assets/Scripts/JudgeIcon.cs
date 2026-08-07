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
    public Transform startPos;
    public float moveHeight = 30f;

    private Coroutine effectCoroutine;

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
            ResetText(text);
        }

        perfect.gameObject.SetActive(false);
        good.gameObject.SetActive(false);
        miss.gameObject.SetActive(false);

        effectCoroutine = StartCoroutine(EffectCoroutine(text));
    }


    private IEnumerator EffectCoroutine(Text text)
    {
        text.gameObject.SetActive(true);

        RectTransform rect = text.rectTransform;
        Vector3 startPosition = startPos.localPosition;
        rect.localPosition = startPosition;

        Color c = text.color;
        c.a = 1f;
        text.color = c;

        Vector3 peakPosition = startPosition + Vector3.up * moveHeight;
        float t = 0f;

        // 위로 이동
        while (t < fadeTime * 0.35f)
        {
            t += Time.deltaTime;
            float p = t / (fadeTime * 0.35f);
            p = Mathf.Sin(p * Mathf.PI * 0.5f);
            rect.localPosition = Vector3.Lerp(startPosition, peakPosition, p);

            yield return null;
        }

        t = 0f;
        // 내려오면서 페이드
        while (t < fadeTime * 0.65f)
        {
            t += Time.deltaTime;
            float p = t / (fadeTime * 0.65f);
            rect.localPosition = Vector3.Lerp(peakPosition, startPosition, p);

            c.a = 1f - p;
            text.color = c;
            yield return null;
        }


        ResetText(text);
        text.gameObject.SetActive(false);
        effectCoroutine = null;
    }

    private void ResetText(Text text)
    {
        text.rectTransform.localPosition = startPos.localPosition;

        Color c = text.color;
        c.a = 1f;
        text.color = c;
    }
}
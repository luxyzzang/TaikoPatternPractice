using System.Collections;
using UnityEngine;

public class JudgeEffect : MonoBehaviour
{
    public static JudgeEffect Instance;

    [Header("Sprites")]
    public Sprite perfectSprite;
    public Sprite goodSprite;
    public Sprite missSprite;

    [Header("Renderer")]
    public SpriteRenderer effectRenderer;

    [Header("Fade")]
    public float fadeTime = 0.1f;
    private Coroutine effectCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        effectRenderer.enabled = false;
    }

    public void PlayPerfectEffect() => PlayEffect(perfectSprite);
    public void PlayGoodEffect() => PlayEffect(goodSprite);
    public void PlayMissEffect() => PlayEffect(missSprite);

    private void PlayEffect(Sprite sprite)
    {
        if (effectCoroutine != null) { StopCoroutine(effectCoroutine); }
        effectCoroutine = StartCoroutine(EffectCoroutine(sprite));
    }

    private IEnumerator EffectCoroutine(Sprite sprite)
    {
        effectRenderer.enabled = true;
        effectRenderer.sprite = sprite;

        Color c = effectRenderer.color;
        c.a = 1f;
        effectRenderer.color = c;

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            c.a = 1f - t / fadeTime;
            effectRenderer.color = c;

            yield return null;
        }

        c.a = 0f;
        effectRenderer.color = c;
        effectRenderer.enabled = false;
        effectCoroutine = null;
    }
}
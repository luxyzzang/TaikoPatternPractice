using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public enum State { Hidden, FadeIn, Show, FadeOut }

[System.Serializable]
public class KeyImage
{
    public Image image;

    [HideInInspector] public float alpha;
    [HideInInspector] public float remainTime;
    [HideInInspector] public State state;
}

public class UIGame : MonoBehaviour
{
    public static UIGame Instance;
    private GameManager game;

    public GameObject pauseWindow;
    public Gradient2 bgGradient;
    public Image timeMask;
    public Image clearGauge;
    public Text accuracyTxt;
    public Text perfectCntTxt;
    public Text goodCntTxt;
    public Text missCntTxt;
    public Text comboTxt;

    public float fadeInTime = 0.05f;
    public float showTime = 0.1f;
    public float fadeOutTime = 0.05f;

    public KeyImage leftDon;
    public KeyImage leftKat;
    public KeyImage rightDon;
    public KeyImage rightKat;

    [Header("Mobile Taiko")]
    public Image mobileTaikoImg;
    public float taikoPressDistance = 10f;
    public float taikoPressTime = 0.05f;

    private Coroutine taikoCoroutine;
    private Vector2 taikoStartPos;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        game = GameManager.Instance;
        UpdateInfo();
        gameObject.SetActive(false);
        mobileTaikoImg.alphaHitTestMinimumThreshold = 0.1f;
        taikoStartPos = mobileTaikoImg.rectTransform.anchoredPosition;
    }

    private void Update()
    {
        UpdateKeyImage(leftDon);
        UpdateKeyImage(leftKat);
        UpdateKeyImage(rightDon);
        UpdateKeyImage(rightKat);
        timeMask.fillAmount = Mathf.Clamp01(Time.time / game.LastHitTime);
    }

    public void UpdateInfo()
    {
        int perfectCnt = game.perfectCnt;
        int goodCnt = game.goodCnt;
        int missCnt = game.missCnt;
        int judgedCnt = perfectCnt + goodCnt +missCnt;
        int comboCnt = game.comboCnt;
        float accurarcy;

        if (judgedCnt > 0) { accurarcy = (perfectCnt * 100 + goodCnt * 50) / (float)judgedCnt; }
        else { accurarcy = 0f; }

        accuracyTxt.text = accurarcy.ToString("F2") + "%";
        perfectCntTxt.text = perfectCnt.ToString();
        goodCntTxt.text = goodCnt.ToString();
        missCntTxt.text = missCnt.ToString();
        comboTxt.text = comboCnt.ToString();
        comboTxt.enabled = comboCnt > 0;
        clearGauge.fillAmount = game.totalNoteCnt > 0 ? (float)(perfectCnt + goodCnt) / game.totalNoteCnt : 0f;
    }

    public void StartGame()
    {
        UnityEngine.Gradient g = new UnityEngine.Gradient();
        Color startColor = UILobby.Instance.difficultyGradient.Evaluate(GameManager.Instance.level / 100f);

        g.SetKeys(new GradientColorKey[]
        { new GradientColorKey(Color.white, 0f), new GradientColorKey(startColor, 1f)},
        new GradientAlphaKey[] {new GradientAlphaKey(1f, 0f),new GradientAlphaKey(1f, 1f)});

        bgGradient.EffectGradient = g;
    }

    private void UpdateKeyImage(KeyImage img)
    {
        switch (img.state)
        {
            case State.FadeIn:
                img.alpha += Time.deltaTime / fadeInTime;
                if (img.alpha >= 1f)
                {
                    img.alpha = 1f;
                    img.state = State.Show;
                }
                break;

            case State.Show:
                img.remainTime -= Time.deltaTime;
                if (img.remainTime <= 0)
                    img.state = State.FadeOut;
                break;

            case State.FadeOut:
                img.alpha -= Time.deltaTime / fadeOutTime;
                if (img.alpha <= 0f)
                {
                    img.alpha = 0f;
                    img.state = State.Hidden;
                }
                break;
        }

        Color c = img.image.color;
        c.a = img.alpha;
        img.image.color = c;
    }

    public void KeyPressed(KeyImage img)
    {
        if (Application.isMobilePlatform) { MobileTaikoPressed(); }

        img.remainTime = showTime;
        if (img.state == State.Hidden || img.state == State.FadeOut) { img.state = State.FadeIn; }
    }

    public void OpenPauseWindow() => pauseWindow.SetActive(true);

    public void ResumeGame()
    {
        pauseWindow.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        pauseWindow.SetActive(false);
        game.ResetGame();
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        UILobby.Instance.gameObject.SetActive(true);
        pauseWindow.SetActive(false);
        game.ResetGame();
        game.isGame = false;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void MobileTaikoPressed()
    {
        if (taikoCoroutine != null)
        {
            StopCoroutine(taikoCoroutine);
            mobileTaikoImg.rectTransform.anchoredPosition = taikoStartPos;
        }

        taikoCoroutine = StartCoroutine(TaikoPressCoroutine());
    }

    private IEnumerator TaikoPressCoroutine()
    {
        RectTransform rect = mobileTaikoImg.rectTransform;
        Vector2 pressPos = taikoStartPos + Vector2.down * taikoPressDistance;
        float t = 0f;

        // ´­¸²
        while (t < taikoPressTime)
        {
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(taikoStartPos, pressPos, t / taikoPressTime);
            yield return null;
        }

        // º¹±Í
        t = 0f;
        while (t < taikoPressTime)
        {
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(pressPos, taikoStartPos, t / taikoPressTime);
            yield return null;
        }

        rect.anchoredPosition = taikoStartPos;
        taikoCoroutine = null;
    }
}
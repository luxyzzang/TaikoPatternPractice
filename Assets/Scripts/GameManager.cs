using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum HitType { Don, Kat }

public struct HitInput
{
    public HitType type;
    public bool isDouble;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private PoolManager pool;
    private SoundManager sound;
    private UILobby uiLobby;
    private UIGame uiGame;
    private UIResult uiResult;
    private JudgeIcon judgeIcon;
    private JudgeEffect judgeEffect;

    [Header("Resolution")]
    private float targetWidth = 1920f;
    private float targetHeight = 1080f;
    private Vector3 baseCameraPosition = new Vector3(4.5f, -1f, -10f);
    private float baseBarScaleY = 2.3f;
    private float baseJudgeCircleScale = 0.8f;
    private float baseNoteScale = 1f;
    private Camera cam;
    private int lastWidth;
    private int lastHeight;
    public float appliedBarScaleY;
    public float appliedJudgeCircleScale;
    public float appliedNoteScale;

    [Header("Options")]
    public int level;
    public float bpm = 120f;
    public int requestNoteCount = 100; // 요구된 노트량
    public float scrollSpeed = 1f;
    public float firstSpawnTime = 1f; // 첫 노트가 SpawnPoint에서 생성되는 시간
    public bool beatSoundActive = true;
    public bool onlyFullCombo = false;
    public bool onlyAllPerfect = false;
    public Transform spawnPos;
    public GameObject judgeCircle;
    public GameObject barPrefab;

    private readonly float baseMoveTime = 360f; // 기준값
    private int spawnCount = 0;

    [Header("Difficulty")]
    public float perfectJudge = 0.025f;
    public float goodJudge = 0.075f;
    public float missJudge = 0.108f;

    [Header("Score")]
    public int totalNoteCnt = 0; // 생성된 실제 노트 총개수
    public int comboCnt = 0;
    public int perfectCnt = 0;
    public int goodCnt = 0;
    public int missCnt = 0;
    public bool isGame;

    private float speed;
    private float startTime;
    private float firstHitTime;
    private float lastHitTime;
    private int nextNoteIndex = 0;
    private int nextBarIndex = 0;
    private int nextBeatIndex = 0;

    private List<NoteData> notes = new();
    private List<NoteData> bars = new();
    private List<float> beats = new();
    public Queue<Note> activeNotes = new();

    public float CurrentTime => Time.time - startTime;
    public float LastHitTime => lastHitTime;

    private void Awake()
    {
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        cam = Camera.main;
        pool = PoolManager.Instance;
        sound = SoundManager.Instance;
        uiLobby = UILobby.Instance;
        uiGame = UIGame.Instance;
        uiResult = UIResult.Instance;
        judgeIcon = JudgeIcon.Instance;
        judgeEffect = JudgeEffect.Instance;
        UpdateScaleAndPosition();
    }

    private void Update()
    {
        if (!isGame) return;
        // if (Input.GetKeyDown(KeyCode.F5)) { ResetGame(); }

        if (totalNoteCnt == perfectCnt + goodCnt + missCnt) 
        { 
            Invoke(nameof(PresentGameResult), 2.5f);
            isGame = false;
        }

        while (nextNoteIndex < notes.Count && CurrentTime >= notes[nextNoteIndex].spawnTime)
        {
            SpawnNote(notes[nextNoteIndex]);
            nextNoteIndex++;
        }

        while (nextBarIndex < bars.Count && CurrentTime >= bars[nextBarIndex].spawnTime)
        {
            SpawnBar(bars[nextBarIndex]);
            nextBarIndex++;
        }

        while(nextBeatIndex < beats.Count && CurrentTime >= beats[nextBeatIndex])
        {
            sound.PlayBeatSound();
            nextBeatIndex++;
        }

        PauseGame();
    }

    private void CreateRandomPatterns()
    {
        PatternGenerator pg = gameObject.AddComponent<PatternGenerator>();
        pg.Perform(level);

        startTime = Time.time;
        float moveTime = baseMoveTime / bpm / scrollSpeed;
        speed = spawnPos.position.x / moveTime;
        firstHitTime = firstSpawnTime + moveTime;

        float measureInterval = 60f / bpm * 4f;
        float noteInterval = measureInterval / (int)pg.patternData.beatsPerMeasure;
        float firstBarSpawnTime = firstHitTime - moveTime;
        int measureIndex = 0;
        int beatIndex = 0;

        foreach (string measure in pg.result)
        {
            float measureStartTime = firstHitTime + measureIndex * measureInterval;
            for (int i = 0; i < 4; i++) { beats.Add(measureStartTime + measureInterval * i / 4f); }
            measureIndex++;

            for (int i = 0; i < measure.Length; i++)
            {
                if (i == 0)
                {
                    NoteData note = new();
                    note.hitTime = firstHitTime + beatIndex * noteInterval;
                    note.spawnTime = note.hitTime - moveTime;
                    bars.Add(note);
                }

                if (measure[i] != '0')
                {
                    NoteData note = new();
                    note.type = (NoteType)(measure[i] - '0');
                    note.hitTime = firstHitTime + beatIndex * noteInterval;
                    note.spawnTime = note.hitTime - moveTime;
                    notes.Add(note);
                }

                beatIndex++;
            }
        }

        totalNoteCnt = pg.result.Sum(x => x.Count(c => c == '1' || c == '2'));
        lastHitTime = notes[notes.Count - 1].hitTime;
        Destroy(pg);
    }

    private void SpawnNote(NoteData data)
    {
        Note note = pool.GetFromPool(spawnPos, (int)data.type);
        note.Init(data.type, data.hitTime, speed, ++spawnCount);
        activeNotes.Enqueue(note);
    }

    private void SpawnBar(NoteData data)
    {
        Bar bar = Instantiate(barPrefab, spawnPos).GetComponent<Bar>();
        bar.Init(data.hitTime, speed);
        bar.transform.localScale = new Vector3(bar.transform.localScale.x, appliedBarScaleY, 0);
    }


    public void CheckCorrect(int keyPressBit)
    {
        if (activeNotes.Count == 0) return;

        Note note = activeNotes.Peek();
        HitType inputType = (keyPressBit & ((1 << 0) | (1 << 1))) != 0 ? HitType.Don : HitType.Kat;
        float diff = Mathf.Abs(CurrentTime - note.hitTime);

        // 타입 검사
        if (note.type == NoteType.Don && inputType != HitType.Don) return;
        if (note.type == NoteType.Kat && inputType != HitType.Kat) return;

        // 시간 판정
        int idx = -1;

        if (diff <= perfectJudge)
        { 
            idx = 0;
            perfectCnt++;
            comboCnt++;
            judgeIcon.PlayPerfectEffect();
            judgeEffect.PlayPerfectEffect();
        }
        else if (diff <= goodJudge)
        { 
            idx = 1; 
            goodCnt++;
            comboCnt++;
            judgeIcon.PlayGoodEffect();
            judgeEffect.PlayGoodEffect();
        }
        else if (diff <= missJudge) 
        { 
            idx = 2;
            missCnt++;
            comboCnt = 0;
            judgeIcon.PlayMissEffect();
            judgeEffect.PlayMissEffect();
        }

        if (idx != -1)
        {
            activeNotes.Dequeue();
            pool.ReturnToPool(note, (int)note.type);
            if (onlyAllPerfect && (goodCnt > 0 || missCnt > 0)) { ResetGame(); }
            else if (onlyFullCombo && missCnt > 0) { ResetGame(); }

            uiGame.UpdateInfo();
        }
    }

    public void StartGame(int level)
    {
        this.level = level;
        uiLobby.gameObject.SetActive(false);
        ResetGame();
        uiGame.gameObject.SetActive(true);
        uiGame.StartGame();
        isGame = true;
    }

    public void ResetGame()
    {
        isGame = false;
        CancelInvoke(nameof(PresentGameResult));

        // 활성 노트 반환
        pool.ReturnToPoolAll();
        foreach (Bar bar in FindObjectsByType<Bar>(FindObjectsSortMode.None)) { Destroy(bar.gameObject); }

        // 데이터 초기화
        notes.Clear();
        bars.Clear();
        beats.Clear();
        activeNotes.Clear();

        totalNoteCnt = 0;
        comboCnt = 0;
        perfectCnt = 0;
        goodCnt = 0;
        missCnt = 0;

        spawnCount = 0;
        nextNoteIndex = 0;
        nextBarIndex = 0;
        nextBeatIndex = 0;

        uiGame.UpdateInfo();
        CreateRandomPatterns();
        isGame = true;
    }

    private void PauseGame()
    {
        if (!isGame) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            uiGame.OpenPauseWindow();
        }
    }

    private void PresentGameResult() => uiResult.gameObject.SetActive(true);

    private void UpdateScaleAndPosition()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        float targetAspect = targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float aspectScale = currentAspect / targetAspect;

        Vector3 newPos = baseCameraPosition;
        newPos.x = baseCameraPosition.x * aspectScale;
        newPos.y = baseCameraPosition.y * aspectScale;

        cam.transform.position = newPos;
        appliedBarScaleY = baseBarScaleY * aspectScale;
        appliedJudgeCircleScale = baseJudgeCircleScale * aspectScale;
        appliedNoteScale = baseNoteScale * aspectScale;

        judgeCircle.transform.localScale = Vector3.one * appliedJudgeCircleScale;
    }
}
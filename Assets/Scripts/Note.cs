using UnityEngine;

public enum NoteType { None, Don, Kat, BigDon, BigKat }

class NoteData
{
    public NoteType type;
    public float spawnTime;
    public float hitTime;
}
public class Note : MonoBehaviour
{
    public NoteType type; // 노트 타입
    public float hitTime; // 정확히 쳐야 하는 타이밍
    private float speed;
    private bool isMissed = false;

    private GameManager game;
    private PoolManager pool;
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        game = GameManager.Instance;
        pool = PoolManager.Instance;
    }

    private void OnEnable()
    {
        isMissed = false;
    }

    public void Init(NoteType type, float hitTime, float speed, int cnt)
    {
        this.type = type;
        this.hitTime = hitTime;
        this.speed = speed;
        sprite.sortingOrder = -cnt;
    }

    private void Update()
    {
        float remain = hitTime - game.CurrentTime;
        float x = remain * speed;

        transform.position = new Vector3(x, 0, 0);

        if (game.CurrentTime > hitTime + game.missJudge && !isMissed) 
        {
            isMissed = true;
            game.missCnt++;
            game.comboCnt = 0;
            game.activeNotes.Dequeue();
            UIGame.Instance.UpdateInfo();

            if (game.onlyAllPerfect || game.onlyFullCombo) { game.ResetGame(); }
        }

        if (x < -10f) { pool.ReturnToPool(this, (int)type); }
    }
}
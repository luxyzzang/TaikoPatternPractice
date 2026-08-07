using UnityEngine;

public class Bar : MonoBehaviour
{
    public NoteType type; // 노트 타입
    public float hitTime; // 정확히 쳐야 하는 타이밍
    private float speed;

    private GameManager game;
    private PoolManager pool;

    private void Start()
    {
        game = GameManager.Instance;
        pool = PoolManager.Instance;
    }

    public void Init(float hitTime, float speed)
    {
        this.hitTime = hitTime;
        this.speed = speed;
    }

    private void Update()
    {
        float remain = hitTime - game.CurrentTime;
        float x = remain * speed;

        transform.position = new Vector3(x, 0, 0);
        if (x < -10f) { Destroy(gameObject); }
    }
}
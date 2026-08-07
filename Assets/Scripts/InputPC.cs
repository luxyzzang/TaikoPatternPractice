using UnityEngine;

public class InputPC : MonoBehaviour
{
    private GameManager game;
    private SoundManager sound;
    private UIGame uiGame;

    public KeyCode leftDon = KeyCode.F;
    public KeyCode rightDon = KeyCode.J;
    public KeyCode leftKat = KeyCode.D;
    public KeyCode rightKat = KeyCode.K;

    private void Start()
    {
        game = GameManager.Instance;
        sound = SoundManager.Instance;
        uiGame = UIGame.Instance;
    }

    private void Update()
    {
        if (!game.isGame) return;
        CheckKeyPress();
    }

    private void CheckKeyPress()
    {
        int keyPressBit = 0;

        if (Input.GetKeyDown(leftDon))
        {
            keyPressBit |= 1 << 0;
            uiGame.KeyPressed(uiGame.leftDon);
            sound.PlayDon();
        }
        if (Input.GetKeyDown(rightDon))
        {
            keyPressBit |= 1 << 1;
            uiGame.KeyPressed(uiGame.rightDon);
            sound.PlayDon();
        }
        if (Input.GetKeyDown(leftKat))
        {
            keyPressBit |= 1 << 2;
            uiGame.KeyPressed(uiGame.leftKat);
            sound.PlayKat();
        }
        if (Input.GetKeyDown(rightKat))
        {
            keyPressBit |= 1 << 3;
            uiGame.KeyPressed(uiGame.rightKat);
            sound.PlayKat();
        }

        if (keyPressBit == 0) return;
        game.CheckCorrect(keyPressBit);
    }
}
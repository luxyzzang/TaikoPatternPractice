using UnityEngine;
using UnityEngine.EventSystems;

public class InputMobile : MonoBehaviour
{
    private GameManager game;
    private SoundManager sound;
    private UIGame uiGame;

    private Vector2 lastClickPosition;

    private void Start()
    {
        game = GameManager.Instance;
        sound = SoundManager.Instance;
        uiGame = UIGame.Instance;
    }

    public void SaveClickPosition(BaseEventData data)
    {
        PointerEventData eventData = data as PointerEventData;
        if (eventData != null) { lastClickPosition = eventData.position; }
    }

    public void PressDon() => Invoke(nameof(DelayedPressDon), 0.0001f);

    public void PressKat() => Invoke(nameof(DelayedPressKat), 0.0001f);

    private void DelayedPressDon()
    {
        if (lastClickPosition.x < Screen.width * 0.5f)
        {
            uiGame.KeyPressed(uiGame.leftDon);
            sound.PlayDon();
            game.CheckCorrect(1 << 0);
        }
        else
        {
            uiGame.KeyPressed(uiGame.rightDon);
            sound.PlayDon();
            game.CheckCorrect(1 << 1);
        }
    }

    private void DelayedPressKat()
    {
        if (lastClickPosition.x < Screen.width * 0.5f)
        {
            uiGame.KeyPressed(uiGame.leftKat);
            sound.PlayKat();
            game.CheckCorrect(1 << 2);
        }
        else
        {
            uiGame.KeyPressed(uiGame.rightKat);
            sound.PlayKat();
            game.CheckCorrect(1 << 3);
        }
    }
}
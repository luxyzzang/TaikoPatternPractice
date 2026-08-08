using UnityEngine;
using UnityEngine.EventSystems;

public class InputMobile : MonoBehaviour
{
    private GameManager game;
    private SoundManager sound;
    private UIGame uiGame;

    private void Start()
    {
        game = GameManager.Instance;
        sound = SoundManager.Instance;
        uiGame = UIGame.Instance;
    }

    public void PressDon(BaseEventData data)
    {
        PointerEventData eventData = data as PointerEventData;
        if (eventData == null) return;

        if (eventData.position.x < Screen.width * 0.5f)
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

    public void PressKat(BaseEventData data)
    {
        PointerEventData eventData = data as PointerEventData;
        if (eventData == null) return;

        if (eventData.position.x < Screen.width * 0.5f)
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
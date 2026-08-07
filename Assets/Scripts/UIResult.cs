using UnityEngine;

public class UIResult : MonoBehaviour
{
    public static UIResult Instance;

    private GameManager game;
    private UILobby uiLobby;
    private UIGame uiGame;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        game = GameManager.Instance;
        uiLobby = UILobby.Instance;
        uiGame = UIGame.Instance;
        gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        game.ResetGame();
        gameObject.SetActive(false);
    }

    public void EndGame()
    {
        uiLobby.gameObject.SetActive(true);
        uiGame.gameObject.SetActive(false);
        game.isGame = false;
        gameObject.SetActive(false);
    }
}
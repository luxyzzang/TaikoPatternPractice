using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public InputPC pc;
    public InputMobile mobile;

    private void Awake()
    {
        Instance = this;

        pc.enabled = !Application.isMobilePlatform;
        mobile.enabled = Application.isMobilePlatform;
    }
}

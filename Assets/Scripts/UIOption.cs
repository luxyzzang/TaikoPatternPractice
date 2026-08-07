using System;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour
{
    public static UIOption Instance;
    private GameManager game;
    private InputManager input;
    private SoundManager sound;

    public Gradient color;
    public Slider bpmSlider;
    public Slider noteSlider;
    public Slider scrollSlider;
    public Slider judgementSlider;
    public Slider volumeSlider;
    public InputField bpmInputField;
    public InputField noteInputField;
    public InputField scrollInputField;
    public InputField judgementInputField;
    public InputField volumeInputField;

    public Toggle beatSoundToggle;
    public Toggle fullComboToggle;
    public Toggle allPerfectToggle;

    [Header("Key Setting")]
    public GameObject keySettingWindow;
    public Text leftDonText;
    public Text rightDonText;
    public Text leftKatText;
    public Text rightKatText;
    public Text keyPressedText;
    private int keySetNum;
    private bool isWaitingKey;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        game = GameManager.Instance;
        sound = SoundManager.Instance;
        input = InputManager.Instance;
        BpmInputFieldValueChanged();
        NoteCountInputFieldValueChanged();
        ScrollInputFieldValueChanged();
        JudgementInputFieldValueChanged();

        if (!Application.isMobilePlatform)
        {

            ApplyKeyText(leftDonText, input.pc.leftDon.ToString());
            ApplyKeyText(rightDonText, input.pc.rightDon.ToString());
            ApplyKeyText(leftKatText, input.pc.leftKat.ToString());
            ApplyKeyText(rightKatText,input.pc.rightKat.ToString());
        }
    }

    private void Update()
    {
        if (isWaitingKey)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    ApplyKey(key);
                    isWaitingKey = false;
                    break;
                }
            }
        }
    }

    public void BpmSliderValueChanged()
    {
        float t = Mathf.Clamp01((bpmSlider.value - 50f) / 270f); // 50 ~ 320 -> 0 ~ 1
        float n = Mathf.Pow(t, 1.75f) * 0.95f;
        Color c = color.Evaluate(n);

        bpmSlider.fillRect.GetComponent<Image>().color = c;
        bpmSlider.handleRect.Find("Handle").GetComponent<Image>().color = c;
        bpmInputField.textComponent.color = c;
        bpmInputField.text = bpmSlider.value.ToString();

        game.bpm = bpmSlider.value;
    }

    public void BpmInputFieldValueChanged()
    {
        if (string.IsNullOrEmpty(bpmInputField.text)) { bpmInputField.text = bpmSlider.value.ToString(); }
        bpmInputField.text = Mathf.Clamp(int.Parse(bpmInputField.text), 50, 400).ToString();
        bpmSlider.value = int.Parse(bpmInputField.text);

        BpmSliderValueChanged();
    }

    public void NoteCountSliderValueChanged()
    {
        float t = Mathf.Clamp01((noteSlider.value - 50f) / 950f); // 50 ~ 1000 -> 0 ~ 1
        float n = Mathf.Pow(t, 1.5f) * 0.95f;
        Color c = color.Evaluate(n);

        noteSlider.fillRect.GetComponent<Image>().color = c;
        noteSlider.handleRect.Find("Handle").GetComponent<Image>().color = c;
        noteInputField.textComponent.color = c;
        noteInputField.text = noteSlider.value.ToString();

        game.requestNoteCount = Mathf.RoundToInt(noteSlider.value);
    }

    public void NoteCountInputFieldValueChanged()
    {
        if (string.IsNullOrEmpty(noteInputField.text)) { noteInputField.text = noteSlider.value.ToString(); }
        noteInputField.text = Mathf.Clamp(int.Parse(noteInputField.text), 50, 1000).ToString();
        noteSlider.value = int.Parse(noteInputField.text);

        NoteCountSliderValueChanged();
    }

    public void ScrollSliderValueChanged()
    {
        scrollSlider.SetValueWithoutNotify(Mathf.Round(scrollSlider.value * 10f) / 10f);
        float t = Mathf.Clamp01((scrollSlider.value - 0.5f) / 2f); // 0.5 ~ 2.5 -> 0 ~ 1
        float n = Mathf.Pow(t, 1.5f) * 0.95f;
        Color c = color.Evaluate(n);

        scrollSlider.fillRect.GetComponent<Image>().color = c;
        scrollSlider.handleRect.Find("Handle").GetComponent<Image>().color = c;
        scrollInputField.textComponent.color = c;
        scrollInputField.text = scrollSlider.value.ToString("F1");

        game.scrollSpeed = scrollSlider.value;
    }

    public void ScrollInputFieldValueChanged()
    {
        if (string.IsNullOrEmpty(scrollInputField.text)) { scrollInputField.text = scrollSlider.value.ToString("F1"); }

        if (float.TryParse(scrollInputField.text, out float value))
        {
            value = Mathf.Clamp(value, 0.5f, 4f);
            value = Mathf.Round(value * 10f) / 10f; // 0.1 단위

            scrollInputField.text = value.ToString("F1");
            scrollSlider.value = value;
        }
        else
        {
            scrollInputField.text = scrollSlider.value.ToString("F1");
        }

        ScrollSliderValueChanged();
    }

    public void JudgementSliderValueChanged()
    {
        judgementSlider.SetValueWithoutNotify(Mathf.Round(judgementSlider.value * 10f) / 10f);

        float t = Mathf.Clamp01((judgementSlider.value - 10f) / 40f); // 10 ~ 50 -> 0 ~ 1
        float n = Mathf.Pow(t, 2.5f) * 0.95f;
        Color c = color.Evaluate(n);

        judgementSlider.fillRect.GetComponent<Image>().color = c;
        judgementSlider.handleRect.Find("Handle").GetComponent<Image>().color = c;
        judgementInputField.textComponent.color = c;
        judgementInputField.text = (60f - judgementSlider.value).ToString("F1");

        game.perfectJudge = (60f - judgementSlider.value) / 1000f;
    }

    public void JudgementInputFieldValueChanged()
    {
        if (string.IsNullOrEmpty(judgementInputField.text)) { judgementInputField.text = judgementSlider.value.ToString("F1"); }

        if (float.TryParse(judgementInputField.text, out float value))
        {
            value = Mathf.Clamp(value, 10f, 50f);
            value = Mathf.Round(value * 10f) / 10f; // 0.1 단위

            judgementInputField.text = value.ToString("F1");
            judgementSlider.value = value;
        }
        else
        {
            judgementInputField.text = (60 - judgementSlider.value).ToString("F1");
        }

        JudgementSliderValueChanged();
    }

    public void VolumeSliderValueChanged()
    {
        volumeInputField.text = volumeSlider.value.ToString();
        sound.audioSource.volume = volumeSlider.value / 100f;
    }

    public void VolumeInputFieldValueChanged()
    {
        if (string.IsNullOrEmpty(volumeInputField.text)) { volumeInputField.text = volumeSlider.value.ToString(); }
        volumeInputField.text = Mathf.Clamp(int.Parse(volumeInputField.text), 0, 100).ToString();
        volumeSlider.value = int.Parse(volumeInputField.text);

        VolumeSliderValueChanged();
    }

    public void BeatSoundToggleChanged() => game.beatSoundActive = beatSoundToggle.isOn;

    public void FullComboToggleChanged() => game.onlyFullCombo = fullComboToggle.isOn;

    public void AllPerfectToggleChanged() => game.onlyAllPerfect = allPerfectToggle.isOn;

    public void OpenKeySettingWindow(int num)
    {
        keySetNum = num;
        keyPressedText.text = "";
        isWaitingKey = true;
        keySettingWindow.SetActive(true);
    }

    public void ApplyKey(KeyCode key)
    {
        string keyStr = key.ToString();
        Text text;

        switch (keySetNum)
        {
            case 0:
                {
                    input.pc.leftDon = key;
                    text = leftDonText;
                    break;
                }
            case 1:
                {
                    input.pc.rightDon = key;
                    text = rightDonText;
                    break;
                }
            case 2:
                {
                    input.pc.leftKat = key;
                    text = leftKatText;
                    break;
                }
            case 3:
                {
                    input.pc.rightKat = key;
                    text = rightKatText;
                    break;
                }
            default: text = null; break;
        }

        ApplyKeyText(text, keyStr);
        keyPressedText.text = keyStr;
        keyPressedText.fontSize = 250 - (keyStr.Length - 1) * 15;
        Invoke(nameof(CloseKeySettingWindow), 1f);
    }

    private void ApplyKeyText(Text text, string keyStr)
    {
        text.text = keyStr;
        text.fontSize = 50 - (keyStr.Length - 1) * 2;
    }

    private void CloseKeySettingWindow() => keySettingWindow.SetActive(false);

    public void OpenOptionWindow() => gameObject.SetActive(true);

    public void CloseOptionWindow() => gameObject.SetActive(false);
}
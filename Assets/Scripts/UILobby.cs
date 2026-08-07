using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    public static UILobby Instance;

    public Gradient difficultyGradient;
    public TextAsset levelText;
    [SerializeField][TextArea] private List<string> levelDescriptions = new();
    public Transform testContents;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLevelDescriptions();

        int i = 0;
        foreach (Transform t in testContents)
        {
            t.GetComponent<Image>().color = difficultyGradient.Evaluate(i / (float)testContents.childCount);
            t.Find("Description Text").GetComponent<Text>().text = levelDescriptions[i++]; 
            t.Find("Level Text").GetComponent<Text>().text = "Level " + i;
        }
    }

    private void LoadLevelDescriptions()
    {
        var matches = Regex.Matches(levelText.text, @"(\d+)\$\s*(.*?)(?=\n\d+\$|\z)", RegexOptions.Singleline);
        foreach (Match match in matches) { levelDescriptions.Add(match.Groups[2].Value.Trim()); }
    }
}

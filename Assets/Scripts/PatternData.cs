using UnityEngine;
using RNGNeeds;

public enum MeasureResolution
{
    _2 = 2,
    _4 = 4,
    _8 = 8,
    _12 = 12,
    _16 = 16,
    _24 = 24,
    _32 = 32,
    _48 = 48
}

[System.Serializable]
public struct RankRequirement
{
    [Range(50, 300)] public int bpm;
    [Range(80, 100)] public float comboCount;
    [Range(50, 100)] public float accuracy;
}

[CreateAssetMenu(fileName = "PatternData", menuName = "Taiko/Pattern Data")]
public class PatternData : ScriptableObject
{
    public ProbabilityList<string> patternList;
    public MeasureResolution beatsPerMeasure;
    public bool isDetarame;

    [Header("요구 수준")]
    public RankRequirement bronze;
    public RankRequirement silver;
    public RankRequirement gold;

    [Header("기차 패턴을 사용합니다.")] 
    public bool usedStreamPattern;
    public int lowestCount;
    public int highestCount;

    [Header(@"기차 패턴에 사용할 패턴 요소를 넣습니다.
    완성된 패턴마다 50% 확률로 아베코베가 적용됩니다.")]
    public ProbabilityList<string> streamUnits;
}
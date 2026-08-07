using System.Collections.Generic;
using UnityEngine;

public class PatternGenerator : MonoBehaviour
{
    public PatternData patternData;
    public List<string> result = new();

    public void Perform(int level)
    {
        patternData = Resources.Load<PatternData>("PatternDatas/" + level);
        PatternData data = patternData;
        result.Clear();

        int measure = (int)data.beatsPerMeasure;
        string pattern = new string('0', measure * 2);
        int comboCount = 0;

        while (comboCount < GameManager.Instance.requestNoteCount)
        {
            if (data.usedStreamPattern)
            {
                int streamCount = Random.Range(data.lowestCount, data.highestCount + 1);
                string streamPattern = "";

                int currentCount = 0;

                while (currentCount < streamCount)
                {
                    string unit = data.streamUnits.PickValue();
                    streamPattern += unit;

                    foreach (char c in unit)
                    {
                        if (c == '1' || c == '2') { currentCount++; }
                    }
                }

                // 정확히 streamCount개의 1,2까지만 남김
                int noteCount = 0;
                int cutIndex = streamPattern.Length;

                for (int i = 0; i < streamPattern.Length; i++)
                {
                    if (streamPattern[i] == '1' || streamPattern[i] == '2')
                    {
                        noteCount++;

                        if (noteCount == streamCount)
                        {
                            cutIndex = i + 1;
                            break;
                        }
                    }
                }

                streamPattern = streamPattern.Substring(0, cutIndex);

                // 50% 확률로 아베코베
                if (Random.value < 0.5f)
                {
                    char[] chars = streamPattern.ToCharArray();

                    for (int i = 0; i < chars.Length; i++)
                    {
                        if (chars[i] == '1') { chars[i] = '2'; }
                        else if (chars[i] == '2') { chars[i] = '1'; }
                    }

                    streamPattern = new string(chars);
                }

                int zeroCount = (measure - ((pattern.Length + streamPattern.Length) % measure)) % measure;
                if (zeroCount < measure / 12) { zeroCount += measure / 6; } // 휴식구간 씹힘 방지용

                pattern += streamPattern + new string('0', zeroCount);
                comboCount += streamCount;
            }
            else
            {
                string pickedPattern = data.patternList.PickValue();
                pattern += pickedPattern + "0";

                foreach (char c in pickedPattern)
                {
                    if (c != '0') { comboCount++; }
                }
            }
        }

        if (data.isDetarame)
        {
            char[] chars = pattern.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '1' || chars[i] == '2')
                {
                    chars[i] = Random.value < 0.5f ? '1' : '2';
                }
            }

            pattern = new string(chars);
        }

        // measure의 배수가 되도록 0 추가
        int remain = pattern.Length % measure;
        if (remain != 0) { pattern += new string('0', measure - remain); }

        for (int i = 0; i < pattern.Length; i += measure) { result.Add(pattern.Substring(i, measure)); }
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveUnifiedButtonManagerToJson : MonoBehaviour
{
    [Header("UnifiedButtonManager Reference")]
    public UnifiedButtonManager buttonManager; // UnifiedButtonManager 참조

    [Header("File Settings")]
    public string fileName = "UnifiedButtonManagerData.json"; // 저장할 JSON 파일 이름

    void Start()
    {
        if (buttonManager == null)
        {
            Debug.LogError("UnifiedButtonManager is not assigned!");
            return;
        }

        SaveToJson(); // Unity 실행 시 JSON 저장
    }

    public void SaveToJson()
    {
        if (buttonManager.listValueData == null || buttonManager.listValueData.Count == 0)
        {
            Debug.LogError("listValueData in UnifiedButtonManager is empty or not assigned.");
            return;
        }

        // JSON 데이터를 저장할 리스트 생성
        List<Entry> entries = new List<Entry>();

        foreach (var valueData in buttonManager.listValueData)
        {
            for (int i = 0; i < valueData.valueNames.Count; i++)
            {
                if (i < valueData.sceneNames.Count) // 값 개수가 맞는지 체크
                {
                    entries.Add(new Entry(valueData.valueNames[i], valueData.sceneNames[i]));
                }
            }
        }

        // Entry 리스트를 JSON으로 변환
        EntryList entryList = new EntryList { entries = entries };
        string json = JsonUtility.ToJson(entryList, true); // Pretty print JSON

        // 파일 저장 경로
        string filePath = Path.Combine(Application.dataPath, fileName);

        // JSON 파일 저장
        File.WriteAllText(filePath, json);
        Debug.Log($"JSON saved to: {filePath}");
    }
}

[System.Serializable]
public class Entry
{
    public string koreanName;
    public string englishName;

    public Entry(string korean, string english)
    {
        koreanName = korean;
        englishName = english;
    }
}

[System.Serializable]
public class EntryList
{
    public List<Entry> entries = new List<Entry>();
}

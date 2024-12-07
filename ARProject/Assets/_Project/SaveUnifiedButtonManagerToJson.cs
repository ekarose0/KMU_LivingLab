using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveUnifiedButtonManagerToJson : MonoBehaviour
{
    [Header("UnifiedButtonManager Reference")]
    public UnifiedButtonManager buttonManager; // UnifiedButtonManager ����

    [Header("File Settings")]
    public string fileName = "UnifiedButtonManagerData.json"; // ������ JSON ���� �̸�

    void Start()
    {
        if (buttonManager == null)
        {
            Debug.LogError("UnifiedButtonManager is not assigned!");
            return;
        }

        SaveToJson(); // Unity ���� �� JSON ����
    }

    public void SaveToJson()
    {
        if (buttonManager.listValueData == null || buttonManager.listValueData.Count == 0)
        {
            Debug.LogError("listValueData in UnifiedButtonManager is empty or not assigned.");
            return;
        }

        // JSON �����͸� ������ ����Ʈ ����
        List<Entry> entries = new List<Entry>();

        foreach (var valueData in buttonManager.listValueData)
        {
            for (int i = 0; i < valueData.valueNames.Count; i++)
            {
                if (i < valueData.sceneNames.Count) // �� ������ �´��� üũ
                {
                    entries.Add(new Entry(valueData.valueNames[i], valueData.sceneNames[i]));
                }
            }
        }

        // Entry ����Ʈ�� JSON���� ��ȯ
        EntryList entryList = new EntryList { entries = entries };
        string json = JsonUtility.ToJson(entryList, true); // Pretty print JSON

        // ���� ���� ���
        string filePath = Path.Combine(Application.dataPath, fileName);

        // JSON ���� ����
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

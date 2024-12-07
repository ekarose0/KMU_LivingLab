using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerWithButtons : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text sceneNameText; // TMP �ؽ�Ʈ�� ���� �� �̸� ǥ��
    public Button targetButton; // ��ư ����
    public TextAsset jsonFile; // JSON ���� ����

    [Header("Button Actions")]
    public string titleButtonName = "Title"; // Ÿ��Ʋ ��ư �̸�
    public string exitButtonName = "Exit"; // ���� ��ư �̸�
    public string titleSceneName = "MainMenu"; // Ÿ��Ʋ �� �̸�

    private Dictionary<string, string> sceneNameDictionary = new Dictionary<string, string>();

    void Start()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON file is not assigned!");
            return;
        }

        if (sceneNameText == null)
        {
            Debug.LogError("Scene Name Text is not assigned!");
            return;
        }

        if (targetButton == null)
        {
            Debug.LogError("Target Button is not assigned!");
            return;
        }

        // ��ư Ŭ�� �̺�Ʈ ����
        targetButton.onClick.AddListener(() => HandleButtonClick(targetButton.name));

        // JSON ������ �ε� �� �� ��Ī ǥ��
        LoadJsonData();
        DisplayCurrentSceneName();
    }

    private void HandleButtonClick(string buttonName)
    {
        if (buttonName == titleButtonName)
        {
            LoadScene(titleSceneName); // Ÿ��Ʋ ������ �̵�
        }
        else if (buttonName == exitButtonName)
        {
            ExitGame(); // ���� ����
        }
        else
        {
            Debug.LogWarning($"Unhandled button name: {buttonName}");
        }
    }

    private void LoadScene(string sceneName)
    {
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
        {
            Debug.LogError($"Scene '{sceneName}' is not in the Build Settings. Please add it.");
            return;
        }

        Debug.Log($"Loading Scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }

    private void LoadJsonData()
    {
        // JSON �����͸� SceneEntryList�� ��ȯ
        SceneEntryList entryList = JsonUtility.FromJson<SceneEntryList>(jsonFile.text);

        // Dictionary�� ��ȯ
        foreach (SceneEntry entry in entryList.entries)
        {
            if (!sceneNameDictionary.ContainsKey(entry.englishName))
            {
                sceneNameDictionary.Add(entry.englishName, entry.koreanName);
            }
        }

        Debug.Log("JSON data loaded successfully.");
    }

    private void DisplayCurrentSceneName()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (sceneNameDictionary.TryGetValue(currentSceneName, out string koreanName))
        {
            sceneNameText.text = $"{koreanName} ������...";
        }
        else
        {
            sceneNameText.text = "�� �� ���� ��� ������...";
        }
    }
}

[System.Serializable]
public class SceneEntry
{
    public string koreanName;
    public string englishName;
}

[System.Serializable]
public class SceneEntryList
{
    public List<SceneEntry> entries = new List<SceneEntry>();
}

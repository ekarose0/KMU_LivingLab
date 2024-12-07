using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerWithButtons : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text sceneNameText; // TMP 텍스트로 현재 씬 이름 표시
    public Button targetButton; // 버튼 참조
    public TextAsset jsonFile; // JSON 파일 참조

    [Header("Button Actions")]
    public string titleButtonName = "Title"; // 타이틀 버튼 이름
    public string exitButtonName = "Exit"; // 종료 버튼 이름
    public string titleSceneName = "MainMenu"; // 타이틀 씬 이름

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

        // 버튼 클릭 이벤트 설정
        targetButton.onClick.AddListener(() => HandleButtonClick(targetButton.name));

        // JSON 데이터 로드 및 씬 명칭 표시
        LoadJsonData();
        DisplayCurrentSceneName();
    }

    private void HandleButtonClick(string buttonName)
    {
        if (buttonName == titleButtonName)
        {
            LoadScene(titleSceneName); // 타이틀 씬으로 이동
        }
        else if (buttonName == exitButtonName)
        {
            ExitGame(); // 게임 종료
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
        // JSON 데이터를 SceneEntryList로 변환
        SceneEntryList entryList = JsonUtility.FromJson<SceneEntryList>(jsonFile.text);

        // Dictionary로 변환
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
            sceneNameText.text = $"{koreanName} 가는중...";
        }
        else
        {
            sceneNameText.text = "알 수 없는 경로 가는중...";
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

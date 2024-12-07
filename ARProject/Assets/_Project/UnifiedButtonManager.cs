using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnifiedButtonManager : MonoBehaviour
{
    [Header("Title Button Elements")]
    public Button titleButton;
    public Image titleImage;
    public TMP_Text titleText;
    public float titleBlinkAlphaMin = 0.8f;
    public float titleBlinkAlphaMax = 1.0f;
    public float titleBlinkInterval = 0.5f;

    [Header("List<T> Buttons")]
    public Button[] listButtons = new Button[5];
    public RectTransform[] listButtonTransforms = new RectTransform[5];

    [Header("Value Buttons")]
    public Button[] valueButtons = new Button[5];
    public RectTransform[] valueButtonTransforms = new RectTransform[5];
    public TMP_Text[] valueButtonTexts = new TMP_Text[5];

    [Header("Value Data")]
    public List<ValueData> listValueData = new List<ValueData>();

    [Header("Animation Settings")]
    public float listMoveDuration = 1.0f;
    public float valueMoveDuration = 1.0f;
    public Vector2 listInactivePosition = new Vector2(-258, 0);
    public Vector2 listActivePosition = new Vector2(-45, 0);
    public Vector2 valueActivePosition = new Vector2(45, 0);
    public Vector2 valueInactivePosition = new Vector2(300, 0);

    private Coroutine activeAnimationCoroutine;
    private int currentListIndex = -1;

    // Reference to GotoMainAR
    public GotoMainAR gotoMainAR;

    void Start()
    {
        // Initialize Title Blink Animation
        StartCoroutine(BlinkTitleButton());

        // Add listeners to Title and List buttons
        titleButton.onClick.AddListener(OnTitleButtonClick);
        for (int i = 0; i < listButtons.Length; i++)
        {
            int index = i; // Avoid closure issue
            listButtons[i].onClick.AddListener(() => OnListButtonClick(index));
        }

        // Add listeners to Value buttons
        for (int i = 0; i < valueButtons.Length; i++)
        {
            int index = i;
            valueButtons[i].onClick.AddListener(() => OnValueButtonClick(index));
        }
    }

    #region Title Button
    IEnumerator BlinkTitleButton()
    {
        while (true)
        {
            yield return FadeAlpha(titleImage, titleBlinkAlphaMax, titleBlinkAlphaMin, titleBlinkInterval / 2);
            yield return FadeAlpha(titleImage, titleBlinkAlphaMin, titleBlinkAlphaMax, titleBlinkInterval / 2);
        }
    }

    IEnumerator FadeAlpha(Image image, float from, float to, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            Color color = image.color;
            color.a = alpha;
            image.color = color;

            // Sync text alpha
            if (titleText != null)
            {
                Color textColor = titleText.color;
                textColor.a = alpha;
                titleText.color = textColor;
            }

            yield return null;
        }
    }

    void OnTitleButtonClick()
    {
        StopAllCoroutines(); // Stop blinking animation
        titleImage.gameObject.SetActive(false); // Hide Title
        titleButton.interactable = false;

        if (activeAnimationCoroutine != null)
            StopCoroutine(activeAnimationCoroutine);

        activeAnimationCoroutine = StartCoroutine(MoveListButtonsIntoView());
    }
    #endregion

    #region List<T> Buttons
    IEnumerator MoveListButtonsIntoView()
    {
        for (int i = 0; i < listButtonTransforms.Length; i++)
        {
            RectTransform rect = listButtonTransforms[i];
            Vector2 targetPosition = new Vector2(listActivePosition.x, rect.anchoredPosition.y);
            StartCoroutine(MoveToPosition(rect, targetPosition, listMoveDuration));
        }
        yield return null;
    }

    void OnListButtonClick(int index)
    {
        if (activeAnimationCoroutine != null)
            StopCoroutine(activeAnimationCoroutine);

        activeAnimationCoroutine = StartCoroutine(HandleListButtonClick(index));
    }

    IEnumerator HandleListButtonClick(int clickedIndex)
    {
        // Move all List<T> buttons except the clicked one to the inactive position
        for (int i = 0; i < listButtonTransforms.Length; i++)
        {
            RectTransform rect = listButtonTransforms[i];
            if (i == clickedIndex)
            {
                // Move clicked List<T> button to active position
                Vector2 targetPosition = new Vector2(listActivePosition.x, rect.anchoredPosition.y);
                StartCoroutine(MoveToPosition(rect, targetPosition, listMoveDuration));
            }
            else
            {
                // Move other List<T> buttons to inactive position
                Vector2 targetPosition = new Vector2(listInactivePosition.x, rect.anchoredPosition.y);
                StartCoroutine(MoveToPosition(rect, targetPosition, listMoveDuration));
            }
        }

        yield return new WaitForSeconds(listMoveDuration);

        // Update and show Value buttons
        if (currentListIndex != -1)
        {
            for (int i = 0; i < valueButtonTransforms.Length; i++)
            {
                StartCoroutine(MoveToPosition(valueButtonTransforms[i], new Vector2(valueInactivePosition.x, valueButtonTransforms[i].anchoredPosition.y), valueMoveDuration));
            }
            yield return new WaitForSeconds(valueMoveDuration);
        }

        UpdateValueTexts(clickedIndex);
        for (int i = 0; i < valueButtonTransforms.Length; i++)
        {
            StartCoroutine(MoveToPosition(valueButtonTransforms[i], new Vector2(valueActivePosition.x, valueButtonTransforms[i].anchoredPosition.y), valueMoveDuration));
        }

        currentListIndex = clickedIndex;
    }
    #endregion

    #region Value Buttons
    void UpdateValueTexts(int listIndex)
    {
        for (int i = 0; i < valueButtonTexts.Length; i++)
        {
            valueButtonTexts[i].text = listValueData[listIndex].valueNames[i];
        }
    }

    void OnValueButtonClick(int valueIndex)
    {
        if (currentListIndex == -1 || gotoMainAR == null)
            return;

        string sceneName = listValueData[currentListIndex].sceneNames[valueIndex];
        gotoMainAR.StartGoByName(sceneName); // Trigger GotoMainAR animation and scene load
    }
    #endregion

    #region Utility
    IEnumerator MoveToPosition(RectTransform rect, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = rect.anchoredPosition;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        rect.anchoredPosition = targetPosition;
    }
    #endregion
}

[System.Serializable]
public class ValueData
{
    public List<string> valueNames = new List<string>();
    public List<string> sceneNames = new List<string>();
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveCounterUI : MonoBehaviour
{
    public int waveNumber;
    private CanvasGroup waveNumberTransparency;
    private RectTransform waveNumberTransform;
    [SerializeField] private Text currentWaveNumberText;

    private void Start()
    {
        InitVariables();
    }

    private void InitVariables()
    {
        waveNumberTransparency = GetComponent<CanvasGroup>();
        waveNumberTransform = GetComponent<RectTransform>();
        CenterUI();
        waveNumber = 0;
    }

    private void CenterUI()
    {
        // Get the screen dimensions
        RectTransform canvasRectTransform = waveNumberTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        // Calculate the center position relative to the top right corner of the screen
        Vector2 centerPosition = new Vector2(canvasRectTransform.rect.width - waveNumberTransform.rect.width, canvasRectTransform.rect.height - (waveNumberTransform.rect.height * 2));

        // Set the anchored position of the UI element to the calculated center position
        waveNumberTransform.anchoredPosition = -centerPosition/2;
    }

    private void WaveStartAddToWaveNumber()
    {
        waveNumber++;
        currentWaveNumberText.text = waveNumber.ToString();
    }

    private void WaveStartMoveWaveNumberText()
    {
        StartCoroutine(MoveWaveNumberText());
    }

    IEnumerator MoveWaveNumberText()
    {
        yield return new WaitForSeconds(4);
        waveNumberTransform.pivot = new Vector2(1, 1);
        transform.LeanMove(new Vector2(Screen.width, Screen.height), 2);
    }

    private void WaveStartFadeInWaveNumberText()
    {
        waveNumberTransparency.alpha = 0;
        waveNumberTransparency.LeanAlpha(1, 2);
    }

    //executes the UI sequence for when a new wave starts
    public void WaveStartWaveNumberUISequence()
    {
        WaveStartAddToWaveNumber();
        WaveStartFadeInWaveNumberText();
        WaveStartMoveWaveNumberText();
        FadeTextToColor(currentWaveNumberText, new Color(200f/255f, 0, 0), 3); //dark red color
    }

    private void WaveEndBlinkWaveNumberText()
    {
        StartCoroutine(BlinkWaveNumberText());
    }

    IEnumerator BlinkWaveNumberText()
    {
        int blinkCount = 7;
        waveNumberTransparency.alpha = 1;
        yield return new WaitForSeconds(1);

        for(int i = 0; i < blinkCount; i++)
        {
            waveNumberTransparency.LeanAlpha(1, 0.5f);
            yield return new WaitForSeconds(0.5f);

            waveNumberTransparency.LeanAlpha(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    //executes the UI sequence for when a wave ends
    public void WaveEndWaveNumberUISequence()
    {
        FadeTextToColor(currentWaveNumberText, Color.white, 1);
        WaveEndBlinkWaveNumberText();
    }

    private void FadeTextToColor(Text text, Color color, float time)
    {
        StartCoroutine(FadeTransitionTextColor(text, color, time));
    }

    IEnumerator FadeTransitionTextColor(Text text, Color targetColor, float duration)
    {
        Color startColor = text.color;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            Color currentColor = Color.Lerp(startColor, targetColor, timeElapsed / duration);
            text.color = currentColor;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}

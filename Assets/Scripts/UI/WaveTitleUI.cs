using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveTitleUI : MonoBehaviour
{
    [SerializeField] private Text waveTitleText;
    private CanvasGroup waveTitleTransparency;

    private void Start()
    {
        InitVariables();
    }

    private void InitVariables()
    {
        waveTitleTransparency = GetComponent<CanvasGroup>();
    }

    private void WaveStartFadeWaveTitleText()
    {
        StartCoroutine(FadeWaveTitleText());
    }

    IEnumerator FadeWaveTitleText()
    {
        waveTitleTransparency.LeanAlpha(1, 1);
        yield return new WaitForSeconds(4);

        waveTitleTransparency.LeanAlpha(0, 1);
    }

    public void WaveStartWaveTitleUISequence()
    {
        WaveStartFadeWaveTitleText();
        FadeTextToColor(waveTitleText, new Color(200f / 255f, 0, 0), 2);
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

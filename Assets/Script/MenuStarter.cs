using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuStarter : MonoBehaviour
{
    [SerializeField] private GameObject starterCanvas;
    [SerializeField] private GameObject nextCanvas; // The canvas to show after
    [SerializeField] private RawImage starterText;
    [SerializeField] private float fadeSpeed = 1.0f;

    private bool isStarting = true;

    private void Start()
    {
        // Ensure initial state
        starterCanvas.SetActive(true);
        if (nextCanvas != null) nextCanvas.SetActive(false);

        // Start the pulsing alpha effect
        StartCoroutine(FadePulse());
    }

    private void Update()
    {
        // Detect "Press any key" to skip/start
        if (isStarting && Input.anyKeyDown)
        {
            StopAllCoroutines();
            SwitchCanvas();
        }
    }

    private IEnumerator FadePulse()
    {
        while (isStarting)
        {
            // Fade In
            yield return StartCoroutine(Fade(0, 1));
            // Fade Out
            yield return StartCoroutine(Fade(1, 0));
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color tempColor = starterText.color;

        while (elapsed < 1.0f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            tempColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed);
            starterText.color = tempColor;
            yield return null;
        }
    }

    private void SwitchCanvas()
    {
        isStarting = false;
        starterCanvas.SetActive(false);
        if (nextCanvas != null) nextCanvas.SetActive(true);

        Debug.Log("Game Started - Switched to Next Canvas");
    }
}
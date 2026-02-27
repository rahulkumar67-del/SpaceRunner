using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroNavigator : MonoBehaviour
{
    [Header("Images to Show")]
    public RawImage[] introImages; 

    private int currentIndex = 0;

    void Start()
    {
        // Initial setup: Show first image, hide others
        UpdateImageVisibility();
    }

    // Call this function from your UI Button "OnClick" event
    public void OnNextButtonPressed()
    {
        currentIndex++;

        if (currentIndex < introImages.Length)
        {
            // Move to the next image
            UpdateImageVisibility();
        }
        else
        {
            // We finished the second image, start the game
            PlayGame();
        }
    }

    void UpdateImageVisibility()
    {
        for (int i = 0; i < introImages.Length; i++)
        {
            // Only the current index is active
            introImages[i].gameObject.SetActive(i == currentIndex);
        }
    }

   
    void PlayGame()
    {
        Debug.Log("Transitioning to game...");
        SceneManager.LoadScene("MainScene");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{

    [SerializeField] private GameObject Menucanvas;
    [SerializeField] private GameObject InstructionCanvas;
    public void StartGame()
    {
        // Load the main game scene (replace "MainScene" with your actual scene name)
        SceneManager.LoadScene("MainScene");
    }

        public void QuitGame()
        {
            // Quit the application
            Application.Quit();
    }

    public void Insrtuction()
    {
        Menucanvas.SetActive(false);
        InstructionCanvas.SetActive(true);

    }

    public void Home()
    {
        Menucanvas.SetActive(true);
        InstructionCanvas.SetActive(false);
    }
}

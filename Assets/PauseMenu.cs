using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject optionMenu;
    private bool m_inOptionsMenu = false;

    PlayerInput P1_input;

    public void SetPlayerInput(PlayerInput p1)
    {
        P1_input = p1;
    }

    private void Update()
    {
        if (m_inOptionsMenu) return;

        bool p1Pressed = P1_input != null && P1_input.actions["Pause"].WasPressedThisFrame();
        if (p1Pressed)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    public void Resume()
    {
        print("Resumed");
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        GameIsPaused = false;

    }

    void Pause()
    {
        print("Paused");
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
    }

    public void LoadOptions()
    {
        print("Options");
        m_inOptionsMenu = true;

        pauseMenuUI.SetActive(false);
        optionMenu.SetActive(true);

    }

    public void Back()
    {
        print("Back");
        m_inOptionsMenu = false;

        pauseMenuUI.SetActive(true);
        optionMenu.SetActive(false);

    }
    public void Quit()
    {
        print("Quit");
        Time.timeScale = 1.0f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

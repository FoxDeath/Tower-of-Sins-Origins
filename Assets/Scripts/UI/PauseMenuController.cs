using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    #region Attributes
    private bool isPaused;

    [SerializeField] GameObject loadPauseMenu;
    [SerializeField] GameObject firstPauseMenuButton;

    private MainMenuController mainMenuController;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        mainMenuController = GetComponent<MainMenuController>();

        isPaused = false;
    }
    #endregion

    #region Normal Methods
    public void MenuInput()
    {
        if(isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        isPaused = false;

        PlayerInput.SetIsRecievingInput(true);

        UnloadPauseMenu();
    }

    void Pause()
    {
        isPaused = true;

        PlayerInput.SetIsRecievingInput(false);
        
        LoadPauseMenu();
    }

    public void LoadPauseMenu()
    {
        loadPauseMenu.SetActive(true);
        
        mainMenuController.SetFirstButton(firstPauseMenuButton);
    }

    public void UnloadPauseMenu()
    {
        loadPauseMenu.SetActive(false);        
    }
    #endregion
}

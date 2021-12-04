using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    #region Attributes
    [SerializeField] string sceneGame;
    [SerializeField] string sceneMainMenu;

    [SerializeField] public GameObject loadOptionsMenu;
    [SerializeField] GameObject loadSaveMenu;
    [SerializeField] GameObject loadQuitConfirmMenu;

    [SerializeField] GameObject firstOptionsButton;
    [SerializeField] GameObject firstQuitButton;

    private PauseMenuController pauseMenu;
    #endregion

    #region Normal Methods

    #region MonoBehaviour Methods
    void Start()
    {
        pauseMenu = GetComponent<PauseMenuController>();
    }
    #endregion
    public void NewGameScene()
    {
        SceneManager.LoadScene(sceneGame);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(sceneMainMenu);
    }

    public void LoadSaveMenu()
    {
        loadSaveMenu.SetActive(true);
    }

    public void LoadOptionsMenu()
    {
        loadOptionsMenu.SetActive(true);
        pauseMenu.UnloadPauseMenu();
        SetFirstButton(firstOptionsButton);
    }

    public void LoadQuitConfirmMenu()
    {
        loadQuitConfirmMenu.SetActive(true);
        pauseMenu.UnloadPauseMenu();
        SetFirstButton(firstQuitButton);
    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Does not work in editor");
    }

    public void SetFirstButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(button);
    }
    #endregion
}

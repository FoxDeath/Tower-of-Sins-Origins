using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    #region Attributes
    [SerializeField] GameObject loadVideoMenu;
    [SerializeField] GameObject loadAudioMenu;
    [SerializeField] GameObject loadControllsMenu;
    [SerializeField] GameObject loadAccesabilityMenu;
    [SerializeField] GameObject firstVideoButton;
    [SerializeField] GameObject firstAudioButton;
    [SerializeField] GameObject firstControlButton;
    [SerializeField] GameObject firstAccesabilityButton;

    private MainMenuController mainMenuController;
    #endregion

    #region MonoBehaviour Methods
    void Start()
    {
        mainMenuController = GetComponent<MainMenuController>();
    }
    #endregion

    #region Normal Methods
    public void LoadVideoMenu()
    {
        loadVideoMenu.SetActive(true);
        mainMenuController.loadOptionsMenu.SetActive(false);
        mainMenuController.SetFirstButton(firstVideoButton);
    }

    public void LoadAudioMenu()
    {
        loadAudioMenu.SetActive(true);
        mainMenuController.loadOptionsMenu.SetActive(false);
        mainMenuController.SetFirstButton(firstAudioButton);
    }

    public void LoadControllsMenu()
    {
        loadControllsMenu.SetActive(true);
        mainMenuController.loadOptionsMenu.SetActive(false);
        mainMenuController.SetFirstButton(firstControlButton);
    }

    public void LoadAccesabilityMenu()
    {
        loadAccesabilityMenu.SetActive(true);
        mainMenuController.loadOptionsMenu.SetActive(false);
        mainMenuController.SetFirstButton(firstAccesabilityButton);
    }
    #endregion
}

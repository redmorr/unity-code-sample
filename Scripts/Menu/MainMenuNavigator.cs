using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuNavigator : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private NewGameMenu newGameMenu;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private CreditsMenu creditsMenu;
    [SerializeField] private DisplaySettings displaySettings;
    [SerializeField] private DialogWindow dialogWindow;
    
    private MenuStack menuStack = new();
    
    private void OnEnable() => StartCoroutine(EnableCancelInput());
    private void OnDisable() => playerInput.UI.Cancel.performed -= Cancel;

    private void Awake()
    {
        Persistence.Initialize();
        mainMenu.Setup(OpenNewGameMenu, OpenSettingsMenu, OpenCreditsMenu, OpenQuitGameDialogWindow);
        newGameMenu.Setup(LoadDevRoom, LoadLevel1,LoadLevel2, menuStack.PreviousMenu);
        settingsMenu.Setup(OpenDisplaySettings, menuStack.PreviousMenu);
        displaySettings.Setup(menuStack.PreviousMenu);
        creditsMenu.Setup(menuStack.PreviousMenu);

        menuStack.OpenMenu(mainMenu.gameObject);
    }
    
    private void OpenDisplaySettings() => menuStack.OpenMenu(displaySettings.gameObject);
    private void OpenNewGameMenu() => menuStack.OpenMenu(newGameMenu.gameObject);
    private void OpenSettingsMenu() => menuStack.OpenMenu(settingsMenu.gameObject);
    private void OpenCreditsMenu() => menuStack.OpenMenu(creditsMenu.gameObject);

    private void OpenQuitGameDialogWindow()
    {
        dialogWindow.Setup("Do you really want quit the game?", Quit, menuStack.PreviousMenu);
        menuStack.OpenMenu(dialogWindow.gameObject);
    }
    
    private void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    
    private void LoadDevRoom()
    {
        SceneManager.LoadScene(Constants.SampleScene); // TODO: Remove or bring back devroom
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LoadLevel1()
    {
        SceneManager.LoadScene(Constants.Level1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void LoadLevel2()
    {
        SceneManager.LoadScene(Constants.Level2);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private IEnumerator EnableCancelInput()
    {
        yield return new WaitForEndOfFrame();
        playerInput.UI.Cancel.performed += Cancel;
    }

    private void Cancel(InputAction.CallbackContext ctx)
    {
        if (menuStack.Count == 1) OpenQuitGameDialogWindow();
        else menuStack.PreviousMenu();
    }
    
    private void OnDestroy()
    {
        playerInput.UI.Cancel.performed -= Cancel;
    }
}
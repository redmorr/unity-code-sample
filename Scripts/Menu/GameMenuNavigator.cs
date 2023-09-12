using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuNavigator : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private DisplaySettings displaySettings;
    [SerializeField] private DialogWindow dialogWindow;

    public static Action OnPauseMenuClosed;
    public static Action OnQuitToMainMenu;
    
    private MenuStack menuStack = new();

    private void Awake()
    {
        pauseMenu.Setup(ClosePauseMenu, OpenSettingsMenu, OpenQuitToMainMenuDialogWindow);
        settingsMenu.Setup(OpenDisplaySettings, menuStack.PreviousMenu);
        displaySettings.Setup(menuStack.PreviousMenu);
    }
    
    private void OnEnable() => StartCoroutine(EnableCancelInput());
    private void OnDisable() => playerInput.UI.Cancel.performed -= Cancel;
    
    private IEnumerator EnableCancelInput()
    {
        yield return new WaitForEndOfFrame();
        playerInput.UI.Cancel.performed += Cancel;
    }
    
    private void Cancel(InputAction.CallbackContext ctx)
    {
        switch (menuStack.Count)
        {
            case 0:
                OpenPauseMenu();
                break;
            case 1:
                ClosePauseMenu();
                break;
            default:
                menuStack.PreviousMenu();
                break;
        }
    }
    
    public void OpenPauseMenu()
    {
        menuStack.OpenMenu(pauseMenu.gameObject);
    }

    private void ClosePauseMenu()
    {
        menuStack.PreviousMenu();
        OnPauseMenuClosed?.Invoke();
    }

    private void OpenDisplaySettings()
    {
        menuStack.OpenMenu(displaySettings.gameObject);
    }

    private void OpenSettingsMenu()
    {
        menuStack.OpenMenu(settingsMenu.gameObject);
    }
    
    private void OpenQuitToMainMenuDialogWindow()
    {
        dialogWindow.Setup("Do you really want to quit to main menu?", () => OnQuitToMainMenu?.Invoke(), menuStack.PreviousMenu);
        menuStack.OpenMenu(dialogWindow.gameObject);
    }
}
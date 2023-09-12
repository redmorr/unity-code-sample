using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private enum State
    {
        MainMenu,
        Active,
        Paused,
        Dead
    }

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private State state;
    [SerializeField] private GameMenuNavigator gameMenuNav;
    
    private void Awake()
    {
        GameMenuNavigator.OnPauseMenuClosed = () => Pause(false);
        GameMenuNavigator.OnQuitToMainMenu = OnQuitToMainMenu;
        playerInput.Player.Pause.performed += HandlePause;
        playerInput.Player.PadStart.performed += HandlePause;
    }
    
    private void OnQuitToMainMenu()
    {
        Pause(false);
        SceneManager.LoadScene(Constants.MainMenu);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void HandlePause(InputAction.CallbackContext ctx)
    {
        if (state == State.Active) Pause(true);
    }

    private void Pause(bool active)
    {
        if (active)
        {
            state = State.Paused;
            gameMenuNav.gameObject.SetActive(true);
            gameMenuNav.OpenPauseMenu();
            Time.timeScale = 0f;
            playerInput.Player.Disable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            state = State.Active;
            gameMenuNav.gameObject.SetActive(false);
            Time.timeScale = 1f;
            playerInput.Player.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDestroy()
    {
        GameMenuNavigator.OnPauseMenuClosed = null;
        playerInput.Player.Pause.performed -= HandlePause;
    }
}
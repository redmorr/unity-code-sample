using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    
    public void Setup(UnityAction onNewGame, UnityAction onSettings, UnityAction onCredits, UnityAction onQuit)
    {
        newGameButton.onClick.AddListener(onNewGame);
        settingsButton.onClick.AddListener(onSettings);
        creditsButton.onClick.AddListener(onCredits);
        quitButton.onClick.AddListener(onQuit);
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }
}
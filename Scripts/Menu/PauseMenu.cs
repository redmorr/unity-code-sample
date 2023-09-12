using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    public void Setup(UnityAction onResume, UnityAction onSettings, UnityAction onQuit)
    {
        resumeButton.onClick.AddListener(onResume);
        settingsButton.onClick.AddListener(onSettings);
        quitButton.onClick.AddListener(onQuit);
    }
    
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }
}
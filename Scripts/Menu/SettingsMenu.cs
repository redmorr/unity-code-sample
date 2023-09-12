using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Button settingsDisplayButton;
    [SerializeField] private Button backButton;
    
    public void Setup(UnityAction onDisplaySettings, UnityAction onBack)
    {
        settingsDisplayButton.onClick.AddListener(onDisplaySettings);
        backButton.onClick.AddListener(onBack);
    }
    
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(settingsDisplayButton.gameObject);
    }
}
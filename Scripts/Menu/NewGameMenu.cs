using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField] private Button devRoomButton;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button backButton;
    
    public void Setup(UnityAction onDevRoom, UnityAction onLevel1, UnityAction onLevel2, UnityAction onBack)
    {
        devRoomButton.onClick.AddListener(onDevRoom);
        level1Button.onClick.AddListener(onLevel1);
        level2Button.onClick.AddListener(onLevel2);
        backButton.onClick.AddListener(onBack);
    }
    
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(devRoomButton.gameObject);
    }
}
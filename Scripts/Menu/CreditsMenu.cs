using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField] private Button backButton;
    
    public void Setup(UnityAction previousMenu)
    {
        backButton.onClick.AddListener(previousMenu);
    }
    
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);
    }
}
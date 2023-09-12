using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(noButton.gameObject);
    }

    public void Setup(string text, UnityAction onConfirm, UnityAction onDecline)
    {
        questionText.SetText(text);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(onConfirm);
        noButton.onClick.AddListener(onDecline);
    }
}
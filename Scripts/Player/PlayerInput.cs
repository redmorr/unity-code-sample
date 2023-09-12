using UnityEngine;

[CreateAssetMenu]
public class PlayerInput : ScriptableObject
{
    public PlayerControls.PlayerActions Player => controls.Player;
    public PlayerControls.UIActions UI => controls.UI;
    
    private PlayerControls controls;

    private void OnEnable()
    {
        controls = new PlayerControls();
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
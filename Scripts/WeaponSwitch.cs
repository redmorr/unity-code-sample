using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitch : MonoBehaviour
{
    private enum State
    {
        DOWN,
        SWITCHING,
        UP
    }


    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Transform weaponSocket;
    [SerializeField] private Transform weaponUp;
    [SerializeField] private Transform weaponDown;
    [SerializeField] private float switchTime;

    [Header("Debug")]
    [SerializeField] [ReadOnly] private Weapon currentWeapon;
    [SerializeField] [ReadOnly] private State state;
    [SerializeField] [ReadOnly] private float switchTimer;

    public bool Ready => state == State.UP;

    private void Awake()
    {
        state = State.DOWN;
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        Switch(1);
    }

    private void OnEnable()
    {
        playerInput.Player.Weapon1.performed += HandleSwitch1;
        playerInput.Player.Weapon2.performed += HandleSwitch2;
        playerInput.Player.Weapon3.performed += HandleSwitch3;
    }

    private void HandleSwitch1(InputAction.CallbackContext ctx) => Switch(1);
    private void HandleSwitch2(InputAction.CallbackContext ctx) => Switch(2);
    private void HandleSwitch3(InputAction.CallbackContext ctx) => Switch(3);

    private void Switch(int digit)
    {
        int weaponIndex = digit - 1;

        if (currentWeapon != null) currentWeapon.gameObject.SetActive(false);
        currentWeapon = weapons[weaponIndex];
        currentWeapon.gameObject.SetActive(true);

        weaponSocket.localPosition = weaponDown.localPosition;
        state = State.SWITCHING;
        switchTimer = 0f;
    }

    private void Update()
    {
        if (state == State.SWITCHING)
        {
            switchTimer = Mathf.MoveTowards(switchTimer, switchTime, Time.deltaTime);
            weaponSocket.localPosition = Vector3.Lerp(weaponDown.localPosition, weaponUp.localPosition, switchTimer / switchTime);
            if (switchTimer >= switchTime) state = State.UP;
        }
    }

    private void OnDisable()
    {
        playerInput.Player.Weapon1.performed -= HandleSwitch1;
        playerInput.Player.Weapon2.performed -= HandleSwitch2;
        playerInput.Player.Weapon3.performed -= HandleSwitch3;
    }
}
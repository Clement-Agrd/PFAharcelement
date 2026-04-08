using UnityEngine;

public class PlayerMobileInput : MonoBehaviour
{
    public VirtualJoystick moveJoystick;
    public VirtualJoystick aimJoystick;

    public Vector2 MoveInput => moveJoystick.Input;
    public Vector2 AimInput => aimJoystick.Input;
}
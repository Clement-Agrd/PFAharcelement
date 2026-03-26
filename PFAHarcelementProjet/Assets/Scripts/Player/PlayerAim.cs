using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private PlayerInputManager input;

    void Start()
    {
        input = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        Vector2 aim = input.Aim;

        if (aim.magnitude > 0.2f)
        {
            Vector3 direction = new Vector3(aim.x, 0, aim.y);

            direction = Quaternion.Euler(0, 45, 0) * direction;

            transform.forward = direction;
        }
    }
}
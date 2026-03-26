using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float dashForce = 10f;

    private PlayerInputManager input;

    void Start()
    {
        input = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }
    }

    public void Dash()
    {
        Vector3 dir = new Vector3(input.Move.x, 0, input.Move.y);

        if (dir == Vector3.zero)
            dir = transform.forward;

        transform.position += dir.normalized * dashForce;
    }
}
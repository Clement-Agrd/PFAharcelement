using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    private PlayerInputManager input;

    private void Start()
    {
        input = GetComponent<PlayerInputManager>();
    }

    private void Update()
    {
        Vector2 move = input.Move;

        Vector3 direction = new Vector3(move.x, 0, move.y);

        // Isométrique rotation
        direction = Quaternion.Euler(0, 45, 0) * direction;

        transform.position += direction * speed * Time.deltaTime;
    }
}
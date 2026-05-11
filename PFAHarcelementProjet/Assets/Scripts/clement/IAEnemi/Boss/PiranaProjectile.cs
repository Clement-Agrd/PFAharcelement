using UnityEngine;

public class PiranaProjectile : MonoBehaviour
{
    public float speed    = 5f;
    public float lifetime = 2.5f;
    Quaternion Rotation;

    private Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    void Start()
    {
        rb.linearVelocity = Vector3.up * speed;
        Destroy(gameObject, lifetime);
    }
}
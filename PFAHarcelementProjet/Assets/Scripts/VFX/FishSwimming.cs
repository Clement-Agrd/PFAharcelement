using UnityEngine;

public class FishSwimming : MonoBehaviour
{
    [Header("Mouvement de nage")]
    public float swimSpeed = 2f;          // Vitesse d'avance
    public float lateralAmplitude = 0.3f; // Amplitude oscillation gauche/droite
    public float lateralFrequency = 3f;   // Fréquence oscillation
    
    [Header("Mouvement vertical")]
    public float verticalAmplitude = 0.05f;
    public float verticalFrequency = 2f;

    [Header("Rotation")]
    public float yawAmount = 15f;         // Rotation gauche/droite (degrés)
    public float rollAmount = 5f;         // Roulis (degrés)

    [Header("Limites de nage")]
    public float swimDistance = 10f;      // Distance avant demi-tour
    
    private Vector3 startPosition;
    private float direction = 1f;
    private float t = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        t += Time.deltaTime;

        // --- Avance ---
        transform.Translate(Vector3.forward * swimSpeed * direction * Time.deltaTime);

        // --- Oscillation latérale ---
        float lateralOffset = Mathf.Sin(t * lateralFrequency) * lateralAmplitude;
        float verticalOffset = Mathf.Sin(t * verticalFrequency) * verticalAmplitude;

        transform.position = new Vector3(
            transform.position.x + lateralOffset * Time.deltaTime,
            startPosition.y + verticalOffset,
            transform.position.z
        );

        // --- Rotation naturelle ---
        float yaw   = Mathf.Sin(t * lateralFrequency) * yawAmount;
        float roll  = Mathf.Sin(t * verticalFrequency) * rollAmount;
        transform.rotation = Quaternion.Euler(roll, transform.eulerAngles.y, yaw);

        // --- Demi-tour automatique ---
        float distanceTravelled = Vector3.Distance(
            new Vector3(startPosition.x, 0, startPosition.z),
            new Vector3(transform.position.x, 0, transform.position.z)
        );

        if (distanceTravelled >= swimDistance)
        {
            direction *= -1f;
            startPosition = transform.position;
            transform.Rotate(0f, 180f, 0f); // Tourne le mesh
        }
    }
}
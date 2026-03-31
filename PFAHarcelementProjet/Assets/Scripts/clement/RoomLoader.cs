using UnityEngine;
using System.Collections;

public class RoomLoader : MonoBehaviour
{
    public static RoomLoader Instance;

    [Header("Rooms")]
    public GameObject[] combatRooms;
    public GameObject[] eliteRooms;
    public GameObject[] shopRooms;
    public GameObject eventRoom;
    public GameObject bossRoom;

    [Header("Setup")]
    public Transform spawnPoint;
    public Transform roomContainer;

    [Header("Animation Settings")]

    [SerializeField] private float fallDistance = 15f;

    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float riseSpeed = 2f;

    [SerializeField] private float waveDelayMultiplier = 0.1f;
    [SerializeField] private float maxDelay = 0.5f;

    [SerializeField] private float rotationIntensity = 200f;

    [SerializeField] private float totalAnimDuration = 2f;

    private GameObject currentRoom;

    void Awake()
    {
        Instance = this;
    }

    public void LoadRoom(RoomType type)
    {
        StartCoroutine(TransitionAndLoad(type));
    }

    private IEnumerator TransitionAndLoad(RoomType type)
    {
        if (currentRoom != null)
        {
            yield return StartCoroutine(SlideOutRoom(currentRoom));
            Destroy(currentRoom);
        }

        GameObject prefab = null;
        switch (type)
        {
            case RoomType.Combat: prefab = combatRooms[Random.Range(0, combatRooms.Length)]; break;
            case RoomType.Elite:  prefab = eliteRooms[Random.Range(0, eliteRooms.Length)]; break;
            case RoomType.Shop:   prefab = shopRooms[Random.Range(0, shopRooms.Length)]; break;
            case RoomType.Event:  prefab = eventRoom; break;
            case RoomType.Boss:   prefab = bossRoom; break;
        }

        currentRoom = Instantiate(prefab, spawnPoint.position, Quaternion.identity, roomContainer);

        yield return StartCoroutine(SlideInRoomStylized(currentRoom));
    }

    // 🔻 SORTIE
    private IEnumerator SlideOutRoom(GameObject room)
    {
        Transform[] parts = room.GetComponentsInChildren<Transform>();

        foreach (Transform part in parts)
        {
            if (part == room.transform) continue;

            float delay = (part.position.x - room.transform.position.x) * waveDelayMultiplier;
            delay = Mathf.Clamp(delay, 0f, maxDelay);

            StartCoroutine(FallPart(part, delay));
        }

        yield return new WaitForSeconds(totalAnimDuration);
    }

    private IEnumerator FallPart(Transform part, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPos = part.position;
        Vector3 targetPos = startPos + Vector3.down * fallDistance;

        Vector3 randomRotation = Random.insideUnitSphere * rotationIntensity;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fallSpeed;

            part.position = Vector3.Lerp(startPos, targetPos, t);
            part.Rotate(randomRotation * Time.deltaTime);

            yield return null;
        }
    }

    // 🔺 ENTRÉE
    private IEnumerator SlideInRoomStylized(GameObject room)
    {
        Transform[] parts = room.GetComponentsInChildren<Transform>();

        foreach (Transform part in parts)
        {
            if (part == room.transform) continue;

            Vector3 finalPos = part.position;

            part.position = finalPos + Vector3.down * fallDistance;

            float delay = (finalPos.x - room.transform.position.x) * waveDelayMultiplier;
            delay = Mathf.Clamp(delay, 0f, maxDelay);

            StartCoroutine(RisePart(part, finalPos, delay));
        }

        yield return new WaitForSeconds(totalAnimDuration);
    }

    private IEnumerator RisePart(Transform part, Vector3 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPos = part.position;
        Vector3 randomRotation = Random.insideUnitSphere * rotationIntensity;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * riseSpeed;

            part.position = Vector3.Lerp(startPos, targetPos, t);

            // rotation qui diminue
            part.Rotate(randomRotation * (1f - t) * Time.deltaTime);

            yield return null;
        }

        part.rotation = Quaternion.identity;
    }
}
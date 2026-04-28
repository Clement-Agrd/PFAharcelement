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
    private PlayerController player;

    void Awake()
    {
        Instance = this;
        player   = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // ─────────────────────────────────────────────────────────────
    // API PUBLIQUE
    // ─────────────────────────────────────────────────────────────

    public void LoadRoom(RoomType type)
    {
        StartCoroutine(TransitionAndLoad(type));
    }

    // ─────────────────────────────────────────────────────────────
    // CORE LOGIC
    // ─────────────────────────────────────────────────────────────

    private IEnumerator TransitionAndLoad(RoomType type)
    {
        player.SetMovement(false); // 🔒 Bloque le joueur

        // SORTIE DE L'ANCIENNE SALLE

        if (currentRoom != null)
        {
            foreach (var exit in currentRoom.GetComponentsInChildren<IRoomExit>())
            {
                exit.OnRoomExit();
            }

            yield return StartCoroutine(SlideOutRoom(currentRoom));
            Destroy(currentRoom);
        }


        // CHOIX DU PREFAB
        GameObject prefab = GetRoomPrefab(type);

        // ⚠️ IMPORTANT :
        // On instancie TOUJOURS avec une rotation identité
        // Le prefab DOIT être à (0,0,0)
        currentRoom = Instantiate(
            prefab,
            spawnPoint.position,
            Quaternion.identity,
            roomContainer
        );

        StageManager.Instance.RegisterRoom(currentRoom);

        // ENTRÉE DE LA SALLE
        yield return StartCoroutine(SlideInRoomStylized(currentRoom));

        // ✅ NOTIFICATION "SALLE PRÊTE"
        foreach (var enter in currentRoom.GetComponentsInChildren<IRoomEnter>())
        {
            enter.OnRoomEnter();
        }

        player.SetMovement(true); // 🔓 Débloque le joueur
    }

    // ─────────────────────────────────────────────────────────────
    // PREFAB SELECTION
    // ─────────────────────────────────────────────────────────────

    private GameObject GetRoomPrefab(RoomType type)
    {
        return type switch
        {
            RoomType.Combat => combatRooms[Random.Range(0, combatRooms.Length)],
            RoomType.Elite  => eliteRooms[Random.Range(0, eliteRooms.Length)],
            RoomType.Shop   => shopRooms[Random.Range(0, shopRooms.Length)],
            RoomType.Event  => eventRoom,
            RoomType.Boss   => bossRoom,
            _               => combatRooms[0]
        };
    }

    // ─────────────────────────────────────────────────────────────
    // ANIMATIONS
    // ─────────────────────────────────────────────────────────────


    private IEnumerator SlideOutRoom(GameObject room)
    {
        RoomVisualRoot visualRoot = room.GetComponent<RoomVisualRoot>();
        if (visualRoot == null || visualRoot.visualsRoot == null)
            yield break;

        Transform[] parts = visualRoot.visualsRoot.GetComponentsInChildren<Transform>();

        foreach (Transform part in parts)
        {
            if (part == visualRoot.visualsRoot) continue;

            float delay = Mathf.Clamp(
                (part.position.x - visualRoot.visualsRoot.position.x) * waveDelayMultiplier,
                0f,
                maxDelay
            );

            StartCoroutine(FallPart(part, delay));
        }

        yield return new WaitForSeconds(totalAnimDuration);
    }
    private IEnumerator FallPart(Transform part, float delay)
    {
        if (part == null) yield break;

        yield return new WaitForSeconds(delay);

        if (part == null) yield break;

        Vector3 startPos = part.position;
        Quaternion startRot = part.localRotation;

        Vector3 targetPos = startPos + Vector3.down * fallDistance;
        Vector3 randomRot = Random.insideUnitSphere * rotationIntensity;

        float t = 0f;
        while (t < 1f)
        {
            if (part == null) yield break;

            t += Time.deltaTime * fallSpeed;

            part.position = Vector3.Lerp(startPos, targetPos, t);
            part.Rotate(randomRot * Time.deltaTime);

            yield return null;
        }

        if (part == null) yield break;
        part.localRotation = startRot;
    }


    
    private IEnumerator SlideInRoomStylized(GameObject room)
    {
        RoomVisualRoot visualRoot = room.GetComponent<RoomVisualRoot>();
        if (visualRoot == null || visualRoot.visualsRoot == null)
            yield break;

        Transform[] parts = visualRoot.visualsRoot.GetComponentsInChildren<Transform>();

        foreach (Transform part in parts)
        {
            if (part == visualRoot.visualsRoot) continue;

            Vector3 finalPos = part.position;
            part.position = finalPos + Vector3.down * fallDistance;

            float delay = Mathf.Clamp(
                (finalPos.x - visualRoot.visualsRoot.position.x) * waveDelayMultiplier,
                0f,
                maxDelay
            );

            StartCoroutine(RisePart(part, finalPos, delay));
        }

        yield return new WaitForSeconds(totalAnimDuration);
    }

    private IEnumerator RisePart(Transform part, Vector3 targetPos, float delay)
    {
        if (part == null) yield break;

        yield return new WaitForSeconds(delay);

        if (part == null) yield break;

        Vector3 startPos = part.position;
        Quaternion startRot = part.localRotation;
        Vector3 randomRot = Random.insideUnitSphere * rotationIntensity;

        float t = 0f;
        while (t < 1f)
        {
            if (part == null) yield break;

            t += Time.deltaTime * riseSpeed;

            part.position = Vector3.Lerp(startPos, targetPos, t);
            part.Rotate(randomRot * (1f - t) * Time.deltaTime);

            yield return null;
        }

        if (part == null) yield break;
        part.position = targetPos;
        part.localRotation = startRot;
    }
}

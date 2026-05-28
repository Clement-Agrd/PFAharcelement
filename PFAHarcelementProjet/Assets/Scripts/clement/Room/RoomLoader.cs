using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class RoomLoader : MonoBehaviour
{
    public static RoomLoader Instance;

    private Transform spawnPoint;

    [Header("Rooms")]
    public GameObject[] combatRooms;
    public GameObject[] eliteRooms;
    public GameObject[] shopRooms;
    public GameObject eventRoom;
    public GameObject bossRoom;

    [Header("Setup")]
    public Transform roomContainer;

    [Header("Animation Settings")]
    [SerializeField] private float fallDistance = 15f;
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private float waveDelayMultiplier = 0.1f;
    [SerializeField] private float maxDelay = 0.5f;
    [SerializeField] private float rotationIntensity = 200f;
    [SerializeField] private float totalAnimDuration = 2f;

    [SerializeField] private string spawnLayerName = "Spawn";

    private GameObject currentRoom;

    // ─────────────────────────────────────────────────────────────
    // PLAYER SAFE ACCESS (IMPORTANT)
    // ─────────────────────────────────────────────────────────────

    private PlayerController Player
    {
        get
        {
            GameObject obj = GameObject.FindWithTag("Player");
            if (obj == null) return null;

            return obj.GetComponent<PlayerController>();
        }
    }

    void Awake()
    {
        Instance = this;
        spawnPoint = FindSpawnPoint();
    }
    
    private Transform FindSpawnPoint()
    {
        int spawnLayer = LayerMask.NameToLayer(spawnLayerName);
        if (spawnLayer < 0)
        {
            Debug.LogError($"[RoomLoader] Layer '{spawnLayerName}' introuvable !");
            return null;
        }

        var player = Player;
        if (player == null) return null;

        foreach (Transform t in player.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.layer == spawnLayer)
                return t;
        }

        return null;
    }

    // ─────────────────────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────────────────────

    public void LoadRoom(RoomType type)
    {
        StartCoroutine(TransitionAndLoad(type));
    }

    // ─────────────────────────────────────────────────────────────
    // CORE
    // ─────────────────────────────────────────────────────────────

    private IEnumerator TransitionAndLoad(RoomType type)
    {
        PlayerController player = Player;

        if (player == null)
        {
            Debug.LogError("❌ Player introuvable dans RoomLoader");
            yield break;
        }

        // Disable player safely
        player.SetMovement(false);
        player.SetActions(false);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
            combat.CancelCombat();

        // ─────────────────────────────────────────────
        // EXIT OLD ROOM
        // ─────────────────────────────────────────────

        if (currentRoom != null)
        {
            foreach (var exit in currentRoom.GetComponentsInChildren<IRoomExit>())
                exit.OnRoomExit();

            yield return StartCoroutine(SlideOutRoom(currentRoom));

            Destroy(currentRoom);
        }

        // ─────────────────────────────────────────────
        // SPAWN NEW ROOM
        // ─────────────────────────────────────────────

        GameObject prefab = GetRoomPrefab(type);

        currentRoom = Instantiate(
            prefab,
            spawnPoint != null ? spawnPoint.position : Vector3.zero,
            Quaternion.identity,
            roomContainer
        );

        StageManager.Instance?.RegisterRoom(currentRoom);

        // ─────────────────────────────────────────────
        // ENTER ANIMATION
        // ─────────────────────────────────────────────

        yield return StartCoroutine(SlideInRoomStylized(currentRoom));

        foreach (var enter in currentRoom.GetComponentsInChildren<IRoomEnter>())
            enter.OnRoomEnter();

        // ─────────────────────────────────────────────
        // RE-ENABLE PLAYER (SAFE)
        // ─────────────────────────────────────────────

        player = Player;
        if (player != null)
        {
            CharacterController newCC = player.GetComponent<CharacterController>();
            if (newCC != null)
                newCC.enabled = true;

            player.SetMovement(true);
            player.SetActions(true);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // PREFABS
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
    // ANIMATIONS (UNCHANGED BUT SAFE)
    // ─────────────────────────────────────────────────────────────

    private IEnumerator SlideOutRoom(GameObject room)
    {
        RoomVisualRoot root = room.GetComponent<RoomVisualRoot>();
        if (root == null || root.visualsRoot == null)
            yield break;

        Transform[] parts = root.visualsRoot.GetComponentsInChildren<Transform>();

        foreach (Transform part in parts)
        {
            if (part == root.visualsRoot) continue;

            float delay = Mathf.Clamp(
                (part.position.x - root.visualsRoot.position.x) * waveDelayMultiplier,
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
        Quaternion startRot = part.rotation;

        Vector3 targetPos = startPos + Vector3.down * fallDistance;
        Vector3 randomRot = Random.insideUnitSphere * rotationIntensity;

        float t = 0f;

        while (t < 1f)
        {
            if (part == null) yield break;

            t += Time.deltaTime * fallSpeed;

            part.position = Vector3.Lerp(startPos, targetPos, t);

            part.rotation = startRot * Quaternion.Euler(randomRot * t * Time.deltaTime);

            yield return null;
        }

        if (part == null) yield break;

        part.position = targetPos;
        part.rotation = startRot;
    }

    private IEnumerator SlideInRoomStylized(GameObject room)
    {
        RoomVisualRoot root = room.GetComponent<RoomVisualRoot>();
        if (root == null || root.visualsRoot == null)
            yield break;

        Transform[] parts = root.visualsRoot.GetComponentsInChildren<Transform>();

        foreach (Transform part in parts)
        {
            if (part == root.visualsRoot) continue;

            Vector3 finalPos = part.position;
            part.position = finalPos + Vector3.down * fallDistance;

            float delay = Mathf.Clamp(
                (finalPos.x - root.visualsRoot.position.x) * waveDelayMultiplier,
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
        Quaternion startRot = part.rotation;
        Vector3 randomRot = Random.insideUnitSphere * rotationIntensity;

        float t = 0f;

        while (t < 1f)
        {
            if (part == null) yield break;

            t += Time.deltaTime * riseSpeed;

            part.position = Vector3.Lerp(startPos, targetPos, t);

            part.rotation = startRot * Quaternion.Euler(randomRot * (1f - t) * Time.deltaTime);

            yield return null;
        }

        if (part == null) yield break;

        part.position = targetPos;
        part.rotation = startRot;
    }
}
using UnityEngine;

public class BossZoneState : EnemyStateBase
{
    private BossController boss;
    private float          timer;
    private bool           zonesSpawned;

    public BossZoneState(BossController boss, StateMachine sm)
        : base(boss, sm) => this.boss = boss;

    public override void Enter()
    {
        timer        = 0f;
        zonesSpawned = false;
        enemy.PlayAnim("Cast");
        SpawnZones();
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        // On attend que les zones aient eu le temps d'exploser
        if (timer >= boss.ZoneStateDuration)
            stateMachine.ChangeState(boss.GetRecoveryState());
    }

    private void SpawnZones()
    {
        // Une zone garantie sur le joueur
        if (enemy.PlayerTransform != null)
            SpawnZoneAt(enemy.PlayerTransform.position);

        // Zones aléatoires autour du boss
        for (int i = 0; i < boss.RandomZoneCount; i++)
        {
            Vector2 rand   = Random.insideUnitCircle * boss.ZoneSpawnRadius;
            Vector3 offset = new Vector3(rand.x, 0f, rand.y);
            SpawnZoneAt(enemy.transform.position + offset);
        }
    }

    private void SpawnZoneAt(Vector3 worldPos)
    {
        int mask = LayerMask.GetMask("Ground");
        Vector3 rayOrigin = new Vector3(worldPos.x, 20f, worldPos.z);

        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 40f, mask))
        {
            Debug.Log("SpawnZone annulé — pas de sol détecté");
            return; // ← ne spawn pas si pas de Ground
        }

        Object.Instantiate(boss.ZonePrefab, hit.point, Quaternion.identity);
    }
}
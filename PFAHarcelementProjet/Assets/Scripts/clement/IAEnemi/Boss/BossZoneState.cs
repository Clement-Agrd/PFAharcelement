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
        // Projette au sol via raycast
        if (Physics.Raycast(worldPos + Vector3.up * 5f, Vector3.down,
                out RaycastHit hit, 20f))
            worldPos = hit.point;

        Object.Instantiate(boss.ZonePrefab, worldPos, Quaternion.identity);
    }
}
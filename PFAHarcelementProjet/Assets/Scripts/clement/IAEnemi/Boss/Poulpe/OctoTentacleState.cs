using System.Collections;
using UnityEngine;

public class OctoTentacleState : EnemyStateBase
{
    private OctoBossController octo;
    private float timer;
    private bool  done;

    public OctoTentacleState(OctoBossController boss, StateMachine sm)
        : base(boss, sm) => octo = boss;

    public override void Enter()
    {
        timer = 0f;
        done  = false;
        enemy.PlayAnim("Cast");
        enemy.StartCoroutine(SpawnWaves());
    }

    public override void Update()
    {
        if (done)
            stateMachine.ChangeState(octo.GetRecoveryState());
    }

    private IEnumerator SpawnWaves()
    {
        // Vague 1
        SpawnTentacles();
        yield return new WaitForSeconds(octo.WaveInterval);

        // Vague 2 — plus rapide (warning réduit)
        SpawnTentacles(faster: true);
        yield return new WaitForSeconds(octo.WaveInterval);

        done = true;
    }

    private void SpawnTentacles(bool faster = false)
    {
        // Une zone garantie sur le joueur
        if (enemy.PlayerTransform != null)
            SpawnAt(enemy.PlayerTransform.position, faster);

        // 7 zones aléatoires autour du boss
        for (int i = 0; i < 7; i++)
        {
            Vector2 rand   = Random.insideUnitCircle * octo.TentacleSpawnRadius;
            Vector3 offset = new Vector3(rand.x, 0f, rand.y);
            SpawnAt(enemy.transform.position + offset, faster);
        }
    }

    private void SpawnAt(Vector3 worldPos, bool faster)
    {
        int mask = ~LayerMask.GetMask("Player", "Enemy", "Boss");
        Vector3 origin = new Vector3(worldPos.x, 20f, worldPos.z);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 40f, mask))
            worldPos = hit.point;

        GameObject go = Object.Instantiate(
            octo.TentaclePrefab, worldPos, Quaternion.identity);

        // Deuxième vague plus rapide
        if (faster)
        {
            var tz = go.GetComponent<TentacleZone>();
            if (tz != null) tz.warningDuration *= 0.5f;
        }
    }
}
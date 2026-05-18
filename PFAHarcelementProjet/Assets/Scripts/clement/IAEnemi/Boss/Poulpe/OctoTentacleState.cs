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

        // 7 zones aléatoires autour du joueur
        if (enemy.PlayerTransform != null)
        {
            for (int i = 0; i < 7; i++)
            {
                Vector2 rand   = Random.insideUnitCircle * octo.TentacleSpawnRadius;
                Vector3 offset = new Vector3(rand.x, 0f, rand.y);
                SpawnAt(enemy.PlayerTransform.position + offset, faster);
            }
        }
    }

    private void SpawnAt(Vector3 worldPos, bool faster)
    {
        int mask = LayerMask.GetMask("Ground");
        Vector3 origin = new Vector3(worldPos.x, 20f, worldPos.z);

        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 40f, mask))
        {
            Debug.Log("SpawnTentacle annulé — pas de sol détecté");
            return; // ← ne spawn pas si pas de Ground
        }

        GameObject go = Object.Instantiate(
            octo.TentaclePrefab, hit.point, Quaternion.identity);

        if (faster)
        {
            var tz = go.GetComponent<TentacleZone>();
            if (tz != null) tz.warningDuration *= 0.5f;
        }
    }
}
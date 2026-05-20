using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSummonState : EnemyStateBase
{
    private ThirdBossController boss;
    private bool done;

    public BossSummonState(ThirdBossController boss, StateMachine sm)
        : base(boss, sm) => this.boss = boss;

    public override void Enter()
    {
        done = false;
        enemy.TriggerAnim("Alert");
        enemy.StartCoroutine(SummonRoutine());
    }

    public override void Update()
    {
        if (done)
            stateMachine.ChangeState(boss.GetRecoveryState());
    }

    private IEnumerator SummonRoutine()
    {
        // 🔮 Telegraph
        yield return new WaitForSeconds(boss.SummonWindup);

        List<GameObject> spawned = new List<GameObject>();

        int groundMask = LayerMask.GetMask("Ground");

        // ✅ Spawn les ennemis
        for (int i = 0; i < boss.SummonCount; i++)
        {
            float angle = i * (360f / boss.SummonCount);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(rad), 0f, Mathf.Sin(rad)
            ) * boss.SummonRadius;

            Vector3 spawnPos = enemy.transform.position + offset;

            // ✅ Vérifie qu'on touche bien le sol
            bool validSpawn = Physics.Raycast(
                spawnPos + Vector3.up * 10f,
                Vector3.down,
                out RaycastHit hit,
                20f,
                groundMask
            );

            if (!validSpawn)
            {
                Debug.LogWarning("❌ Spawn annulé (pas de sol)");
                continue; // NE PAS spawn si invalide
            }

            spawnPos = hit.point;

            GameObject go = Object.Instantiate(
                boss.SummonPrefab,
                spawnPos,
                Quaternion.identity
            );

            spawned.Add(go);
        }

        // ✅ Sécurité : si rien spawn → on sort direct
        if (spawned.Count == 0)
        {
            Debug.LogWarning("⚠️ Aucun ennemi invoqué");
            done = true;
            yield break;
        }

        // ✅ Attente des morts (safe + anti softlock)
        float maxWait = 15f;
        float timer = 0f;

        while (timer < maxWait)
        {
            timer += 0.5f;

            spawned.RemoveAll(go =>
            {
                if (go == null) return true;

                var e = go.GetComponent<EnemyController>();

                // ✅ pas d'EnemyController = ignoré
                if (e == null) return true;

                return e.IsDead;
            });

            Debug.Log("Invocs restantes: " + spawned.Count);

            if (spawned.Count == 0)
                break;

            yield return new WaitForSeconds(0.5f);
        }

        // ✅ même si bug → on continue
        done = true;
    }
}
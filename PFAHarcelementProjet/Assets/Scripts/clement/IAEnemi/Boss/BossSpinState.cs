using System.Collections;
using UnityEngine;

public class BossSpinState : EnemyStateBase
{
    private BossController boss;

    private enum Phase { Telegraph, Spinning, Expanding }
    private Phase phase;
    private float timer;
    private float initialEmissionRate;

    // Valeurs initiales du swarm pour pouvoir les reset au Exit
    private float initialSwarmRadius;
    private float initialSwarmSpeed;
    private float initialNoiseStrength;

    public BossSpinState(BossController boss, StateMachine sm)
        : base(boss, sm) => this.boss = boss;

    // ── Enter ─────────────────────────────────────────────────────────
    public override void Enter()
    {
        timer = 0f;
        phase = Phase.Telegraph;
        enemy.PlayAnim("Idle");

        // Sauvegarde les valeurs initiales du swarm
        if (boss.PiranaSwarm != null)
        {
            initialSwarmRadius    = boss.PiranaSwarm.shape.radius;
            initialSwarmSpeed     = boss.PiranaSwarm.main.startSpeed.constant;
            initialNoiseStrength  = boss.PiranaSwarm.noise.strength.constant;
            initialEmissionRate = boss.PiranaSwarm.emission.rateOverTimeMultiplier;
        }

        StartTelegraph();
    }
    
    // ── Update ────────────────────────────────────────────────────────
    public override void Update()
    {
        timer += Time.deltaTime;

        switch (phase)
        {
            case Phase.Telegraph:   UpdateTelegraph();  break;
            case Phase.Spinning:    UpdateSpinning();   break;
            case Phase.Expanding:   UpdateExpanding();  break;
        }
    }

    // ── Telegraph ─────────────────────────────────────────────────────
    private void StartTelegraph()
    {
        // Le swarm se resserre vers le centre → signal visuel que quelque chose se prépare
        if (boss.PiranaSwarm != null)
        {
            var noise       = boss.PiranaSwarm.noise;
            var emission    = boss.PiranaSwarm.emission;

            // Accélère l'agitation des piranhas
            noise.strengthMultiplier = boss.TelegraphNoiseStrength;

            // Augmente le nombre de piranhas → le boss "gonfle"
            emission.rateOverTimeMultiplier = boss.TelegraphEmissionRate;
        }

        boss.TelegraphVFX?.Play(); // ring ou glow au sol
    }

    private void UpdateTelegraph()
    {
        // Pendant le telegraph le swarm se contracte progressivement
        if (boss.PiranaSwarm != null)
        {
            float t     = timer / boss.TelegraphDuration;
            var   shape = boss.PiranaSwarm.shape;

            // Rayon qui rétrécit → les piranhas se rassemblent
            shape.radius = Mathf.Lerp(initialSwarmRadius,
                                      initialSwarmRadius * 0.3f, t);
        }

        if (timer >= boss.TelegraphDuration)
        {
            timer = 0f;
            phase = Phase.Spinning;
            boss.TelegraphVFX?.Stop();
            enemy.PlayAnim("Spin");
        }
    }

    // ── Spin ──────────────────────────────────────────────────────────
    private void UpdateSpinning()
    {
        enemy.transform.Rotate(Vector3.up, boss.SpinSpeed * Time.deltaTime);

        // Le swarm tourne avec le boss et commence à s'élargir légèrement
        if (boss.PiranaSwarm != null)
        {
            float t     = timer / boss.SpinDuration;
            var   shape = boss.PiranaSwarm.shape;
            shape.radius = Mathf.Lerp(initialSwarmRadius * 0.7f,
                                      initialSwarmRadius, t);
        }

        if (timer >= boss.SpinDuration)
        {
            timer = 0f;
            phase = Phase.Expanding;
            enemy.PlayAnim("Expand");
        }
    }

    // ── Expand ────────────────────────────────────────────────────────
    private void UpdateExpanding()
    {
        float t = timer / boss.ExpandDuration;

        if (boss.PiranaSwarm != null)
        {
            var shape = boss.PiranaSwarm.shape;

            if (t <= 0.5f)
            {
                // Première moitié → radius explose + spin ultra rapide
                float expandT  = t / 0.4f;
                shape.radius   = Mathf.Lerp(initialSwarmRadius, boss.AoERadius,
                    Mathf.SmoothStep(0f, 1f, expandT));

                enemy.transform.Rotate(Vector3.up, boss.SpinSpeed * 3f * Time.deltaTime);
            }
            else
            {
                // Deuxième moitié → radius revient, spin ralentit
                float retractT = (t - 0.5f) / 0.5f;
                shape.radius   = Mathf.Lerp(boss.AoERadius, initialSwarmRadius,
                    Mathf.SmoothStep(0f, 1f, retractT));

                enemy.transform.Rotate(Vector3.up, boss.SpinSpeed * 3f
                                                                  * (1f - retractT) * Time.deltaTime);
            }
        }

        // Dégâts seulement pendant l'expansion
        if (t <= 0.5f)
            DamageInRadius(Mathf.Lerp(0f, boss.AoERadius, t / 0.5f));

        if (timer >= boss.ExpandDuration)
            stateMachine.ChangeState(boss.GetRecoveryState());
    }

    // ── Helpers ───────────────────────────────────────────────────────
    private void DamageInRadius(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerHealth>()
                   ?.TakeDamage(boss.AoEDamagePerSecond * Time.deltaTime);
        }
    }

    // ── Exit ──────────────────────────────────────────────────────────
    public override void Exit()
    {
        enemy.transform.rotation = Quaternion.identity;

        boss.TelegraphVFX?.Stop();

        // Reset le swarm à ses valeurs initiales
        if (boss.PiranaSwarm != null)
        {
            var shape    = boss.PiranaSwarm.shape;
            var main     = boss.PiranaSwarm.main;
            var noise    = boss.PiranaSwarm.noise;
            var emission = boss.PiranaSwarm.emission;

            shape.radius                    = initialSwarmRadius;
            main.startSpeedMultiplier       = initialSwarmSpeed;
            noise.strengthMultiplier        = initialNoiseStrength;
            emission.rateOverTimeMultiplier = initialEmissionRate; // ✅ au lieu de 30f
        }
    }
}
using System.Collections;
using UnityEngine;

public class BossSpinState : EnemyStateBase
{
    private BossController boss;

    private enum Phase { Telegraph, Spinning, Expanding }
    private Phase phase;
    private float timer;

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
            boss.SpinVFX?.Play();
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
            shape.radius = Mathf.Lerp(initialSwarmRadius * 0.3f,
                                      initialSwarmRadius, t);
        }

        if (timer >= boss.SpinDuration)
        {
            timer = 0f;
            phase = Phase.Expanding;
            boss.SpinVFX?.Stop();
            boss.ExpandVFX?.Play();
            enemy.PlayAnim("Expand");
        }
    }

    // ── Expand ────────────────────────────────────────────────────────
    private void UpdateExpanding()
    {
        float t = timer / boss.ExpandDuration;

        // Le swarm explose vers l'extérieur visuellement
        if (boss.PiranaSwarm != null)
        {
            var shape      = boss.PiranaSwarm.shape;
            var main       = boss.PiranaSwarm.main;
            var noise      = boss.PiranaSwarm.noise;

            shape.radius                 = Mathf.Lerp(initialSwarmRadius,
                                                      boss.AoERadius, t);
            main.startSpeedMultiplier    = Mathf.Lerp(0f, 4f, t);
            noise.strengthMultiplier     = Mathf.Lerp(boss.TelegraphNoiseStrength,
                                                      0.1f, t); // moins d'agitation quand ils s'éparpillent
        }

        // Dégâts continus dans le rayon croissant
        float currentRadius = Mathf.Lerp(0f, boss.AoERadius, t);
        DamageInRadius(currentRadius);

        if (timer >= boss.ExpandDuration)
        {
            boss.ExpandVFX?.Stop();
            stateMachine.ChangeState(boss.GetRecoveryState());
        }
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
        boss.SpinVFX?.Stop();
        boss.ExpandVFX?.Stop();

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
            emission.rateOverTimeMultiplier = 30f; // ta valeur de base
        }
    }
}
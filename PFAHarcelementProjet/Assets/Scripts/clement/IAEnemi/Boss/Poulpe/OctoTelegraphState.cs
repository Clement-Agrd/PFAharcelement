using UnityEngine;

public class OctoTelegraphState : EnemyStateBase
{
    private OctoBossController octo;
    private int   patternIndex;
    private float timer;

    public OctoTelegraphState(OctoBossController boss, StateMachine sm, int pattern)
        : base(boss, sm)
    {
        octo         = boss;
        patternIndex = pattern;
    }

    // Constructeur qui accepte l'enum castée en int
    public OctoTelegraphState(OctoBossController boss, StateMachine sm, System.Enum pattern)
        : this(boss, sm, System.Convert.ToInt32(pattern)) { }

    public override void Enter()
    {
        timer = 0f;
        enemy.TriggerAnim("Attack"); // animation d'attaque = telegraph visuel
        octo.TelegraphVFX?.Play();
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= octo.TelegraphDuration)
            octo.LaunchPattern(patternIndex);
    }

    public override void Exit()
    {
        octo.TelegraphVFX?.Stop();
    }
}
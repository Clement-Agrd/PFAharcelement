using UnityEngine;

public class BossRecoveryState : EnemyStateBase
{
    private float duration;
    private float timer;
    // Callback pour choisir le pattern — chaque boss passe sa propre méthode
    private System.Action onComplete;

    public BossRecoveryState(EnemyController enemy, StateMachine sm,
        float duration, System.Action onComplete)
        : base(enemy, sm)
    {
        this.duration   = duration;
        this.onComplete = onComplete;
    }

    public override void Enter()
    {
        timer = 0f;
        enemy.PlayAnim("Idle");
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            onComplete?.Invoke();
    }
}
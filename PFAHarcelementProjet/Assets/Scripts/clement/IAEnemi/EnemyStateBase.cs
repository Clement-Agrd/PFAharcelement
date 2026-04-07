public abstract class EnemyStateBase
{
    protected EnemyController enemy;
    protected StateMachine    stateMachine;

    public EnemyStateBase(EnemyController enemy, StateMachine stateMachine)
    {
        this.enemy        = enemy;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()        { }
    public virtual void Update()       { }
    public virtual void FixedUpdate()  { }
    public virtual void Exit()         { }
}
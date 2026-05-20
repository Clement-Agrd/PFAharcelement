using UnityEngine;
using UnityEngine.AI;

public class StateMachine
{
    public EnemyStateBase CurrentState { get; private set; }

    public void Initialize(EnemyStateBase startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }


    public void ChangeState(EnemyStateBase newState)
    {
        if (newState == null)
        {
            Debug.LogError("⚠️ ChangeState appelé avec NULL !");
            return;
        }

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }


    public void Update()       => CurrentState?.Update();
    public void FixedUpdate()  => CurrentState?.FixedUpdate();
}
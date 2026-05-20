using System.Collections;
using UnityEngine;

public class OctoSweepState : EnemyStateBase
{
    private OctoBossController octo;
    private bool done;

    public OctoSweepState(OctoBossController boss, StateMachine sm)
        : base(boss, sm) => octo = boss;

    public override void Enter()
    {
        done = false;
        enemy.PlayAnim("Attack");
        enemy.StartCoroutine(SweepRoutine());
    }

    public override void Update()
    {
        if (done) stateMachine.ChangeState(octo.GetRecoveryState());
    }

    private IEnumerator SweepRoutine()
    {
        // Choix aléatoire d'une des lignes prédéfinies
        OctoSweepLine line = octo.SweepLines[
            Random.Range(0, octo.SweepLines.Length)];

        // Spawn les tentacules avec un décalage progressif
        for (int i = 0; i < line.spawnPoints.Length; i++)
        {
            GameObject go = Object.Instantiate(
                octo.SweepTentaclePrefab,
                line.spawnPoints[i].position,
                line.spawnPoints[i].rotation
            );

            var st = go.GetComponent<SweepTentacle>();
            if (st != null)
            {
                st.slideDirection = line.slideDirection;
                st.fallDelay      = i * octo.SweepStaggerDelay;
            }
        }

        // Attend que toute la ligne soit passée
        yield return new WaitForSeconds(
            octo.SweepStaggerDelay * line.spawnPoints.Length
            + octo.SweepTotalDuration);

        done = true;
    }
}

// Défini dans l'Inspector — une ligne de tentacules
[System.Serializable]
public class OctoSweepLine
{
    public Transform[] spawnPoints;   // positions prédéfinies sur les bords
    public Vector3     slideDirection; // vers où ils glissent (ex: Vector3.forward)
}
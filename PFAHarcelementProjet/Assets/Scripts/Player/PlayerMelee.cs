// Scripts/Player/PlayerMelee.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMelee : MonoBehaviour
{
    public bool IsAttacking { get; private set; }

    HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    [Header("Détection")]
    public float hitRange  = 1.5f;
    public float coneAngle = 60f;

    [Header("Rotation")]
    public float attackRotateSpeed = 720f;
    public float maxRotateTime     = 0.15f;

    [Header("Dash (premier coup seulement)")]
    public float dashDistance = 3f;
    public float dashSpeed    = 12f;

    [Header("Timing")]
    public float pauseAtPeak = 0.2f;

    [Header("Combo")]
    public float comboBufferTime = 0.25f;

    bool  comboQueued;
    float comboTimer;
    bool  hasDashedThisChain;

    [Header("Références")]
    public Animator animator;

    PlayerStats         stats;
    PlayerController    controller;
    CharacterController cc;

    float nextAttack;

    void Awake()
    {
        stats      = GetComponent<PlayerStats>();
        controller = GetComponent<PlayerController>();
        cc         = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (comboQueued)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                comboQueued = false;
        }
    }

    public void TryAttack(Vector3 attackDirection)
    {
        if (IsAttacking)
        {
            comboQueued = true;
            comboTimer  = comboBufferTime;
            return;
        }

        if (!controller.CanAct || controller.IsDashing) return;

        float attackSpeed       = stats.GetStat(StatType.AttackSpeed);
        float cooldownReduction = stats.GetStat(StatType.CooldownReduction);
        float cooldown = (1f / attackSpeed) *
                         (1f - Mathf.Clamp01(cooldownReduction));

        if (Time.time < nextAttack)          return;
        if (attackDirection == Vector3.zero) return;

        // Son mêlée
        if (PlayerSoundManager.Instance != null)
            PlayerSoundManager.Instance.PlayMelee();

        animator?.CrossFade("Armature|Attaque", 0.05f, 0, 0f);

        bool withDash      = !hasDashedThisChain;
        hasDashedThisChain = true;

        StartCoroutine(RotateAndAttack(attackDirection.normalized, withDash));
        nextAttack = Time.time + cooldown;
    }

    IEnumerator RotateAndAttack(Vector3 direction, bool withDash)
    {
        IsAttacking = true;
        hitTargets.Clear();

        Quaternion startRot  = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        float angle = Quaternion.Angle(startRot, targetRot);
        float tMax  = Mathf.Min(angle / attackRotateSpeed, maxRotateTime);

        float t = 0f;
        while (t < tMax)
        {
            transform.rotation = Quaternion.Slerp(
                startRot, targetRot, t / tMax);
            t += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
        StartCoroutine(MeleeRoutine(direction, withDash));
    }

    IEnumerator MeleeRoutine(Vector3 direction, bool withDash)
    {
        controller.SetActions(false);
        controller.SetMovement(false);

        if (withDash)
        {
            float traveled = 0f;
            while (traveled < dashDistance)
            {
                float step = dashSpeed * Time.deltaTime;
                cc.Move(direction * step);
                traveled += step;
                CheckConeDamage(direction);
                yield return null;
            }
        }
        else
        {
            CheckConeDamage(direction);
            yield return new WaitForSeconds(0.05f);
        }

        if (pauseAtPeak > 0f)
            yield return new WaitForSeconds(pauseAtPeak);

        controller.SetMovement(true);
        controller.SetActions(true);
        IsAttacking = false;

        if (comboQueued)
        {
            comboQueued = false;
            StartCoroutine(RotateAndAttack(direction, false));
        }
        else
        {
            hasDashedThisChain = false;
        }
    }

    void CheckConeDamage(Vector3 forward)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hitRange);

        foreach (Collider hit in hits)
        {
            if (hit.transform.root == transform.root) continue;

            IDamageable dmg = hit.GetComponent<IDamageable>();
            if (dmg == null || hitTargets.Contains(dmg)) continue;

            Vector3 toTarget = (hit.transform.position -
                                transform.position).normalized;
            float angle = Vector3.Angle(forward, toTarget);

            if (angle <= coneAngle * 0.5f)
            {
                ApplyDamage(dmg);
                hitTargets.Add(dmg);
            }
        }
    }

    void ApplyDamage(IDamageable target)
    {
        float damage = stats.GetStat(StatType.MeleeDamage);
        target.TakeDamage(damage);

        float lifeSteal = stats.GetStat(StatType.LifeSteal);
        if (lifeSteal > 0f)
            GetComponent<PlayerHealth>()?.Heal(damage * lifeSteal);
    }

    public void CancelAttack()
    {
        StopAllCoroutines();
        hitTargets.Clear();
        comboQueued        = false;
        hasDashedThisChain = false;
        IsAttacking        = false;

        controller.SetMovement(true);
        controller.SetActions(true);
    }
}
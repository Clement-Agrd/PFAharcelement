// Scripts/Ultimates/ShieldUltimate.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShieldUltimate : MonoBehaviour
{
    [Header("Visuel")]
    public GameObject shieldVFX; // bulle autour du requin

    private float          shieldDuration;
    private float          shieldCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerHealth   playerHealth;

    public bool  IsActive           => isActive;
    public float GetRemaining()     => Mathf.Max(0f, shieldCooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => GetRemaining() / shieldCooldown;
    public bool  IsReady()          => GetRemaining() <= 0f && !isActive;

    void Awake()
    {
        controls     = new PlayerControls();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.SpecialAttack.performed += OnInput;
    }

    void OnDisable()
    {
        controls.Player.SpecialAttack.performed -= OnInput;
        controls.Player.Disable();
    }

    public void Setup(float duration, float cooldown)
    {
        shieldDuration = duration;
        shieldCooldown = cooldown;

        // Crée le VFX bouclier si pas encore fait
        if (shieldVFX == null)
        {
            shieldVFX = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shieldVFX.transform.SetParent(transform);
            shieldVFX.transform.localPosition = Vector3.zero;
            shieldVFX.transform.localScale    = Vector3.one * 3f;

            // Material transparent bleu
            Renderer rend = shieldVFX.GetComponent<Renderer>();
            Material mat  = new Material(Shader.Find("Standard"));
            mat.color = new Color(0f, 0.5f, 1f, 0.3f);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend",  (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend",  (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite",    0);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            rend.material   = mat;

            // Retire le collider de la bulle
            Destroy(shieldVFX.GetComponent<Collider>());
            shieldVFX.SetActive(false);
        }
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(ShieldCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator ShieldCoroutine()
    {
        isActive = true;

        if (shieldVFX != null)
            shieldVFX.SetActive(true);

        // Active l'invincibilité
        if (playerHealth != null)
            playerHealth.SetInvincible(true);

        Debug.Log($"🛡️ Bouclier actif pendant {shieldDuration}s");

        yield return new WaitForSeconds(shieldDuration);

        // Désactive l'invincibilité
        if (playerHealth != null)
            playerHealth.SetInvincible(false);

        if (shieldVFX != null)
            shieldVFX.SetActive(false);

        isActive = false;

        Debug.Log("🛡️ Bouclier terminé");
    }
}
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    private ParticleSystem[] _all;

    void Awake()
    {
        // true = inclut les GameObjects inactifs
        _all = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in _all)
        {
            ps.Stop(true);
            ps.Clear(true);
        }
    }

    public void Play()
    {
        gameObject.SetActive(true); // ← réactive si inactif
        foreach (var ps in _all)
        {
            ps.Stop(true);
            ps.Clear(true);
            ps.Play(true);
        }
    }

    public void Stop()
    {
        foreach (var ps in _all) ps.Stop(true);
        gameObject.SetActive(false); // ← remet inactif après
    }

    public bool IsPlaying()
    {
        foreach (var ps in _all)
            if (ps.isPlaying) return true;
        return false;
    }
}
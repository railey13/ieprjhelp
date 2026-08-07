using System;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectManager : MonoBehaviour
{
    public static HitEffectManager Instance { get; private set; }

    [Serializable]
    public class HitEffectEntry
    {
        public DamageSubtype subtype;
        public GameObject vfxPrefab;
    }

    [Header("Hit Effects")]
    [SerializeField] private List<HitEffectEntry> hitEffects = new List<HitEffectEntry>();
    [SerializeField] private GameObject defaultVFXPrefab; // fallback if no match found

    private readonly Dictionary<DamageSubtype, GameObject> _lookup = new Dictionary<DamageSubtype, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLookup();
    }

    private void BuildLookup()
    {
        _lookup.Clear();
        foreach (var entry in hitEffects)
        {
            if (entry == null || entry.vfxPrefab == null) continue;

            if (!_lookup.ContainsKey(entry.subtype))
                _lookup.Add(entry.subtype, entry.vfxPrefab);
            else
                Debug.LogWarning($"[HitEffectManager] Duplicate entry for {entry.subtype} ignored.");
        }
    }

    // Spawns the correct VFX prefab for a given damage subtype at a world position.
    public GameObject PlayEffect(DamageSubtype subtype, Vector3 position, float scale = 1f, float destroyDelay = 1.0f)
    {
        GameObject prefab = GetPrefabFor(subtype);

        if (prefab == null)
        {
            Debug.LogWarning($"[HitEffectManager] No VFX prefab found for {subtype} and no default set.");
            return null;
        }

        GameObject effect = Instantiate(prefab, position, Quaternion.identity);
        effect.transform.localScale = Vector3.one * scale;

        Destroy(effect, destroyDelay);
        return effect;
    }

    private GameObject GetPrefabFor(DamageSubtype subtype)
    {
        if (_lookup.TryGetValue(subtype, out var prefab))
            return prefab;

        return defaultVFXPrefab;
    }
}
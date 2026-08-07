using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private float vfxScale = 2f;
    [SerializeField] private float vfxYOffset = 1f;
    [SerializeField] private float destroyDelay = 1.0f;

    public void PlayHitEffect(DamageSubtype subtype)
    {
        Debug.Log("PlayHitEffect called on " + gameObject.name + " for subtype " + subtype);

        if (HitEffectManager.Instance == null)
        {
            Debug.LogWarning("No HitEffectManager in scene.");
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * vfxYOffset;
        HitEffectManager.Instance.PlayEffect(subtype, spawnPos, vfxScale, destroyDelay);
    }
}
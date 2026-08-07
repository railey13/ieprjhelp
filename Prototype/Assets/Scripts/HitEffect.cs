using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private float vfxScale = 2f;
    [SerializeField] private float vfxScaling = 2.0f;
    private float vfxYOffset = -0.5f;
    
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
        HitEffectManager.Instance.PlayEffect(subtype, spawnPos, vfxScale * vfxScaling, destroyDelay);
    }
}
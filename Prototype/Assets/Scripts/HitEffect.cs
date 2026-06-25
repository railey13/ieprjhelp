using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private GameObject hitVFXPrefab; // effect
    [SerializeField] private float destroyDelay = 1.0f; // how long before the effect is destroyed
    [SerializeField] private float vfxScale = 2f;
    [SerializeField] private float vfxYOffset = 1f;
    public void PlayHitEffect()
    {

        Debug.Log("PlayHitEffect called on " + gameObject.name);

        if (hitVFXPrefab == null)
        {
            Debug.LogWarning("No hit VFX prefab assigned on " + gameObject.name);
            return;
        }

        // spawn the effect at this unit's position
        Debug.Log("Spawning VFX at " + transform.position);
        Vector3 spawnPos = transform.position + Vector3.up * vfxYOffset;
        GameObject effect = Instantiate(hitVFXPrefab, spawnPos, Quaternion.identity);
        Debug.Log("VFX spawned: " + effect.name);

        effect.transform.localScale = Vector3.one * vfxScale;

        // destroy it after the effect finishes
        Destroy(effect, destroyDelay);
    }
}
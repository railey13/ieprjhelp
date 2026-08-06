using UnityEngine;

public class VaericUnit : PlayerClass
{
    [Header("Vaeric Passive")]
    public int selfDamagePerAction = 5;
    public bool hasRevived = false;
    public float reviveAtkBoost = 10f;
    public float revivedDamageMultiplier = 1.3f;

    public override void BasicAttack(EnemyClass target)
    {
        if (target == null)
            return;

        float damage = ApplyVaericDamageBonus(atk);

        // Enzo changes: I override this so Vaeric basic attacks also use his revived damage bonus
        target.TakeDamage(damage);

        TakeSelfDamageFromAction();
    }

    public override void TakeDamage(float damage)
    {
        int damageInt = Mathf.FloorToInt(damage);

        // Enzo changes: I check lethal damage here so Vaeric can revive once before actually dying
        if (!hasRevived && hp - damageInt <= 0)
        {
            hp = 1;
            hasRevived = true;
            atk += reviveAtkBoost;

            Debug.Log(UnitName + " revived at 1 HP and gained attack.");

            HitEffect hitEffect = GetComponent<HitEffect>();
            if (hitEffect != null)
                hitEffect.PlayHitEffect();

            return;
        }

        hp -= damageInt;

        Debug.Log(UnitName + " took " + damage + " damage. Current HP: " + hp);

        HitEffect normalHitEffect = GetComponent<HitEffect>();
        if (normalHitEffect != null)
            normalHitEffect.PlayHitEffect();

        if (hp <= 0)
        {
            hp = 0;
            Debug.Log(UnitName + " has died.");
        }
    }

    public void TakeSelfDamageFromAction()
    {
        if (selfDamagePerAction <= 0)
            return;

        // Enzo changes: I keep this separate so Vaeric skills can call it after the action resolves
        TakeDamage(selfDamagePerAction);
    }

    public float ApplyVaericDamageBonus(float damage)
    {
        if (hasRevived)
            return damage * revivedDamageMultiplier;

        return damage;
    }
}

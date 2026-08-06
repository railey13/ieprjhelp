using UnityEngine;

public class HuxleyUnit : PlayerClass, IOutgoingDamageModifier
{
    [Header("Lycanthrope")]
    public float damageNeededToTransform = 40;
    public float damageTakenTotal = 0;
    public bool isLycanthrope = false;

    [Header("Transformation Buffs")]
    public int atkBoost = 15;
    public int rangeBoost = 1;
    public int movementBoost = 2;
    public float transformedDamageMultiplier = 1.25f;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (hp <= 0)
            return;

        damageTakenTotal += damage;

        if (!isLycanthrope && damageTakenTotal >= damageNeededToTransform)
        {
            isLycanthrope = true;

            atk += atkBoost;
            range += rangeBoost;
            movement += movementBoost;

            Debug.Log(UnitName + " entered Lycanthrope form");
        }
    }

    public int ModifyOutgoingDamage(int damage)
    {
        if (!isLycanthrope)
            return damage;

        // Enzo changes: this makes Huxley hit harder after transforming
        return Mathf.RoundToInt(damage * transformedDamageMultiplier);
    }
}
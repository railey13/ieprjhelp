using UnityEngine;

public class HuxleyUnit : PlayerClass, IOutgoingDamageModifier
{
    [Header("Huxley Lycanthrope")]
    public float damageNeededToTransform = 40f;
    public float damageTakenTotal = 0f;
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

        if (damage <= 0)
            return;

        damageTakenTotal += damage;

        TryTransform();
    }

    private void TryTransform()
    {
        if (isLycanthrope)
            return;

        if (damageTakenTotal < damageNeededToTransform)
            return;

        isLycanthrope = true;

        atk += atkBoost;
        range += rangeBoost;
        movement += movementBoost;

        Debug.Log(UnitName + " entered Lycanthrope form");
    }

    public int ModifyOutgoingDamage(int damage)
    {
        if (!isLycanthrope)
            return damage;

        // Enzo changes: transformed Huxley hits harder
        return Mathf.RoundToInt(damage * transformedDamageMultiplier);
    }
}

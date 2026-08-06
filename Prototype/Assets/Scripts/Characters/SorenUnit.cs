using UnityEngine;

public class SorenUnit : PlayerClass, IOutgoingDamageModifier
{
    [Header("Soren Passive")]
    public bool weakenedNextAttack = false;
    public float damageMultiplierAfterBeingHit = 0.75f;

    [Header("Armor Chip Movement")]
    public int movementGainWhenHit = 1;
    public int maxExtraMovementFromHits = 3;

    private int movementGainedFromHits = 0;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (hp <= 0)
            return;

        if (damage <= 0)
            return;

        // Enzo changes: after Soren gets hit his armor chips and changes how his next attack works
        weakenedNextAttack = true;

        if (movementGainedFromHits < maxExtraMovementFromHits)
        {
            movement += movementGainWhenHit;
            movementGainedFromHits += movementGainWhenHit;
        }

        Debug.Log(UnitName + " got hit, armor chipped, and movement increased");
    }

    public int ModifyOutgoingDamage(int damage)
    {
        if (!weakenedNextAttack)
            return damage;

        weakenedNextAttack = false;

        // Enzo changes: this keeps the earlier downside after getting hit
        int loweredDamage = Mathf.RoundToInt(damage * damageMultiplierAfterBeingHit);

        Debug.Log(UnitName + "'s damage got lowered because he was hit");

        return loweredDamage;
    }
}

using UnityEngine;

public class SorenUnit : PlayerClass, IOutgoingDamageModifier
{
    public bool weakenedNextAttack = false;
    public float damageMultiplierAfterBeingHit = 0.75f;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (hp > 0 && damage > 0)
        {
            // Enzo changes: after Soren gets hit his next damage is lower
            weakenedNextAttack = true;

            Debug.Log(UnitName + " will do less damage on his next attack");
        }
    }

    public int ModifyOutgoingDamage(int damage)
    {
        if (!weakenedNextAttack)
            return damage;

        weakenedNextAttack = false;

        // Enzo changes: Soren does less damage after getting hit
        int loweredDamage = Mathf.RoundToInt(damage * damageMultiplierAfterBeingHit);

        Debug.Log(UnitName + "'s damage got lowered because he was hit");

        return loweredDamage;
    }
}
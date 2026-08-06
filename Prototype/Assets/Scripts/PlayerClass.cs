using UnityEngine;

public class PlayerClass : UnitClass
{
    public virtual void BasicAttack(EnemyClass target)
    {
        if (target == null)
            return;

        // Enzo changes: I made basic attack use SkillUtility so passives can affect it too
        int finalDamage = SkillUtility.BuildDamage(this, atk, false);

        SkillUtility.DealDamage(this, target, finalDamage, "Basic Attack");
    }
}

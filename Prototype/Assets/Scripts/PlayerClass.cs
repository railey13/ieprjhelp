using UnityEngine;

public class PlayerClass : UnitClass
{
    public virtual void BasicAttack(EnemyClass target)
    {
        if (target == null)
            return;

        // Enzo changes: I build the damage first so float atk turns into int damage cleanly
        int finalDamage = SkillUtility.BuildDamage(this, atk, false);

        SkillUtility.DealDamage(this, target, finalDamage, "Basic Attack", basicAttackSubtype);
    }
}
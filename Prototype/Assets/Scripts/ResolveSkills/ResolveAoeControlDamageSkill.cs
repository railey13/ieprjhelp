using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/AoE Control Damage")]
public class ResolveAoeControlDamageSkill : Skill
{
    public int damage = 15;
    public bool addUserAtk = false;
    public float aoeRadius = 3f;

    [Header("Resolve Debuff")]
    public int movementMinus = 1;
    public int rangeMinus = 0;
    public int minimumMovement = 0;
    public int minimumRange = 1;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        List<UnitClass> enemies = SkillUtility.GetUnitsAround(
            target.transform.position,
            aoeRadius,
            user,
            true,
            false
        );

        foreach (UnitClass enemy in enemies)
        {
            SkillUtility.DealDamage(user, enemy, finalDamage, SkillName);

            // Enzo changes: death field slows right away instead of doing damage over time
            if (movementMinus > 0)
                enemy.movement = Mathf.Max(minimumMovement, enemy.movement - movementMinus);

            if (rangeMinus > 0)
                enemy.range = Mathf.Max(minimumRange, enemy.range - rangeMinus);

            Debug.Log(enemy.UnitName + " got slowed by " + SkillName);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

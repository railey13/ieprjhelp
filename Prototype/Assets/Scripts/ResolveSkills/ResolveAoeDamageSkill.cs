using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/AoE Damage")]
public class ResolveAoeDamageSkill : Skill
{
    public int damage = 20;
    public bool addUserAtk = false;
    public float aoeRadius = 3f;
    public bool enemiesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        List<UnitClass> units = SkillUtility.GetUnitsAround(
            target.transform.position,
            aoeRadius,
            user,
            enemiesOnly,
            false
        );

        foreach (UnitClass unit in units)
        {
            SkillUtility.DealDamage(user, unit, finalDamage, SkillName, user.basicAttackSubtype);
        }

        // Enzo changes: the whole aoe resolves first then passives react
        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

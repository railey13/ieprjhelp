using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Arnfinn New/AOE Damage Skill")]
public class ArnfinnAoeDamageSkill : Skill
{
    [Header("Arnfinn AOE Damage")]
    public int damage = 18;
    public bool addUserAtk = true;
    public float aoeRadius = 3f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        List<UnitClass> enemies = SkillUtility.GetUnitsAround(
            target.transform.position,
            aoeRadius,
            user,
            true,
            false
        );

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        foreach (UnitClass enemy in enemies)
        {
            SkillUtility.DealDamage(user, enemy, finalDamage, SkillName, user.basicAttackSubtype);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Soren New/AOE Damage Skill")]
public class SorenAoeDamageSkill : Skill
{
    [Header("Soren AOE Damage")]
    public int damage = 25;
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
            SkillUtility.DealDamage(user, enemy, finalDamage, SkillName);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

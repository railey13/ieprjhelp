using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Huxley New/Damage Skill")]
public class HuxleyDamageSkill : Skill
{
    [Header("Huxley Damage")]
    public int damage = 20;
    public bool addUserAtk = true;
    public bool enemiesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (enemiesOnly && !SkillUtility.IsEnemyFor(user, target))
            return;

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

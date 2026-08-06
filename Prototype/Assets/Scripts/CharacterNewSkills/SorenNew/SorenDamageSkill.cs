using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Soren New/Damage Skill")]
public class SorenDamageSkill : Skill
{
    [Header("Soren Damage")]
    public int damage = 35;
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

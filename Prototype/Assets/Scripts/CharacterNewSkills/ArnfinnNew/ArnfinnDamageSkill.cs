using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Arnfinn New/Damage Skill")]
public class ArnfinnDamageSkill : Skill
{
    [Header("Arnfinn Damage")]
    public int damage = 25;
    public bool addUserAtk = true;
    public bool enemiesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (enemiesOnly && !SkillUtility.IsEnemyFor(user, target))
            return;

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName, user.basicAttackSubtype);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

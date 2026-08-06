using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Damage")]
public class ResolveDamageSkill : Skill
{
    public int damage = 20;
    public bool addUserAtk = false;
    public bool enemiesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (enemiesOnly && !SkillUtility.IsEnemyFor(user, target))
        {
            Debug.Log(SkillName + " failed because the target is not an enemy");
            return;
        }

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName);

        // Enzo changes: I let passives happen after the skill resolves
        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

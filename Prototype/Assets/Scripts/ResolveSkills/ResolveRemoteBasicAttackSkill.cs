using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Remote Basic Attack")]
public class ResolveRemoteBasicAttackSkill : Skill
{
    public bool enemiesOnly = true;
    public int bonusDamage = 0;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (enemiesOnly && !SkillUtility.IsEnemyFor(user, target))
        {
            Debug.Log(SkillName + " failed because the target is not an enemy");
            return;
        }

        // Enzo changes: this is like the skeleton hand doing the basic attack for Vaeric
        int finalDamage = SkillUtility.BuildDamage(user, user.atk + bonusDamage, false);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

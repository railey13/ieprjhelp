using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Vaeric New/Remote Basic Attack Skill")]
public class VaericRemoteBasicAttackSkill : Skill
{
    [Header("Remote Attack")]
    public float bonusDamage = 0f;
    public bool enemiesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (enemiesOnly && !VaericSkillTools.IsEnemyFor(user, target))
            return;

        float finalDamage = user.atk + bonusDamage;
        finalDamage = VaericSkillTools.ApplyVaericBonus(user, finalDamage);

        target.TakeDamage(finalDamage);

        Debug.Log(user.UnitName + " used " + SkillName + " as a remote basic attack on " + target.UnitName);

        VaericSkillTools.ApplyVaericSelfDamage(user);
    }
}

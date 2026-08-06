using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Vaeric New/Damage Skill")]
public class VaericDamageSkill : Skill
{
    [Header("Vaeric Damage")]
    public float damage = 20f;
    public bool addUserAtk = false;
    public bool enemiesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (enemiesOnly && !VaericSkillTools.IsEnemyFor(user, target))
            return;

        float finalDamage = damage;

        if (addUserAtk)
            finalDamage += user.atk;

        finalDamage = VaericSkillTools.ApplyVaericBonus(user, finalDamage);

        target.TakeDamage(finalDamage);

        Debug.Log(user.UnitName + " used " + SkillName + " on " + target.UnitName);

        VaericSkillTools.ApplyVaericSelfDamage(user);
    }
}

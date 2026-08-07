using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Vaeric New/Control Damage Skill")]
public class VaericControlDamageSkill : Skill
{
    [Header("Damage")]
    public float damage = 15f;
    public bool addUserAtk = false;

    [Header("Control")]
    public int movementMinus = 999;
    public int minimumMovement = 0;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (!VaericSkillTools.IsEnemyFor(user, target))
            return;

        float finalDamage = damage;

        if (addUserAtk)
            finalDamage += user.atk;

        finalDamage = VaericSkillTools.ApplyVaericBonus(user, finalDamage);

        target.TakeDamage(finalDamage, DamageSubtype.Dark);

        target.movement = Mathf.Max(minimumMovement, target.movement - movementMinus);

        Debug.Log(user.UnitName + " used " + SkillName + " and reduced " + target.UnitName + " movement.");

        VaericSkillTools.ApplyVaericSelfDamage(user);
    }
}

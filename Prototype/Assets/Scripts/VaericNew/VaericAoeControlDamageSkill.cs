using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Vaeric New/AOE Control Damage Skill")]
public class VaericAoeControlDamageSkill : Skill
{
    [Header("Damage")]
    public float damage = 20f;
    public bool addUserAtk = false;

    [Header("AOE")]
    public float aoeRadius = 3f;

    [Header("Control")]
    public int movementMinus = 1;
    public int minimumMovement = 0;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        List<UnitClass> units = VaericSkillTools.GetUnitsAround(target.transform.position, aoeRadius, user);

        foreach (UnitClass unit in units)
        {
            float finalDamage = damage;

            if (addUserAtk)
                finalDamage += user.atk;

            finalDamage = VaericSkillTools.ApplyVaericBonus(user, finalDamage);

            unit.TakeDamage(finalDamage, DamageSubtype.Dark);

            unit.movement = Mathf.Max(minimumMovement, unit.movement - movementMinus);

            Debug.Log(user.UnitName + " hit " + unit.UnitName + " with " + SkillName);
        }

        VaericSkillTools.ApplyVaericSelfDamage(user);
    }
}

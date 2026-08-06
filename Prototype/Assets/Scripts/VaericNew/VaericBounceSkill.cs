using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Vaeric New/Bounce Damage Skill")]
public class VaericBounceSkill : Skill
{
    [Header("Vaeric Bounce Damage")]
    public float damage = 30f;
    public bool addUserAtk = false;

    [Header("Bounce")]
    public int extraBounces = 2;
    public float bounceRange = 4f;
    public float bounceDamageMultiplier = 1f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (!VaericSkillTools.IsEnemyFor(user, target))
            return;

        List<UnitClass> hitTargets = new List<UnitClass>();

        UnitClass currentTarget = target;
        float currentDamage = damage;

        if (addUserAtk)
            currentDamage += user.atk;

        for (int i = 0; i <= extraBounces; i++)
        {
            if (currentTarget == null)
                break;

            if (hitTargets.Contains(currentTarget))
                break;

            float finalDamage = VaericSkillTools.ApplyVaericBonus(user, currentDamage);

            currentTarget.TakeDamage(finalDamage);
            hitTargets.Add(currentTarget);

            Debug.Log(user.UnitName + " bounced " + SkillName + " to " + currentTarget.UnitName);

            currentTarget = FindNextBounceTarget(user, currentTarget, hitTargets);
            currentDamage *= bounceDamageMultiplier;
        }

        VaericSkillTools.ApplyVaericSelfDamage(user);
    }

    private UnitClass FindNextBounceTarget(UnitClass user, UnitClass fromTarget, List<UnitClass> hitTargets)
    {
        UnitClass[] allUnits = Object.FindObjectsOfType<UnitClass>();

        UnitClass closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (UnitClass unit in allUnits)
        {
            if (unit == null || unit.hp <= 0)
                continue;

            if (hitTargets.Contains(unit))
                continue;

            if (!VaericSkillTools.IsEnemyFor(user, unit))
                continue;

            float distance = Vector3.Distance(fromTarget.transform.position, unit.transform.position);

            if (distance <= bounceRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = unit;
            }
        }

        return closestTarget;
    }
}

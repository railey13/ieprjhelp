using System.Collections.Generic;
using UnityEngine;

public static class VaericSkillTools
{
    public static bool IsEnemyFor(UnitClass user, UnitClass target)
    {
        if (user is PlayerClass && target is EnemyClass)
            return true;

        if (user is EnemyClass && target is PlayerClass)
            return true;

        return false;
    }

    public static float ApplyVaericBonus(UnitClass user, float damage)
    {
        VaericUnit vaeric = user as VaericUnit;

        if (vaeric != null)
            return vaeric.ApplyVaericDamageBonus(damage);

        return damage;
    }

    public static void ApplyVaericSelfDamage(UnitClass user)
    {
        VaericUnit vaeric = user as VaericUnit;

        if (vaeric != null)
            vaeric.TakeSelfDamageFromAction();
    }

    public static List<UnitClass> GetUnitsAround(Vector3 center, float radius, UnitClass user)
    {
        List<UnitClass> unitsFound = new List<UnitClass>();

        UnitClass[] allUnits = Object.FindObjectsOfType<UnitClass>();

        foreach (UnitClass unit in allUnits)
        {
            if (unit == null || unit.hp <= 0)
                continue;

            if (!IsEnemyFor(user, unit))
                continue;

            float distance = Vector3.Distance(center, unit.transform.position);

            if (distance <= radius)
                unitsFound.Add(unit);
        }

        return unitsFound;
    }
}

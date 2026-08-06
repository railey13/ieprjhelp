using System.Collections.Generic;
using UnityEngine;

public interface IOnSkillUsedPassive
{
    void OnSkillUsed(UnitClass user, UnitClass target, Skill skill);
}

public interface IOutgoingDamageModifier
{
    int ModifyOutgoingDamage(int damage);
}

public static class SkillUtility
{
    public static bool IsEnemyFor(UnitClass user, UnitClass target)
    {
        if (user is PlayerClass && target is EnemyClass)
            return true;

        if (user is EnemyClass && target is PlayerClass)
            return true;

        return false;
    }

    public static bool IsAllyFor(UnitClass user, UnitClass target)
    {
        if (user is PlayerClass && target is PlayerClass)
            return true;

        if (user is EnemyClass && target is EnemyClass)
            return true;

        return false;
    }

    public static int BuildDamage(UnitClass user, float baseDamage, bool addUserAtk)
    {
        float damage = baseDamage;

        if (addUserAtk && user != null)
            damage += user.atk;

        // Enzo changes: I convert to int here so all the skill files dont have to care about float damage
        int finalDamage = Mathf.FloorToInt(Mathf.Max(0f, damage));

        if (user != null)
        {
            MonoBehaviour[] behaviours = user.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IOutgoingDamageModifier modifier)
                    finalDamage = modifier.ModifyOutgoingDamage(finalDamage);
            }
        }

        return Mathf.Max(0, finalDamage);
    }

    public static int DealDamage(UnitClass user, UnitClass target, int damage, string skillName)
    {
        if (user == null || target == null)
            return 0;

        int hpBefore = target.hp;

        // Enzo changes: UnitClass takes float damage but int still works because C# can pass int as float
        target.TakeDamage(damage);

        int damageDealt = Mathf.Max(0, hpBefore - target.hp);

        // Enzo changes: I put this here so Darrene can gain resource from custom damage skills
        if (user is DarreneUnit darrene)
            darrene.GainResourceFromDealing(damageDealt);

        Debug.Log(user.UnitName + " used " + skillName + " on " + target.UnitName + " for " + damageDealt + " damage");

        return damageDealt;
    }

    public static void NotifySkillUsed(UnitClass user, UnitClass target, Skill skill)
    {
        if (user == null)
            return;

        MonoBehaviour[] behaviours = user.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IOnSkillUsedPassive passive)
                passive.OnSkillUsed(user, target, skill);
        }
    }

    public static List<UnitClass> GetUnitsAround(Vector3 center, float radius, UnitClass user, bool enemiesOnly, bool alliesOnly)
    {
        List<UnitClass> unitsFound = new List<UnitClass>();

        UnitClass[] allUnits = Object.FindObjectsOfType<UnitClass>();

        foreach (UnitClass unit in allUnits)
        {
            if (unit == null || unit.hp <= 0)
                continue;

            if (enemiesOnly && !IsEnemyFor(user, unit))
                continue;

            if (alliesOnly && !IsAllyFor(user, unit))
                continue;

            float distance = Vector3.Distance(center, unit.transform.position);

            if (distance <= radius)
                unitsFound.Add(unit);
        }

        return unitsFound;
    }
}
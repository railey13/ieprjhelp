using System.Collections.Generic;
using UnityEngine;

public enum ResolveBuffStat
{
    Attack,
    Movement,
    Range,
    Speed
}

[CreateAssetMenu(menuName = "Skills/Resolve/Buff")]
public class ResolveBuffSkill : Skill
{
    public ResolveBuffStat statToBuff = ResolveBuffStat.Attack;
    public int amount = 5;
    public bool alliesOnly = true;

    [Header("Maria Enhanced")]
    public bool canBecomeAoe = true;
    public float enhancedAoeRadius = 3f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (alliesOnly && !SkillUtility.IsAllyFor(user, target))
        {
            Debug.Log(SkillName + " failed because the target is not an ally");
            return;
        }

        MariaPassive maria = user.GetComponent<MariaPassive>();
        bool useAoe = canBecomeAoe && maria != null && maria.IsEnhanced;

        if (useAoe)
        {
            List<UnitClass> allies = SkillUtility.GetUnitsAround(
                target.transform.position,
                enhancedAoeRadius,
                user,
                false,
                true
            );

            foreach (UnitClass ally in allies)
            {
                ApplyBuff(ally);
            }
        }
        else
        {
            ApplyBuff(target);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }

    private void ApplyBuff(UnitClass target)
    {
        switch (statToBuff)
        {
            case ResolveBuffStat.Attack:
                target.atk += amount;
                break;

            case ResolveBuffStat.Movement:
                target.movement += amount;
                break;

            case ResolveBuffStat.Range:
                target.range += amount;
                break;

            case ResolveBuffStat.Speed:
                target.speed += amount;
                break;
        }

        // Enzo changes: buffs are permanent for now because I dont have duration yet
        Debug.Log(target.UnitName + " got +" + amount + " " + statToBuff);
    }
}

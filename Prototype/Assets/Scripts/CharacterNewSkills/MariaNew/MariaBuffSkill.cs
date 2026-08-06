using System.Collections.Generic;
using UnityEngine;

public enum MariaBuffStat
{
    Attack,
    Movement,
    Range
}

[CreateAssetMenu(menuName = "Skills/Maria New/Buff Skill")]
public class MariaBuffSkill : Skill
{
    [Header("Buff")]
    public MariaBuffStat statToBuff = MariaBuffStat.Attack;
    public int amount = 10;
    public bool alliesOnly = true;

    [Header("Maria Enhanced")]
    public bool canBecomeAoe = true;
    public float enhancedAoeRadius = 3f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (alliesOnly && !SkillUtility.IsAllyFor(user, target))
            return;

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
                ApplyBuff(user, ally);
            }
        }
        else
        {
            ApplyBuff(user, target);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }

    private void ApplyBuff(UnitClass user, UnitClass target)
    {
        if (statToBuff == MariaBuffStat.Attack)
            target.atk += amount;
        else if (statToBuff == MariaBuffStat.Movement)
            target.movement += amount;
        else if (statToBuff == MariaBuffStat.Range)
            target.range += amount;

        Debug.Log(user.UnitName + " buffed " + target.UnitName + " with " + SkillName);
    }
}

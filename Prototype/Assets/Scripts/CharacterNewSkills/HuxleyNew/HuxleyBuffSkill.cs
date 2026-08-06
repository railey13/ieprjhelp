using UnityEngine;

public enum HuxleyBuffStat
{
    Attack,
    Movement,
    Range
}

[CreateAssetMenu(menuName = "Skills/Huxley New/Buff Skill")]
public class HuxleyBuffSkill : Skill
{
    [Header("Buff")]
    public HuxleyBuffStat statToBuff = HuxleyBuffStat.Attack;
    public int amount = 10;

    [Header("Targeting")]
    public bool buffSelfIfTargetIsInvalid = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null)
            return;

        UnitClass finalTarget = target;

        // Enzo changes: if I target the wrong thing this just buffs Huxley instead
        if (finalTarget == null || !SkillUtility.IsAllyFor(user, finalTarget))
        {
            if (!buffSelfIfTargetIsInvalid)
            {
                Debug.Log(SkillName + " failed because the target is not an ally");
                return;
            }

            finalTarget = user;
        }

        if (statToBuff == HuxleyBuffStat.Attack)
        {
            finalTarget.atk += amount;
        }
        else if (statToBuff == HuxleyBuffStat.Movement)
        {
            finalTarget.movement += amount;
        }
        else if (statToBuff == HuxleyBuffStat.Range)
        {
            finalTarget.range += amount;
        }

        Debug.Log(user.UnitName + " buffed " + finalTarget.UnitName + " with " + SkillName);

        SkillUtility.NotifySkillUsed(user, finalTarget, this);
    }
}
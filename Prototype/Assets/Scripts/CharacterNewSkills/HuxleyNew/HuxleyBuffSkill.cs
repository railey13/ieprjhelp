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
    public bool alliesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (alliesOnly && !SkillUtility.IsAllyFor(user, target))
            return;

        if (statToBuff == HuxleyBuffStat.Attack)
            target.atk += amount;
        else if (statToBuff == HuxleyBuffStat.Movement)
            target.movement += amount;
        else if (statToBuff == HuxleyBuffStat.Range)
            target.range += amount;

        Debug.Log(user.UnitName + " buffed " + target.UnitName + " with " + SkillName);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

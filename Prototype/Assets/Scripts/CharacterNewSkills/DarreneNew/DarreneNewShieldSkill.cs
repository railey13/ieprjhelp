using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Darrene New/Shield Skill")]
public class DarreneNewShieldSkill : Skill
{
    [Header("Shield")]
    public int shieldAmount = 25;
    public bool alliesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (alliesOnly && !SkillUtility.IsAllyFor(user, target))
            return;

        // Enzo changes: this is temporary shield hp since there isnt a separate shield stat yet
        target.hp += shieldAmount;

        Debug.Log(user.UnitName + " gave " + target.UnitName + " shield hp worth " + shieldAmount);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

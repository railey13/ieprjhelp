using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Shield")]
public class ResolveShieldSkill : Skill
{
    public int shieldAmount = 25;
    public bool alliesOnly = true;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (alliesOnly && !SkillUtility.IsAllyFor(user, target))
        {
            Debug.Log(SkillName + " failed because the target is not an ally");
            return;
        }

        // Enzo changes: simple shield for now since UnitClass doesnt have actual shield
        target.hp += shieldAmount;

        Debug.Log(user.UnitName + " gave " + target.UnitName + " shield HP worth " + shieldAmount);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

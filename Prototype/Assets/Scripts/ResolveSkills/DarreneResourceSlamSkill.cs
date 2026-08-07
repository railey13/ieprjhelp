using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Darrene Resource Slam")]
public class DarreneResourceSlamSkill : Skill
{
    public int resourceCost = 5;
    public int damage = 40;
    public bool addUserAtk = true;
    public float aoeRadius = 4f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        DarreneUnit darrene = user as DarreneUnit;

        if (darrene == null)
        {
            Debug.Log(SkillName + " can only be used by Darrene");
            return;
        }

        if (!darrene.SpendResource(resourceCost))
        {
            Debug.Log("Not enough resource for " + SkillName);
            return;
        }

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        List<UnitClass> enemies = SkillUtility.GetUnitsAround(
            target.transform.position,
            aoeRadius,
            user,
            true,
            false
        );

        foreach (UnitClass enemy in enemies)
        {
            SkillUtility.DealDamage(user, enemy, finalDamage, SkillName, user.basicAttackSubtype);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

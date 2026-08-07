using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Darrene New/Resource Slam Skill")]
public class DarreneNewResourceSlamSkill : Skill
{
    [Header("Resource")]
    public int resourceCost = 5;

    [Header("Damage")]
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
            return;

        List<UnitClass> enemies = SkillUtility.GetUnitsAround(
            target.transform.position,
            aoeRadius,
            user,
            true,
            false
        );

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        foreach (UnitClass enemy in enemies)
        {
            SkillUtility.DealDamage(user, enemy, finalDamage, SkillName, user.basicAttackSubtype);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

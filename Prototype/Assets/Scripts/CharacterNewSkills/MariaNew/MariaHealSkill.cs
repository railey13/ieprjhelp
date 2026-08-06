using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Maria New/Heal Skill")]
public class MariaHealSkill : Skill
{
    [Header("Heal")]
    public int healAmount = 30;
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
                ally.Heal(healAmount);
                Debug.Log(user.UnitName + " healed " + ally.UnitName + " for " + healAmount);
            }
        }
        else
        {
            target.Heal(healAmount);
            Debug.Log(user.UnitName + " healed " + target.UnitName + " for " + healAmount);
        }

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

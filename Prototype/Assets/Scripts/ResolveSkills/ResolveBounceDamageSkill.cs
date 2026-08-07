using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Bounce Damage")]
public class ResolveBounceDamageSkill : Skill
{
    public int damage = 25;
    public bool addUserAtk = false;

    [Header("Bounce")]
    public int extraBounces = 2;
    public float bounceRange = 4f;
    public float bounceDamageMultiplier = 1f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (!SkillUtility.IsEnemyFor(user, target))
        {
            Debug.Log(SkillName + " failed because the target is not an enemy");
            return;
        }

        List<UnitClass> hitUnits = new List<UnitClass>();
        UnitClass currentTarget = target;

        int baseDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        for (int i = 0; i <= extraBounces; i++)
        {
            if (currentTarget == null || currentTarget.hp <= 0)
                break;

            int currentDamage = Mathf.RoundToInt(baseDamage * Mathf.Pow(bounceDamageMultiplier, i));

            SkillUtility.DealDamage(user, currentTarget, currentDamage, SkillName, user.basicAttackSubtype);

            hitUnits.Add(currentTarget);

            UnitClass nextTarget = null;
            float closestDistance = float.MaxValue;

            List<UnitClass> nearbyEnemies = SkillUtility.GetUnitsAround(
                currentTarget.transform.position,
                bounceRange,
                user,
                true,
                false
            );

            foreach (UnitClass enemy in nearbyEnemies)
            {
                if (enemy == null || hitUnits.Contains(enemy))
                    continue;

                float distance = Vector3.Distance(currentTarget.transform.position, enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nextTarget = enemy;
                }
            }

            currentTarget = nextTarget;
        }

        // Enzo changes: I only call this once after the full bounce finishes
        SkillUtility.NotifySkillUsed(user, target, this);
    }
}

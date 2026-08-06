using System.Collections.Generic;
using UnityEngine;

public class DarreneUnit : PlayerClass
{
    [Header("Resource")]
    public int currentResource = 0;
    public int maxResource = 10;
    public int resourceGainWhenHit = 2;
    public int resourceGainWhenDealingDamage = 1;

    [Header("Counter")]
    public bool counterReady = false;
    public int counterDamage = 40;
    public float counterRadius = 3f;

    public override void BasicAttack(EnemyClass target)
    {
        if (target == null)
            return;

        // Enzo changes: Darrene basic attack hits enemies near the selected target
        List<UnitClass> enemies = SkillUtility.GetUnitsAround(
            target.transform.position,
            2.5f,
            this,
            true,
            false
        );

        foreach (UnitClass enemy in enemies)
        {
            SkillUtility.DealDamage(this, enemy, atk, "Darrene Basic AoE");
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (hp <= 0)
            return;

        if (damage > 0)
            GainResource(resourceGainWhenHit);

        if (counterReady)
        {
            counterReady = false;

            List<UnitClass> enemies = SkillUtility.GetUnitsAround(
                transform.position,
                counterRadius,
                this,
                true,
                false
            );

            foreach (UnitClass enemy in enemies)
            {
                SkillUtility.DealDamage(this, enemy, counterDamage, "Counter");
            }

            Debug.Log(UnitName + " countered after being hit");
        }
    }

    public void GainResourceFromDealing(int damageDealt)
    {
        if (damageDealt <= 0)
            return;

        GainResource(resourceGainWhenDealingDamage);
    }

    public void GainResource(int amount)
    {
        currentResource = Mathf.Min(maxResource, currentResource + amount);

        Debug.Log(UnitName + " resource: " + currentResource + "/" + maxResource);
    }

    public bool SpendResource(int amount)
    {
        if (currentResource < amount)
            return false;

        currentResource -= amount;

        Debug.Log(UnitName + " spent " + amount + " resource");

        return true;
    }

    public void SetCounterReady()
    {
        counterReady = true;
    }
}

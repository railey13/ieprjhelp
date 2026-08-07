using System.Collections.Generic;
using UnityEngine;

public class DarreneUnit : PlayerClass
{
    [Header("Darrene Resource")]
    public int currentResource = 0;
    public int maxResource = 10;
    public int resourceGainWhenHit = 2;
    public int resourceGainWhenDealingDamage = 1;

    [Header("Darrene Counter")]
    public bool counterReady = false;
    public int counterDamage = 40;
    public float counterRadius = 3f;

    [Header("Basic Attack AOE")]
    public float basicAttackAoeRadius = 2.5f;

    public override void BasicAttack(EnemyClass target)
    {
        if (target == null)
            return;

        // Enzo changes: Darrene basic attack hits enemies close to the target
        List<UnitClass> enemies = SkillUtility.GetUnitsAround(
            target.transform.position,
            basicAttackAoeRadius,
            this,
            true,
            false
        );

        int finalDamage = SkillUtility.BuildDamage(this, atk, false);

        foreach (UnitClass enemy in enemies)
        {
            SkillUtility.DealDamage(this, enemy, finalDamage, "Darrene Basic AOE", this.basicAttackSubtype);
        }

        PlayAttackAnimation();
    }

    public override void TakeDamage(float damage, DamageSubtype subtype)
    {
        base.TakeDamage(damage, subtype);

        if (hp <= 0)
            return;

        if (damage > 0)
            GainResource(resourceGainWhenHit);

        if (counterReady)
            TriggerCounter();
    }

    private void TriggerCounter()
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
            SkillUtility.DealDamage(this, enemy, counterDamage, "Counter", this.basicAttackSubtype);
        }

        PlayAttackAnimation();

        Debug.Log(UnitName + " countered after being hit");
    }

    public void GainResourceFromDealing(int damageDealt)
    {
        if (damageDealt <= 0)
            return;

        GainResource(resourceGainWhenDealingDamage);
    }

    public void GainResource(int amount)
    {
        if (amount <= 0)
            return;

        currentResource = Mathf.Min(maxResource, currentResource + amount);

        Debug.Log(UnitName + " resource: " + currentResource + "/" + maxResource);
    }

    public bool SpendResource(int amount)
    {
        if (currentResource < amount)
        {
            Debug.Log(UnitName + " does not have enough resource");
            return false;
        }

        currentResource -= amount;

        Debug.Log(UnitName + " spent " + amount + " resource");

        return true;
    }

    public void SetCounterReady()
    {
        counterReady = true;

        Debug.Log(UnitName + " is ready to counter");
    }
}

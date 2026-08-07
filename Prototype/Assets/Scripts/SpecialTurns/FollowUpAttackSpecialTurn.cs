using UnityEngine;

[CreateAssetMenu(menuName = "Special Turns/Follow Up Attack")]
public class FollowUpAttackSpecialTurn : SpecialTurnScript
{
    [Header("Range")]
    public bool useOwnerRange = true;
    public float customRange = 3f;
    public float rangeBuffer = 0.5f;

    [Header("Damage")]
    public bool useOwnerAttackStat = true;
    public int customDamage = 10;

    protected override bool ExtraCanTrigger(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner)
    {
        float attackRange = useOwnerRange ? owner.range : customRange;
        float distance = Vector3.Distance(owner.transform.position, context.target.transform.position);

        Debug.Log("FOLLOW UP DEBUG: " + owner.UnitName + " distance to target = " + distance);
        Debug.Log("FOLLOW UP DEBUG: allowed follow-up range = " + (attackRange + rangeBuffer));

        if (distance > attackRange + rangeBuffer)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because target is out of range");
            return false;
        }

        return true;
    }

    public override void Activate(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner)
    {
        runner.tracker.MarkUsed(owner, this, context.target, limit);

        float baseDamage = useOwnerAttackStat ? owner.atk : customDamage;

        // Enzo changes: I run follow up damage through SkillUtility so passives can still affect it
        int finalDamage = SkillUtility.BuildDamage(owner, baseDamage, false);

        Debug.Log("FOLLOW UP DEBUG: " + owner.UnitName + " follow-up damage = " + finalDamage);

        SkillUtility.DealDamage(owner, context.target, finalDamage, specialTurnName, owner.basicAttackSubtype);

        Debug.Log(owner.UnitName + " triggered " + specialTurnName + " on " + context.target.UnitName);
    }
}